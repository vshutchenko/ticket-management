using System.Collections.Generic;

namespace TicketManagement.BusinessLogic.Validation
{
    public interface IValidationService<T>
    {
        public IEnumerable<ValidationDetails> Details { get; }
        public bool Validate(T item);
    }
}
