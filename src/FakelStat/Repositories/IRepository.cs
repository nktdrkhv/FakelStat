namespace FakelStat.Repositories;

public interface IRepository<T> where T : class
{
    ValueTask<IEnumerable<T>> GetAllAsync();
    ValueTask<T> GetByIdAsync(int id);
    ValueTask<int> InsertAsync(T entity);
    ValueTask UpdateAsync(T entity);
    ValueTask DeleteAsync(T entity);
}