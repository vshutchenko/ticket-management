using System;

namespace TicketManagement.BusinessLogic.Validation
{
    public class ValidationDetails
    {
        public ValidationDetails(string message, string itemName, string stringRepresentation)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            StringRepresentation = stringRepresentation ?? throw new ArgumentNullException(nameof(stringRepresentation));
        }

        public string Message { get; }

        public string ItemName { get; }

        public string StringRepresentation { get; }
    }
}
