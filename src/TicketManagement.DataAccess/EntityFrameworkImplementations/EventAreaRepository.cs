using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class EventAreaRepository : IRepository<EventArea>
    {
        private readonly TicketManagementContext _context;

        public EventAreaRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(EventArea item)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<EventArea> entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            EventArea entity = _context.EventAreas.FirstOrDefault(a => a.Id == id);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<EventArea> GetAll()
        {
            return _context.EventAreas.AsQueryable();
        }

        public Task<EventArea> GetByIdAsync(int id)
        {
            return _context.EventAreas.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(EventArea item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
