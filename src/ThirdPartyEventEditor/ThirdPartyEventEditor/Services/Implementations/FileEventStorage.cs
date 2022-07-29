using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Services.Interfaces;

namespace ThirdPartyEventEditor.Services.Implementations
{
    internal sealed class FileEventStorage : IEventStorage
    {
        private readonly string _path;
        private static readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim _readSemaphore = new SemaphoreSlim(1, 1);

        public FileEventStorage()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["jsonFilePath"]);
        }

        public async Task<List<ThirdPartyEvent>> GetEventsAsync()
        {
            var events = new List<ThirdPartyEvent>();

            await _readSemaphore.WaitAsync();

            try
            {
                using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    events.AddRange(await JsonSerializer.DeserializeAsync<List<ThirdPartyEvent>>(fileStream));
                }
            }
            finally
            {
                _readSemaphore.Release();
            }

            return events;
        }

        public async Task SaveEventsAsync(List<ThirdPartyEvent> events)
        {
            await _writeSemaphore.WaitAsync();

            try
            {
                using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    fs.SetLength(0);
                    await JsonSerializer.SerializeAsync(fs, events);
                }
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }
    }
}