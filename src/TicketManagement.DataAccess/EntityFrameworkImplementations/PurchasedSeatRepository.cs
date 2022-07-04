﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class PurchasedSeatRepository : IRepository<PurchasedSeat>
    {
        private readonly TicketManagementContext _context;

        public PurchasedSeatRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(PurchasedSeat item)
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

        public IQueryable<PurchasedSeat> GetAll()
        {
            return _context.PurchasedSeats.AsQueryable();
        }

        public Task<PurchasedSeat> GetByIdAsync(int id)
        {
            return _context.PurchasedSeats.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PurchasedSeat item)
        {
            _context.Update(item);

            await _context.SaveChangesAsync();
        }
    }
}
