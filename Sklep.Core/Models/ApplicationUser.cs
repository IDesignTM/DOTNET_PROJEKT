using Microsoft.AspNetCore.Identity;

namespace Sklep.Core.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}