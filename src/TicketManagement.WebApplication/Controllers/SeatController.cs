using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Clients.VenueApi.Models;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [AuthorizeRoles(Roles.VenueManager)]
    public class SeatController : Controller
    {
        private readonly ISeatClient _seatClient;
        private readonly IAreaClient _areaClient;
        private readonly ITokenService _tokenService;

        public SeatController(
            ISeatClient seatClient,
            IAreaClient areaClient,
            ITokenService tokenService)
        {
            _seatClient = seatClient;
            _areaClient = areaClient;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult AddSeatRow(int areaId, int row)
        {
            return View(new SeatRowViewModel { AreaId = areaId, Row = row });
        }

        [HttpPost]
        public async Task<IActionResult> AddSeatRow(SeatRowViewModel model)
        {
            for (int i = 1; i <= model.Length; i++)
            {
                var seat = new SeatModel { AreaId = model.AreaId, Row = model.Row, Number = i };

                await _seatClient.CreateAsync(seat, _tokenService.GetToken());
            }

            var area = await _areaClient.GetByIdAsync(model.AreaId, _tokenService.GetToken());

            return RedirectToAction("AreaList", "Area", new { layoutId = area.LayoutId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeatRow(int areaId, int row)
        {
            var seats = await _seatClient.GetByAreaIdAsync(areaId, _tokenService.GetToken());

            var seatsToDelete = seats.Where(s => s.Row == row);

            foreach (var seat in seatsToDelete)
            {
                await _seatClient.DeleteAsync(seat.Id, _tokenService.GetToken());
            }

            var area = await _areaClient.GetByIdAsync(areaId, _tokenService.GetToken());

            return RedirectToAction("AreaList", "Area", new { layoutId = area.LayoutId });
        }
    }
}
