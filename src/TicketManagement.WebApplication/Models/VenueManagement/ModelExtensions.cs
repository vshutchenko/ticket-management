using TicketManagement.WebApplication.Clients.VenueApi.Models;

namespace TicketManagement.WebApplication.Models.VenueManagement
{
    public static class ModelExtensions
    {
        public static VenueViewModel CreateVM(this VenueModel venue, List<LayoutViewModel> layouts)
        {
            return new VenueViewModel
            {
                Id = venue.Id,
                Address = venue.Address,
                Description = venue.Description,
                Phone = venue.Phone,
                Layouts = layouts,
            };
        }

        public static AreaViewModel CreateVM(this AreaModel area, List<SeatViewModel> seats)
        {
            return new AreaViewModel
            {
                Id = area.Id,
                CoordX = area.CoordX,
                CoordY = area.CoordY,
                Description = area.Description,
                LayoutId = area.LayoutId,
                Seats = seats,
            };
        }
    }
}
