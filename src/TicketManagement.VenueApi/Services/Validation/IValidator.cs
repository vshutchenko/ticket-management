namespace TicketManagement.VenueApi.Services.Validation
{
    public interface IValidator<T>
    {
        public void Validate(T item);
    }
}
