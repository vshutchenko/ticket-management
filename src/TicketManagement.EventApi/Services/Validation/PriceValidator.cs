using TicketManagement.Core.Validation;

namespace TicketManagement.EventApi.Services.Validation
{
    internal class PriceValidator : IValidator<decimal>
    {
        public void Validate(decimal item)
        {
            if (item < 0)
            {
                throw new ValidationException("Price is less than zero.");
            }
        }
    }
}
