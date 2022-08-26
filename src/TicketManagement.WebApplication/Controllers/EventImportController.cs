using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestEase;
using TicketManagement.WebApplication.Clients.CommonModels;
using TicketManagement.WebApplication.Clients.EventApi;
using TicketManagement.WebApplication.Clients.EventApi.Models;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Models.EventImport;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [Authorize(Roles = "Event manager")]
    public class EventImportController : Controller
    {
        private readonly IEventClient _eventClient;
        private readonly ITokenService _tokenService;
        private readonly IWebHostEnvironment _enviroment;
        private readonly IMapper _mapper;

        public EventImportController(IEventClient eventClient, ITokenService tokenService, IWebHostEnvironment enviroment, IMapper mapper)
        {
            _eventClient = eventClient ?? throw new ArgumentNullException(nameof(eventClient));
            _enviroment = enviroment ?? throw new ArgumentNullException(nameof(enviroment));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> ImportEvents([FromServices] IVenueClient venueClient, [FromServices] ILayoutClient layoutClient)
        {
            var venues = await venueClient.GetAllAsync(_tokenService.GetToken());
            var layouts = await layoutClient.GetAllAsync(_tokenService.GetToken());

            var venuesVM = venues
                .Select(v => _mapper.Map<VenueViewModel>(v))
                .ToList();

            var layoutsVM = layouts
                .Where(l => l.VenueId == venues.First().Id)
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            var model = new ImportViewModel
            {
                Layouts = new SelectList(layoutsVM, "Id", "Description"),
                Venues = new SelectList(venuesVM, "Id", "Description"),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportEvents(ImportViewModel importModel)
        {
            var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEventModel>>(importModel.EventsJson!.OpenReadStream());

            var invalidEvents = new Dictionary<string, string>();

            foreach (var e in events!)
            {
                byte[] imgBytes = Convert.FromBase64String(e.PosterImage);

                var imageUrl = Path.Combine("EventImages/", $"{Guid.NewGuid()}.png");

                var eventToCreate = new EventModel
                {
                    Name = e.Name,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    LayoutId = int.Parse(importModel.Layout),
                    Published = false,
                    ImageUrl = imageUrl,
                };

                try
                {
                    await _eventClient.CreateAsync(eventToCreate, _tokenService.GetToken());
                }
                catch (ApiException ex)
                {
                    invalidEvents.Add(eventToCreate.Name, ex.DeserializeContent<ErrorModel>().Error!);
                }

                var physicalPath = Path.Combine(_enviroment.WebRootPath, imageUrl);

                using var imageFile = new FileStream(physicalPath, FileMode.Create);

                imageFile.Write(imgBytes, 0, imgBytes.Length);
                imageFile.Flush();
            }

            return View("ImportDetails", invalidEvents);
        }
    }
}