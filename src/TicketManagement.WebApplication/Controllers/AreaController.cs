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
    public class AreaController : Controller
    {
        private readonly IAreaClient _areaClient;
        private readonly ISeatClient _seatClient;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AreaController(
            IAreaClient areaClient,
            ISeatClient seatClient,
            ITokenService tokenService,
            IMapper mapper)
        {
            _areaClient = areaClient;
            _seatClient = seatClient;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> AreaList(int layoutId)
        {
            var areas = await _areaClient.GetByLayoutIdAsync(layoutId, _tokenService.GetToken());

            var areasVM = new List<AreaViewModel>();

            foreach (var area in areas)
            {
                var seats = await _seatClient.GetByAreaIdAsync(area.Id, _tokenService.GetToken());

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

            await _areaClient.CreateAsync(area, _tokenService.GetToken());

            return RedirectToAction("AreaList", new { layoutId = area.LayoutId });
        }

        [HttpGet]
        public async Task<IActionResult> EditArea(int id)
        {
            var area = await _areaClient.GetByIdAsync(id, _tokenService.GetToken());

            var model = _mapper.Map<AreaViewModel>(area);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditArea(AreaViewModel model)
        {
            var area = _mapper.Map<AreaModel>(model);

            await _areaClient.UpdateAsync(area, _tokenService.GetToken());

            return RedirectToAction("AreaList", new { layoutId = area.LayoutId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var area = await _areaClient.GetByIdAsync(id, _tokenService.GetToken());

            await _areaClient.DeleteAsync(id, _tokenService.GetToken());

            return RedirectToAction("AreaList", new { layoutId = area.LayoutId });
        }
    }
}
