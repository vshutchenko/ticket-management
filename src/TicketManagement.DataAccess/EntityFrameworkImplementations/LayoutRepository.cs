using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class LayoutRepository : IRepository<Layout>
    {
        private readonly TicketManagementContext _context;

        public LayoutRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(Layout item)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Layout> entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            Layout entity = _context.Layouts.FirstOrDefault(l => l.Id == id);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Layout> GetAll()
        {
            return _context.Layouts.AsQueryable();
        }

        public Task<Layout> GetByIdAsync(int id)
        {
            return _context.Layouts.Where(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Layout item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
