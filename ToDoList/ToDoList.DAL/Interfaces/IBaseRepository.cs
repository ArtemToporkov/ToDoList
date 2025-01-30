namespace ToDoList.DAL.Interfaces;

public interface IBaseRepository<T>
{
    public Task Create(T entity);
    public IQueryable<T> GetAll();
    public Task Delete(T entity);
    public Task<T> Update(T entity);
}