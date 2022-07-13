using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class VenueRepository : IRepository<Venue>
    {
        private readonly TicketManagementContext _context;

        public VenueRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(Venue item)
        {
            var entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = _context.Areas.FirstOrDefault(v => v.Id == id);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<Venue> GetAll()
        {
            return _context.Venues.AsQueryable();
        }

        public Task<Venue> GetByIdAsync(int id)
        {
            return _context.Venues.Where(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Venue item)
        {
            var venue = await GetByIdAsync(item.Id);

            _context.Entry(venue).State = EntityState.Detached;

            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
