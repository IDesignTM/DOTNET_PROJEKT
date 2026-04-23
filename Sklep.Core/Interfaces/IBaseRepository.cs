using System.Linq.Expressions;
using Sklep.Core.Models;

namespace Sklep.Core.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    // Pobierz wszystko
    Task<IEnumerable<T>> GetAllAsync();
    
    // Pobierz jeden po ID
    Task<T?> GetByIdAsync(int id);
    
    // Dodaj nowy obiekt
    Task AddAsync(T entity);
    
    // Zaktualizuj (operacja w pamięci)
    void Update(T entity);
    
    // Usuń (operacja w pamięci)
    void Delete(T entity);
    
    // Zapisz zmiany w bazie (asynchronicznie)
    Task<bool> SaveChangesAsync();
}