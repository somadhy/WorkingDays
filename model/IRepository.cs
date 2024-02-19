public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> Find(Func<T, bool> where);

    Task<IEnumerable<T>> GetAll();

    Task Create(T t);

    Task Update(T t);
}