using Employee_API.Models;
using System.Linq.Expressions;

namespace Employee_API.Repository.IRepository
{
    public interface IEmployeeRepository
    {
        Task CreateAsync(Employee entity);

        Task UpdateAsync(Employee entity);

        Task DeleteAsync(Employee entity);

        Task Save();

        Task<Employee> GetAsync(Expression<Func<Employee, bool>>? filter = null);

        Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? filter = null);

    }
}
