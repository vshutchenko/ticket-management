using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
            string sqlCommand = "EXEC InsertEvent @name={0}, @description={1}, @layoutId={2}, @startDate={3}, @endDate={4}, @published={5}, @eventId={6} OUT";

            SqlParameter idParam = new SqlParameter("@eventId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            SqlParameter nameParam = new SqlParameter("@name", item.Name);
            SqlParameter descriptionParam = new SqlParameter("@description", item.Description);
            SqlParameter layoutIdParam = new SqlParameter("@layoutId", item.LayoutId);
            SqlParameter startDateParam = new SqlParameter("@startDate", item.StartDate);
            SqlParameter endDateParam = new SqlParameter("@endDate", item.EndDate);
            SqlParameter publishedParam = new SqlParameter("@published", item.Published);

            await _context.Database.ExecuteSqlRawAsync(sqlCommand, nameParam, descriptionParam, layoutIdParam, startDateParam, endDateParam, publishedParam, idParam);

            await _context.SaveChangesAsync();

            int id = (int)idParam.Value;

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            string sqlCommand = "EXEC DeleteEvent @eventId={0}";
            SqlParameter idParam = new SqlParameter("@eventId", id);

            await _context.Database.ExecuteSqlRawAsync(sqlCommand, idParam);

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
            Event existingEvent = await GetByIdAsync(item.Id);

            if (existingEvent.LayoutId != item.LayoutId)
            {
                string sqlCommand = "EXEC UpdateEvent @eventId={0}, @name={1}, @description={2}, @layoutId={3}, @startDate={4}, @endDate={5}, @published={6}";

                SqlParameter idParam = new SqlParameter("@eventId", item.Id);
                SqlParameter nameParam = new SqlParameter("@name", item.Name);
                SqlParameter descriptionParam = new SqlParameter("@description", item.Description);
                SqlParameter layoutIdParam = new SqlParameter("@layoutId", item.LayoutId);
                SqlParameter startDateParam = new SqlParameter("@startDate", item.StartDate);
                SqlParameter endDateParam = new SqlParameter("@endDate", item.EndDate);
                SqlParameter publishedParam = new SqlParameter("@published", item.Published);

                await _context.Database.ExecuteSqlRawAsync(sqlCommand, idParam, nameParam, descriptionParam, layoutIdParam, startDateParam, endDateParam, publishedParam);
            }
            else
            {
                _context.Entry(existingEvent).State = EntityState.Detached;
                _context.Events.Update(item);
            }

            await _context.SaveChangesAsync();
        }
    }
}
