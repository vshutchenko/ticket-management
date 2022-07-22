using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.WebApplication.Models.EventImport;
using TicketManagement.WebApplication.Models.Layout;
using TicketManagement.WebApplication.Models.Venue;

namespace TicketManagement.WebApplication.Controllers
{
    [Authorize(Roles = "Event manager")]
    public class EventImportController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IWebHostEnvironment _enviroment;
        private readonly IMapper _mapper;

        public EventImportController(IEventService eventService, IWebHostEnvironment enviroment, IMapper mapper)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _enviroment = enviroment ?? throw new ArgumentNullException(nameof(enviroment));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult ImportEvents([FromServices] IVenueService venueService, [FromServices] ILayoutService layoutService)
        {
            var venues = venueService.GetAll()
                .Select(v => _mapper.Map<VenueViewModel>(v))
                .ToList();

            var layouts = layoutService.GetAll()
                .Where(l => l.VenueId == venues.First().Id)
                .Select(l => _mapper.Map<LayoutViewModel>(l))
                .ToList();

            var model = new ImportViewModel
            {
                Layouts = new SelectList(layouts, "Id", "Description"),
                Venues = new SelectList(venues, "Id", "Description"),
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
                    await _eventService.CreateAsync(eventToCreate);
                }
                catch (ValidationException ex)
                {
                    invalidEvents.Add(eventToCreate.Name, ex.Message);
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
