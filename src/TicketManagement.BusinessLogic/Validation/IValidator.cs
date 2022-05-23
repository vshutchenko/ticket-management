namespace TicketManagement.BusinessLogic.Validation
{
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates object.
        /// </summary>
        /// <param name="item">Object to validate.</param>
        /// <exception cref="ValidationException">Throws if object is invalid.</exception>
        public void Validate(T item);
    }
}
