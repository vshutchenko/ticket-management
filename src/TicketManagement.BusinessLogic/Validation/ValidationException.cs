using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TicketManagement.BusinessLogic.Validation
{
    [Serializable]
    public class ValidationException : Exception
    {
        private readonly List<string> _errors;

        public ValidationException()
        {
            _errors = new List<string>();
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
