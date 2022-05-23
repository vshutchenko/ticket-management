namespace TicketManagement.BusinessLogic.Validation
{
    internal class PriceValidator : IValidator<decimal>
    {
        public bool Validate(decimal item)
        {
            if (item < 0)
            {
                throw new ValidationException("price cannotbe negative");
            }

            return true;
        }
    }
}
