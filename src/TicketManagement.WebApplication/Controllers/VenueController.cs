using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Clients.VenueApi.Models;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [Authorize(Roles = "Venue manager")]
    public class VenueController : Controller
    {
        private readonly ILayoutClient _layoutClient;
        private readonly IVenueClient _venueClient;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public VenueController(
            ILayoutClient layoutClient,
            IVenueClient venueClient,
            ITokenService tokenService,
            IMapper mapper)
        {
            _layoutClient = layoutClient;
            _venueClient = venueClient;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> VenueList()
        {
            var venues = await _venueClient.GetAllAsync(_tokenService.GetToken());

            var venuesVM = new List<VenueViewModel>();

            foreach (var venue in venues)
            {
                var layouts = await _layoutClient.GetByVenueIdAsync(venue.Id, _tokenService.GetToken());

                var layoutsVM = layouts.Select(l => _mapper.Map<LayoutViewModel>(l)).ToList();

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

            await _venueClient.CreateAsync(venue, _tokenService.GetToken());

            return RedirectToAction("VenueList");
        }

        [HttpGet]
        public async Task<IActionResult> EditVenue(int id)
        {
            var venue = await _venueClient.GetByIdAsync(id, _tokenService.GetToken());

            var model = _mapper.Map<VenueViewModel>(venue);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditVenue(VenueViewModel model)
        {
            var venue = _mapper.Map<VenueModel>(model);

            await _venueClient.UpdateAsync(venue, _tokenService.GetToken());

            return RedirectToAction("VenueList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVenue(int id)
        {
            await _venueClient.DeleteAsync(id, _tokenService.GetToken());

            return RedirectToAction("VenueList");
        }
    }
}
