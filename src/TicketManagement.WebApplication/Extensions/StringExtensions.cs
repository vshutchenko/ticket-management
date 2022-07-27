namespace TicketManagement.WebApplication.Extensions
{
    public static class StringExtensions
    {
        public static string GetAbsoluteUrl(this string url, string host)
        {
            var a = url.StartsWith("https://")
                ? url
                : $"https://{host}/{url}";

            return a;
        }
    }
}
