using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Data;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    public class TicketManagementContext : IdentityDbContext<User>
    {
        public TicketManagementContext(DbContextOptions<TicketManagementContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }

        public DbSet<Layout> Layouts { get; set; }

        public DbSet<Area> Areas { get; set; }

        public DbSet<Seat> Seats { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventArea> EventAreas { get; set; }

        public DbSet<EventSeat> EventSeats { get; set; }

        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchasedSeat> PurchasedSeats { get; set; }
    }
}