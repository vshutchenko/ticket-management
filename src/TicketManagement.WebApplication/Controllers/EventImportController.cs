using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestEase;
using TicketManagement.Core.Clients.CommonModels;
using TicketManagement.Core.Clients.EventApi;
using TicketManagement.Core.Clients.EventApi.Models;
using TicketManagement.Core.Clients.VenueApi;
using TicketManagement.Core.Models;
using TicketManagement.WebApplication.Models.EventImport;
using TicketManagement.WebApplication.Models.VenueManagement;
using TicketManagement.WebApplication.Services;

namespace TicketManagement.WebApplication.Controllers
{
    [AuthorizeRoles(Roles.EventManager)]
    public class EventImportController : BaseController
    {
        private readonly IEventClient _eventClient;
        private readonly IWebHostEnvironment _enviroment;
        private readonly IMapper _mapper;

        public EventImportController(
            IEventClient eventClient,
            ITokenService tokenService,
            IWebHostEnvironment enviroment,
            IMapper mapper)
            : base(tokenService)
        {
            _eventClient = eventClient ?? throw new ArgumentNullException(nameof(eventClient));
            _enviroment = enviroment ?? throw new ArgumentNullException(nameof(enviroment));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> ImportEvents([FromServices] IVenueClient venueClient, [FromServices] ILayoutClient layoutClient)
        {
            var venues = await venueClient.GetAllAsync(TokenService.GetToken());
            var layouts = await layoutClient.GetAllAsync(TokenService.GetToken());

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
        public async Task<IActionResult> ImportEvents(ImportViewModel importModel)
        {
            var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEventModel>>(importModel.EventsJson!.OpenReadStream());

            var invalidEvents = new Dictionary<string, string>();

            var imageFolderPath = Path.Combine(_enviroment.WebRootPath, "EventImages");

            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }

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
                    await _eventClient.CreateAsync(eventToCreate, TokenService.GetToken());
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