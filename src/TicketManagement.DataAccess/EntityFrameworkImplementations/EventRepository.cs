using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.DataAccess.EntityFrameworkImplementations
{
    internal class EventRepository : IRepository<Event>
    {
        private readonly TicketManagementContext _context;

        public EventRepository(TicketManagementContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateAsync(Event item)
        {
            var sqlCommand = "EXEC InsertEvent @name, @description, @layoutId, @startDate, @endDate, @eventId OUT";

            var idParam = new SqlParameter("@eventId", SqlDbType.Int) { Direction = ParameterDirection.Output };

            var parameters = new SqlParameter[]
            {
                idParam,
                new SqlParameter("@name", item.Name),
                new SqlParameter("@description", item.Description),
                new SqlParameter("@layoutId", item.LayoutId),
                new SqlParameter("@startDate", item.StartDate),
                new SqlParameter("@endDate", item.EndDate),
            };

            await _context.Database.ExecuteSqlCommandAsync(sqlCommand, parameters);

            await _context.SaveChangesAsync();

            var id = (int)idParam.Value;

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            _context.Remove(id);

            await _context.SaveChangesAsync();
        }

        public IQueryable<Event> GetAll()
        {
            return _context.Events.AsQueryable();
        }

        public Task<Event> GetByIdAsync(int id)
        {
            return _context.Events.Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Event item)
        {
            var sqlCommand = "EXEC UpdateEvent @eventId, @name, @description, @layoutId, @startDate, @endDate";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@eventId", item.Id),
                new SqlParameter("@name", item.Name),
                new SqlParameter("@description", item.Description),
                new SqlParameter("@layoutId", item.LayoutId),
                new SqlParameter("@startDate", item.StartDate),
                new SqlParameter("@endDate", item.EndDate),
            };

            await _context.Database.ExecuteSqlCommandAsync(sqlCommand, parameters);

            await _context.SaveChangesAsync();
        }
    }
}
