namespace TicketManagement.UserApi.Models
{
    public class PagedModel<TModel>
    {
        public PagedModel()
        {
            Items = new List<TModel>();
        }

        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IList<TModel> Items { get; set; }
    }
}
