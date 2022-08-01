using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Extensions;
using ThirdPartyEventEditor.Services.Interfaces;
using System.Text.Json;

namespace ThirdPartyEventEditor.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventStorage _storage;

        public HomeController(IEventStorage storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var events = await _storage.GetAllAsync();

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
            var @event = new ThirdPartyEvent
            {
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

            await _storage.CreateAsync(@event);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(Guid id)
        {
            var editModel = new ThirdPartyEventEditModel();

            var eventToUpdate = await _storage.GetByIdAsync(id);

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
            var eventToUpdate = await _storage.GetByIdAsync(editModel.Id);

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

            await _storage.UpdateAsync(eventToUpdate);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            var editModel = new ThirdPartyEventEditModel();

            var eventToDelete = await _storage.GetByIdAsync(id);

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
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            var eventToDelete = await _storage.GetByIdAsync(id);

            if (eventToDelete is null)
            {
                ModelState.AddModelError("", "Requested event was not found.");
                return RedirectToAction("Index");
            }

            await _storage.DeleteAsync(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<FileResult> DownloadFile()
        {
            var events = await _storage.GetAllAsync();

            var bytes = JsonSerializer.SerializeToUtf8Bytes(events);

            return File(bytes, "application/json", "events.json");
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