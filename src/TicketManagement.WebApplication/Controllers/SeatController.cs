using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Clients.VenueApi;
using TicketManagement.Core.Clients.VenueApi.Models;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [AuthorizeRoles(Roles.VenueManager)]
    public class SeatController : BaseController
    {
        private readonly ISeatClient _seatClient;
        private readonly IAreaClient _areaClient;

        public SeatController(
            ISeatClient seatClient,
            IAreaClient areaClient,
            ITokenService tokenService)
            : base(tokenService)
        {
            _seatClient = seatClient ?? throw new ArgumentNullException(nameof(seatClient));
            _areaClient = areaClient ?? throw new ArgumentNullException(nameof(areaClient));
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

                await _seatClient.CreateAsync(seat, TokenService.GetToken());
            }

            var area = await _areaClient.GetByIdAsync(model.AreaId, TokenService.GetToken());

            return RedirectToAction(
                nameof(AreaController.AreaList),
                TrimController(nameof(AreaController)),
                new { layoutId = area.LayoutId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeatRow(int areaId, int row)
        {
            var seats = await _areaClient.GetSeatsByAreaIdAsync(areaId, TokenService.GetToken());

            var seatsToDelete = seats.Where(s => s.Row == row);

            foreach (var seat in seatsToDelete)
            {
                await _seatClient.DeleteAsync(seat.Id, TokenService.GetToken());
            }

            var area = await _areaClient.GetByIdAsync(areaId, TokenService.GetToken());

            return RedirectToAction(
                nameof(AreaController.AreaList),
                TrimController(nameof(AreaController)),
                new { layoutId = area.LayoutId });
        }
    }
}
