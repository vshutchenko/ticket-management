using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class PurchaseRepository : IRepository<Purchase>
    {
        private readonly TicketManagementContext _context;

        public PurchaseRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(Purchase item)
        {
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Purchase> entityEntry = await _context.AddAsync(item);

            await _context.SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task DeleteAsync(int id)
        {
            Purchase entity = _context.Purchases.FirstOrDefault(p => p.Id == id);

            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public Task<Purchase> GetByIdAsync(int id)
        {
            return _context.Purchases.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Purchase item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }

        public IQueryable<Purchase> GetAll()
        {
            return _context.Purchases.AsQueryable();
        }
    }
}
