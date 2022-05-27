namespace TicketManagement.BusinessLogic.Validation
{
    public interface IValidator<T>
    {
        public void Validate(T item);
    }
}
