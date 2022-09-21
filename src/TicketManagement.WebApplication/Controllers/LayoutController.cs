using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Core.Clients.VenueApi;
using TicketManagement.Core.Clients.VenueApi.Models;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [AuthorizeRoles(Roles.VenueManager)]
    public class LayoutController : BaseController
    {
        private readonly ILayoutClient _layoutClient;
        private readonly IMapper _mapper;

        public LayoutController(
            ILayoutClient layoutClient,
            ITokenService tokenService,
            IMapper mapper)
            : base(tokenService)
        {
            _layoutClient = layoutClient ?? throw new ArgumentNullException(nameof(layoutClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            await _layoutClient.CreateAsync(layout, TokenService.GetToken());

            return RedirectToAction(nameof(VenueController.VenueList), TrimController(nameof(VenueController)));
        }

        [HttpGet]
        public async Task<IActionResult> EditLayout(int id)
        {
            var layout = await _layoutClient.GetByIdAsync(id, TokenService.GetToken());

            var model = _mapper.Map<LayoutViewModel>(layout);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditLayout(LayoutViewModel model)
        {
            var layout = _mapper.Map<LayoutModel>(model);

            await _layoutClient.UpdateAsync(layout, TokenService.GetToken());

            return RedirectToAction(nameof(VenueController.VenueList), TrimController(nameof(VenueController)));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLayout(int id)
        {
            await _layoutClient.DeleteAsync(id, TokenService.GetToken());

            return RedirectToAction(nameof(VenueController.VenueList), TrimController(nameof(VenueController)));
        }
    }
}
