using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class EventSeatRepository : IRepository<EventSeat>
    {
        private readonly TicketManagementContext _context;

        public EventSeatRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(EventSeat item)
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

        public IQueryable<EventSeat> GetAll()
        {
            return _context.EventSeats.AsQueryable();
        }

        public Task<EventSeat> GetByIdAsync(int id)
        {
            return _context.EventSeats.Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(EventSeat item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
