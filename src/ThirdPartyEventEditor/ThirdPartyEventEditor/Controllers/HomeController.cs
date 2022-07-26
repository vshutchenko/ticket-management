using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using ThirdPartyEventEditor.Models;
using System.Text.Json;
using System.Configuration;
using ThirdPartyEventEditor.Extensions;
using System.Linq;

namespace ThirdPartyEventEditor.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _path;

        public HomeController()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["jsonFilePath"]);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var events = await GetEventsAsync();

            return View(events);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new ThirdPartyEventCreateModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(ThirdPartyEventCreateModel createModel)
        {
            var events = await GetEventsAsync();

            var @event = new ThirdPartyEvent
            {
                Id = events.Max(e => e.Id) + 1,
                Name = createModel.Name,
                Description = createModel.Description,
                StartDate = createModel.StartDate,
                EndDate = createModel.EndDate,
                PosterImage = await createModel.PosterImage.InputStream.GetBase64StringAsync(),
            };

            if (!IsValidEvent(@event))
            {
                return View(createModel);
            }

            events.Add(@event);

            await SaveEventsAsync(events);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var editModel = new ThirdPartyEventEditModel();

            var events = await GetEventsAsync();

            var eventToUpdate = events.FirstOrDefault(e => e.Id == id);

            if (eventToUpdate is null)
            {
                ModelState.AddModelError("", "Requested event was not found.");
                return View(editModel);
            }

            editModel.Id = eventToUpdate.Id;
            editModel.Name = eventToUpdate.Name;
            editModel.Description = eventToUpdate.Description;
            editModel.StartDate = eventToUpdate.StartDate;
            editModel.EndDate = eventToUpdate.EndDate;
            editModel.CurrentImage = eventToUpdate.PosterImage;

            return View(editModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ThirdPartyEventEditModel editModel)
        {
            var events = await GetEventsAsync();

            var eventToUpdate = events.FirstOrDefault(e => e.Id == editModel.Id);

            if (eventToUpdate is null)
            {
                ModelState.AddModelError("", "Requested event was not found.");
                return View(editModel);
            }

            eventToUpdate.Name = editModel.Name;
            eventToUpdate.Description = editModel.Description;
            eventToUpdate.StartDate = editModel.StartDate;
            eventToUpdate.EndDate = editModel.EndDate;

            if (editModel.NewPosterImage != null && editModel.NewPosterImage.ContentLength > 0)
            {
                eventToUpdate.PosterImage = await editModel.NewPosterImage.InputStream.GetBase64StringAsync();
            }
            else
            {
                eventToUpdate.PosterImage = editModel.CurrentImage;
            }

            if (!IsValidEvent(eventToUpdate))
            {
                return View(editModel);
            }

            await SaveEventsAsync(events);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var editModel = new ThirdPartyEventEditModel();

            var events = await GetEventsAsync();

            var eventToDelete = events.FirstOrDefault(e => e.Id == id);

            if (eventToDelete is null)
            {
                ModelState.AddModelError("", "Requested event was not found.");
                return View(editModel);
            }

            editModel.Id = eventToDelete.Id;
            editModel.Name = eventToDelete.Name;
            editModel.Description = eventToDelete.Description;
            editModel.StartDate = eventToDelete.StartDate;
            editModel.EndDate = eventToDelete.EndDate;
            editModel.CurrentImage = eventToDelete.PosterImage;

            return View(editModel);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var events = await GetEventsAsync();

            var eventToDelete = events.FirstOrDefault(e => e.Id == id);

            if (eventToDelete is null)
            {
                ModelState.AddModelError("", "Requested event was not found.");
                return RedirectToAction("Index");
            }

            events.RemoveAll(e => e.Id == id);

            await SaveEventsAsync(events);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public FileResult DownloadFile()
        {
            return File(_path, "application/json", "events.json");
        }

        private async Task<List<ThirdPartyEvent>> GetEventsAsync()
        {
            using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fileStream);
                }
                catch (JsonException)
                {
                    return new List<ThirdPartyEvent>();
                }
            }
        }
        private async Task SaveEventsAsync(List<ThirdPartyEvent> events)
        {
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                fs.SetLength(0);
                await JsonSerializer.SerializeAsync(fs, events);
            }
        }

        private bool IsValidEvent(ThirdPartyEvent @event)
        {
            if (@event.StartDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Start date is in the past.");
            }

            if (@event.EndDate < @event.StartDate)
            {
                ModelState.AddModelError("", "End date is less than start date.");
            }

            return ModelState.IsValid;
        }
    }
}