using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class CategoryDAO : ICategoryDAO
    {
        private readonly IUOW _uow;
        private readonly IGenericRepository<Category> _repo;

        public CategoryDAO(IUOW uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<Category>();
        }

        public async Task<IEnumerable<Category>> GetAllAsync() =>
            await _repo.Entities
                       .Include(c => c.Orchids)
                       .ToListAsync();

        public async Task<Category?> GetByIdAsync(int id) =>
            await _repo.Entities
                       .Include(c => c.Orchids)
                       .FirstOrDefaultAsync(c => c.CategoryId == id);

        public async Task InsertAsync(Category category) =>
            await _repo.InsertAsync(category);

        public Task UpdateAsync(Category category)
        {
            _repo.Update(category);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Category category)
        {
            _repo.Delete(category);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() =>
            await _uow.SaveAsync();
    }
}
