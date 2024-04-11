
using PetaPoco;

namespace FakelStat.Repositories;

public class SqlRepository<T> : IRepository<T> where T : class
{
    private readonly IDatabase _database;
    protected IDatabase Database { get => _database; }

    public SqlRepository(IDatabase database) =>
        _database = database;

    public async ValueTask<IEnumerable<T>> GetAllAsync() =>
        await _database.FetchAsync<T>($"SELECT * FROM {typeof(T).Name}");

    public async ValueTask<T> GetByIdAsync(int id) =>
        await _database.FirstAsync<T>($"SELECT * FROM {typeof(T).Name} WHERE Id = @0", id);

    public async ValueTask<int> InsertAsync(T entity) =>
        (int)await _database.InsertAsync(entity);

    public async ValueTask UpdateAsync(T entity) =>
         await _database.UpdateAsync(entity);

    public async ValueTask DeleteAsync(T entity) =>
        await _database.DeleteAsync(entity);
}
