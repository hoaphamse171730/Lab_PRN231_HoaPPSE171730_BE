using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IRoleDAO
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task InsertAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
        Task SaveAsync();
    }
}
