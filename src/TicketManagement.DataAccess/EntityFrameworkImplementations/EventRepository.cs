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
            var sqlCommand = "EXEC InsertEvent @name={0}, @description={1}, @layoutId={2}, @startDate={3}, @endDate={4}, @imageUrl={5}, @published={6}, @eventId={7} OUT";

            var idParam = new SqlParameter("@eventId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var nameParam = new SqlParameter("@name", item.Name);
            var descriptionParam = new SqlParameter("@description", item.Description);
            var layoutIdParam = new SqlParameter("@layoutId", item.LayoutId);
            var startDateParam = new SqlParameter("@startDate", item.StartDate);
            var endDateParam = new SqlParameter("@endDate", item.EndDate);
            var imageUrlParam = new SqlParameter("@imageUrl", item.ImageUrl);
            var publishedParam = new SqlParameter("@published", item.Published);

            await _context.Database.ExecuteSqlRawAsync(sqlCommand, nameParam, descriptionParam, layoutIdParam, startDateParam, endDateParam, imageUrlParam, publishedParam, idParam);

            await _context.SaveChangesAsync();

            var id = (int)idParam.Value;

            return id;
        }

        public async Task DeleteAsync(int id)
        {
            var sqlCommand = "EXEC DeleteEvent @eventId={0}";
            var idParam = new SqlParameter("@eventId", id);

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
            var existingEvent = await GetByIdAsync(item.Id);
            _context.Entry(existingEvent).State = EntityState.Detached;

            var idParam = new SqlParameter("@eventId", item.Id);
            var nameParam = new SqlParameter("@name", item.Name);
            var descriptionParam = new SqlParameter("@description", item.Description);
            var startDateParam = new SqlParameter("@startDate", item.StartDate);
            var endDateParam = new SqlParameter("@endDate", item.EndDate);
            var imageUrlParam = new SqlParameter("@imageUrl", item.ImageUrl);
            var publishedParam = new SqlParameter("@published", item.Published);

            if (existingEvent.LayoutId != item.LayoutId)
            {
                var sqlCommand = "EXEC UpdateEventWithAreas @eventId={0}, @name={1}, @description={2}, @layoutId={3}, @startDate={4}, @endDate={5}, @imageUrl={6}, @published={7}";

                var layoutIdParam = new SqlParameter("@layoutId", item.LayoutId);

                await _context.Database.ExecuteSqlRawAsync(sqlCommand, idParam, nameParam, descriptionParam, layoutIdParam, startDateParam, endDateParam, imageUrlParam, publishedParam);
            }
            else
            {
                var sqlCommand = "EXEC UpdateEvent @eventId={0}, @name={1}, @description={2}, @startDate={3}, @endDate={4}, @imageUrl={5}, @published={6}";

                await _context.Database.ExecuteSqlRawAsync(sqlCommand, idParam, nameParam, descriptionParam, startDateParam, endDateParam, imageUrlParam, publishedParam);
            }

            await _context.SaveChangesAsync();
        }
    }
}
