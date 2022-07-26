using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;
using ThirdPartyEventEditor.Services.Interfaces;

namespace ThirdPartyEventEditor.Services.Implementations
{
    internal sealed class FileEventStorage : IEventStorage
    {
        private readonly string _path;

        public FileEventStorage()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["jsonFilePath"]);
        }

        public async Task<List<ThirdPartyEvent>> GetEventsAsync()
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

        public async Task SaveEventsAsync(List<ThirdPartyEvent> events)
        {
            using (var fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                fs.SetLength(0);
                await JsonSerializer.SerializeAsync(fs, events);
            }
        }
    }
}