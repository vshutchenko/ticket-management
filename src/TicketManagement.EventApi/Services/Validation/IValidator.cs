namespace TicketManagement.EventApi.Services.Validation
{
    public interface IValidator<T>
    {
        public void Validate(T item);
    }
}
