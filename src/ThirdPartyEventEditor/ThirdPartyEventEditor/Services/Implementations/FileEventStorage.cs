using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Services.Interfaces;

[assembly: InternalsVisibleTo("ThirdPartyEventEditor.IntegrationTests")]

namespace ThirdPartyEventEditor.Services.Implementations
{
    internal sealed class FileEventStorage : IEventStorage
    {
        private readonly string _path;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public FileEventStorage()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["jsonFilePath"]);
        }

        public async Task CreateAsync(ThirdPartyEvent @event)
        {
            await _semaphore.WaitAsync();

            try
            {
                var events = await LoadEventsAsync();

                @event.Id = Guid.NewGuid().ToString();

                events.Add(@event);

                await SaveEventsAsync(events);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task UpdateAsync(ThirdPartyEvent @event)
        {
            await _semaphore.WaitAsync();

            try
            {
                var events = await LoadEventsAsync();

                var eventToUpdate = events.First(e => e.Id == @event.Id);

                eventToUpdate.Name = @event.Name;
                eventToUpdate.Description = @event.Description;
                eventToUpdate.StartDate = @event.StartDate;
                eventToUpdate.EndDate = @event.EndDate;
                eventToUpdate.PosterImage = @event.PosterImage;

                await SaveEventsAsync(events);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task DeleteAsync(string id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var events = await LoadEventsAsync();

                events.RemoveAll(e => e.Id == id);

                await SaveEventsAsync(events);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<List<ThirdPartyEvent>> GetAllAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                return await LoadEventsAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<ThirdPartyEvent> GetByIdAsync(string id)
        {
            await _semaphore.WaitAsync();

            try
            {
                var events = await LoadEventsAsync();

                return events.First(e => e.Id == id);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<ThirdPartyEvent>> LoadEventsAsync()
        {
            try
            {
                using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    return new List<ThirdPartyEvent>(await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fileStream));
                }
            }
            catch (JsonException)
            {
                return new List<ThirdPartyEvent>();
            }
        }

        private async Task SaveEventsAsync(List<ThirdPartyEvent> events)
        {
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.SetLength(0);
                await JsonSerializer.SerializeAsync(fs, events);
            }
        }
    }
}