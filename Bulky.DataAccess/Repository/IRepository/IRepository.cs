using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T - Category
        IEnumerable<T> GetAll(string? includeProperties = null);
        
        // Added tracked to stop EF tracking object and updating without the Update() - Very strange bug. - PM 
        //T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        // void Update(T entity); Added to UnitOfWork class
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
