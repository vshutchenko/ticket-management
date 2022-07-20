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
            var events = new List<ThirdPartyEvent>();

            using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                events.AddRange(await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fileStream));
            }

            return View(events);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new ThirdPartyEventInputModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(ThirdPartyEventInputModel inputModel)
        {
            if (!IsValidInputModel(inputModel))
            {
                return View(new ThirdPartyEventInputModel());
            }

            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fs);

                var @event = new ThirdPartyEvent
                {
                    Id = events.GenerateId(),
                    Name = inputModel.Name,
                    Description = inputModel.Description,
                    StartDate = inputModel.StartDate,
                    EndDate = inputModel.EndDate,
                    PosterImage = await inputModel.PosterImage.InputStream.GetBase64StringAsync(),
                };

                events.Add(@event);

                fs.SetLength(0);

                await JsonSerializer.SerializeAsync(fs, events);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var inputModel = new ThirdPartyEventInputModel();

            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fs);

                var eventToUpdate = events.FirstOrDefault(e => e.Id == id);

                if (eventToUpdate is null)
                {
                    ModelState.AddModelError("", "Requested event was not found.");
                    return View(inputModel);
                }

                inputModel.Id = eventToUpdate.Id;
                inputModel.Name = eventToUpdate.Name;
                inputModel.Description = eventToUpdate.Description;
                inputModel.StartDate = eventToUpdate.StartDate;
                inputModel.EndDate = eventToUpdate.EndDate;
                inputModel.CurrentImage = eventToUpdate.PosterImage;
            }

            return View(inputModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ThirdPartyEventInputModel inputModel)
        {
            if (!IsValidInputModel(inputModel))
            {
                return View(new ThirdPartyEventInputModel());
            }

            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fs);

                var eventToUpdate = events.FirstOrDefault(e => e.Id == inputModel.Id);

                if (eventToUpdate is null)
                {
                    ModelState.AddModelError("", "Requested event was not found.");
                    return View(inputModel);
                }

                eventToUpdate.Name = inputModel.Name;
                eventToUpdate.Description = inputModel.Description;
                eventToUpdate.StartDate = inputModel.StartDate;
                eventToUpdate.EndDate = inputModel.EndDate;
                eventToUpdate.PosterImage = await inputModel.PosterImage.InputStream.GetBase64StringAsync();

                fs.SetLength(0);

                await JsonSerializer.SerializeAsync(fs, events);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var inputModel = new ThirdPartyEventInputModel();

            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fs);

                var eventToDelete = events.FirstOrDefault(e => e.Id == id);

                if (eventToDelete is null)
                {
                    ModelState.AddModelError("", "Requested event was not found.");
                    return View(inputModel);
                }

                inputModel.Id = eventToDelete.Id;
                inputModel.Name = eventToDelete.Name;
                inputModel.Description = eventToDelete.Description;
                inputModel.StartDate = eventToDelete.StartDate;
                inputModel.EndDate = eventToDelete.EndDate;
                inputModel.CurrentImage = eventToDelete.PosterImage;
            }

            return View(inputModel);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(ThirdPartyEventInputModel inputModel)
        {
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var events = await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fs);

                var eventToDelete = events.FirstOrDefault(e => e.Id == inputModel.Id);

                if (eventToDelete is null)
                {
                    ModelState.AddModelError("", "Requested event was not found.");
                    return View("Index");
                }

                events.RemoveAll(e => e.Id == inputModel.Id);

                fs.SetLength(0);

                await JsonSerializer.SerializeAsync(fs, events);
            }

            return View("Index");
        }

        private bool IsValidInputModel(ThirdPartyEventInputModel model)
        {
            if (model.StartDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Start date is in the past.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("", "End date is less than start date.");
            }

            return ModelState.IsValid;
        }
    }
}