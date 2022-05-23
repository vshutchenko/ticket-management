using System;

namespace TicketManagement.BusinessLogic.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool InRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }
    }
}
