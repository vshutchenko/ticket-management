namespace TicketManagement.WebApplication.Models
{
    public class PagingInfo
    {
        public PagingInfo(int totalItems, int itemsPerPage, int currentPage)
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        }

        public int TotalItems { get; }
        public int ItemsPerPage { get; }
        public int CurrentPage { get; }
        public int TotalPages { get; }
    }
}
