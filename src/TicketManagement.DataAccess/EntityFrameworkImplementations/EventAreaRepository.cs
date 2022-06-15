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
            var entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            _context.Remove(id);

            await _context.SaveChangesAsync();
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
