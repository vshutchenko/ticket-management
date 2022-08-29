namespace TicketManagement.Core.Validation
{
    public interface IValidator<T>
    {
        public void Validate(T item);
    }
}
