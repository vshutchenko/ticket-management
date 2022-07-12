using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class AreaRepository : IRepository<Area>
    {
        private readonly TicketManagementContext _context;

        public AreaRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(Area item)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Area> entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            Area entity = _context.Areas.FirstOrDefault(a => a.Id == id);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Area> GetAll()
        {
            return _context.Areas.AsQueryable();
        }

        public Task<Area> GetByIdAsync(int id)
        {
            return _context.Areas.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Area item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
