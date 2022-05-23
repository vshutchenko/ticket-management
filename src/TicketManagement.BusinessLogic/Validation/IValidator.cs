namespace TicketManagement.BusinessLogic.Validation
{
    public interface IValidator<T>
    {
        public bool Validate(T item);
    }
}
