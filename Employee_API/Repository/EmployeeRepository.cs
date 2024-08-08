using Employee_API.Data;
using Employee_API.Models;
using Employee_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace Employee_API.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext appDbContext;

        public EmployeeRepository(ApplicationDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public async Task CreateAsync(Employee entity)
        {
            await appDbContext.Employee_Table.AddAsync(entity);
            await appDbContext.SaveChangesAsync();
        }


        public async Task DeleteAsync(Employee entity)
        {
            appDbContext.Employee_Table.Remove(entity);
            await appDbContext.SaveChangesAsync();
        }



        public async Task<Employee?> GetAsync(Expression<Func<Employee, bool>>? filter = null)
        {
            IQueryable<Employee> query = appDbContext.Employee_Table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var result = await query.FirstOrDefaultAsync();


            return result;
        }



        public async Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? filter = null)
        {
            IQueryable<Employee> query = appDbContext.Employee_Table;

            if(filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }



        public async Task UpdateAsync(Employee entity)
        {
            appDbContext.Employee_Table.Update(entity);
            await appDbContext.SaveChangesAsync();
        }
    }
}
