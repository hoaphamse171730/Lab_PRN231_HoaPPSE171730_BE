using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface ICategoryDAO
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task InsertAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task SaveAsync();
    }
}