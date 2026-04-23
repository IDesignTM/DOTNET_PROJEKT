using Microsoft.EntityFrameworkCore;
using Sklep.Core.Interfaces;
using Sklep.Infrastructure.Data;
using Sklep.Core.Models;

namespace Sklep.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync() => 
        await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => 
        await _context.Set<T>().FindAsync(id);

    public async Task AddAsync(T entity) => 
        await _context.Set<T>().AddAsync(entity);

    public void Update(T entity) => 
        _context.Set<T>().Update(entity);

    public void Delete(T entity) => 
        _context.Set<T>().Remove(entity);

    public async Task<bool> SaveChangesAsync() => 
        await _context.SaveChangesAsync() > 0;
}