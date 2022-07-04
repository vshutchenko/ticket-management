namespace TicketManagement.WebApplication.Models.Layout
{
    public class LayoutListViewModel
    {
        public LayoutListViewModel(IEnumerable<LayoutViewModel> layouts, PagingInfo pagingInfo)
        {
            Layouts = layouts ?? throw new ArgumentNullException(nameof(layouts));
            PagingInfo = pagingInfo ?? throw new ArgumentNullException(nameof(pagingInfo));
        }

        public IEnumerable<LayoutViewModel> Layouts { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
