using System.Globalization;

namespace TicketManagement.WebApplication.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatPrice(this decimal price)
        {
            var nfi = (NumberFormatInfo)Thread.CurrentThread.CurrentUICulture.NumberFormat.Clone();
            nfi.CurrencySymbol = "$";

            return string.Format(nfi, "{0:c}", price);
        }
    }
}
