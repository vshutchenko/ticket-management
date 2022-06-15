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
            var entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            _context.Remove(id);

            await _context.SaveChangesAsync();
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
