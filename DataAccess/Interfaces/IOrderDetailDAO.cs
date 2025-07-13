using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IOrderDetailDAO
    {
        Task<IEnumerable<OrderDetail>> GetAllAsync();
        Task<OrderDetail?> GetByIdAsync(int id);
        Task InsertAsync(OrderDetail detail);
        Task UpdateAsync(OrderDetail detail);
        Task DeleteAsync(OrderDetail detail);
        Task SaveAsync();
    }
}
