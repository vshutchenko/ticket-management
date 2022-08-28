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
    public class LayoutController : Controller
    {
        private readonly ILayoutClient _layoutClient;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public LayoutController(
            ILayoutClient layoutClient,
            ITokenService tokenService,
            IMapper mapper)
        {
            _layoutClient = layoutClient;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult CreateLayout(int venueId)
        {
            return View(new LayoutViewModel { VenueId = venueId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateLayout(LayoutViewModel model)
        {
            var layout = _mapper.Map<LayoutModel>(model);

            await _layoutClient.CreateAsync(layout, _tokenService.GetToken());

            return RedirectToAction("VenueList", "Venue");
        }

        [HttpGet]
        public async Task<IActionResult> EditLayout(int id)
        {
            var layout = await _layoutClient.GetByIdAsync(id, _tokenService.GetToken());

            var model = _mapper.Map<LayoutViewModel>(layout);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLayout(LayoutViewModel model)
        {
            var layout = _mapper.Map<LayoutModel>(model);

            await _layoutClient.UpdateAsync(layout, _tokenService.GetToken());

            return RedirectToAction("VenueList", "Venue");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLayout(int id)
        {
            await _layoutClient.DeleteAsync(id, _tokenService.GetToken());

            return RedirectToAction("VenueList", "Venue");
        }
    }
}
