using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IOrchidDAO
    {
        Task<IEnumerable<Orchid>> GetAllWithCategoryAsync();
        Task<IEnumerable<Orchid>> GetAllAsync();
        Task<Orchid?> GetByIdAsync(int id);
        Task<IEnumerable<Orchid>> GetByNameAsync(string name);
        Task InsertAsync(Orchid orchid);
        Task UpdateAsync(Orchid orchid);
        Task DeleteAsync(Orchid orchid);
        Task SaveAsync();
        Task<Orchid?> GetByIdWithCategoryAsync(int id);
    }
}
