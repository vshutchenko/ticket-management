using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Extensions
{
    public static class RepositoryExtensions
    {
        public static void CheckIfIdExists<T>(this IRepository<T> repository, int id)
        {
            var entity = repository.GetById(id);

            if (entity is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }
    }
}
