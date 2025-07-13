using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IAccountDAO
    {
        Task<Account?> GetByEmailAsync(string email);

        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task InsertAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
        Task SaveAsync();
    }
}
