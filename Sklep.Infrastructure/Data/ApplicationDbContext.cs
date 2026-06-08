using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sklep.Core.Models;

namespace Sklep.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRates => Set<CurrencyExchangeRate>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<DiscountCode> DiscountCodes => Set<DiscountCode>();
    public DbSet<ProductQuestion> ProductQuestions => Set<ProductQuestion>();
    public DbSet<DiscountCodeUsage> DiscountCodeUsages => Set<DiscountCodeUsage>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<LoginHistory> LoginHistories => Set<LoginHistory>();
    public DbSet<ProductViewHistory> ProductViewHistories => Set<ProductViewHistory>();
    public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(decimal));

            foreach (var property in properties)
            {
                builder.Entity(entityType.Name).Property(property.Name).HasConversion<string>();
            }
        }

        builder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Tag>().HasData(
            new Tag { Id = 1, Name = "Nowość" },
            new Tag { Id = 2, Name = "Bestseller" },
            new Tag { Id = 3, Name = "Wyprzedaż" }
        );

        builder.Entity<OrderItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId);
        
        builder.Entity<Size>().HasData(
            new Size { Id = 1, Name = "XS" },
            new Size { Id = 2, Name = "S" },
            new Size { Id = 3, Name = "M" },
            new Size { Id = 4, Name = "L" },
            new Size { Id = 5, Name = "XL" },
            new Size { Id = 6, Name = "One size" }
        );

        builder.Entity<Order>()
            .HasOne(o => o.Payment)
            .WithOne(p => p.Order)
            .HasForeignKey<Payment>(p => p.OrderId
        );

        builder.Entity<Address>()
            .HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId
        );

        builder.Entity<ShippingMethod>().HasData(
            new ShippingMethod { Id = 1, Name = "Kurier DHL", Price = 15, IsActive = true },
            new ShippingMethod { Id = 2, Name = "Paczkomat", Price = 10, IsActive = true },
            new ShippingMethod { Id = 3, Name = "Odbiór osobisty", Price = 0, IsActive = true }
        );
    }
}