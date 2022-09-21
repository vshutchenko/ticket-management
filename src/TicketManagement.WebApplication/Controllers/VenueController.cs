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
    public class VenueController : BaseController
    {
        private readonly IVenueClient _venueClient;
        private readonly IMapper _mapper;

        public VenueController(
            IVenueClient venueClient,
            ITokenService tokenService,
            IMapper mapper)
            : base(tokenService)
        {
            _venueClient = venueClient ?? throw new ArgumentNullException(nameof(venueClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> VenueList()
        {
            var venues = await _venueClient.GetAllAsync(TokenService.GetToken());

            var venuesVM = new List<VenueViewModel>();

            foreach (var venue in venues)
            {
                var layouts = await _venueClient.GetLayoutsByVenueIdAsync(venue.Id, TokenService.GetToken());

                var layoutsVM = layouts.Select(layout => _mapper.Map<LayoutViewModel>(layout)).ToList();

                var venueVM = venue.CreateVM(layoutsVM);

                venuesVM.Add(venueVM);
            }

            return View(venuesVM);
        }

        [HttpGet]
        public IActionResult CreateVenue()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenue(VenueViewModel model)
        {
            var venue = _mapper.Map<VenueModel>(model);

            await _venueClient.CreateAsync(venue, TokenService.GetToken());

            return RedirectToAction(nameof(VenueList));
        }

        [HttpGet]
        public async Task<IActionResult> EditVenue(int id)
        {
            var venue = await _venueClient.GetByIdAsync(id, TokenService.GetToken());

            var model = _mapper.Map<VenueViewModel>(venue);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditVenue(VenueViewModel model)
        {
            var venue = _mapper.Map<VenueModel>(model);

            await _venueClient.UpdateAsync(venue, TokenService.GetToken());

            return RedirectToAction(nameof(VenueList));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            await _venueClient.DeleteAsync(id, TokenService.GetToken());

            return RedirectToAction(nameof(VenueList));
        }
    }
}
