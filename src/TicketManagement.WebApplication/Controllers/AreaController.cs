using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Clients.VenueApi.Models;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [AuthorizeRoles(Roles.VenueManager)]
    public class AreaController : BaseController
    {
        private readonly IAreaClient _areaClient;
        private readonly ISeatClient _seatClient;
        private readonly IMapper _mapper;

        public AreaController(
            IAreaClient areaClient,
            ISeatClient seatClient,
            ITokenService tokenService,
            IMapper mapper)
            : base(tokenService)
        {
            _areaClient = areaClient ?? throw new ArgumentNullException(nameof(areaClient));
            _seatClient = seatClient ?? throw new ArgumentNullException(nameof(seatClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> AreaList(int layoutId)
        {
            var areas = await _areaClient.GetByLayoutIdAsync(layoutId, TokenService.GetToken());

            var areasVM = new List<AreaViewModel>();

            foreach (var area in areas)
            {
                var seats = await _seatClient.GetByAreaIdAsync(area.Id, TokenService.GetToken());

                var seatsVM = seats.Select(a => _mapper.Map<SeatViewModel>(a)).ToList();

                var areaVM = area.CreateVM(seatsVM);

                areasVM.Add(areaVM);
            }

            ViewBag.LayoutId = layoutId;

            return View(areasVM);
        }

        [HttpGet]
        public IActionResult CreateArea(int layoutId)
        {
            ViewBag.LayoutId = layoutId;

            return View(new AreaViewModel { LayoutId = layoutId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea(AreaViewModel model)
        {
            var area = _mapper.Map<AreaModel>(model);

            await _areaClient.CreateAsync(area, TokenService.GetToken());

            return RedirectToAction(nameof(AreaList), new { layoutId = area.LayoutId });
        }

        [HttpGet]
        public async Task<IActionResult> EditArea(int id)
        {
            var area = await _areaClient.GetByIdAsync(id, TokenService.GetToken());

            var model = _mapper.Map<AreaViewModel>(area);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditArea(AreaViewModel model)
        {
            var area = _mapper.Map<AreaModel>(model);

            await _areaClient.UpdateAsync(area, TokenService.GetToken());

            return RedirectToAction(nameof(AreaList), new { layoutId = area.LayoutId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var area = await _areaClient.GetByIdAsync(id, TokenService.GetToken());

            await _areaClient.DeleteAsync(id, TokenService.GetToken());

            return RedirectToAction(nameof(AreaList), new { layoutId = area.LayoutId });
        }
    }
}
