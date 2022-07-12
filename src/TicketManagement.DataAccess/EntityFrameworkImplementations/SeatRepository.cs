using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class SeatRepository : IRepository<Seat>
    {
        private readonly TicketManagementContext _context;

        public SeatRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(Seat item)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Seat> entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            Seat entity = _context.Seats.FirstOrDefault(s => s.Id == id);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Seat> GetAll()
        {
            return _context.Seats.AsQueryable();
        }

        public Task<Seat> GetByIdAsync(int id)
        {
            return _context.Seats.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Seat item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
