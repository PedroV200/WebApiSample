namespace WebApiSample.Infrastructure;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
}