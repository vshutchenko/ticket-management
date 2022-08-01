using NUnit.Framework;
using System.Configuration;
using ThirdPartyEventEditor.Services.Interfaces;
using ThirdPartyEventEditor.Services.Implementations;
using System.Threading.Tasks;
using ThirdPartyEventEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;

namespace ThirdPartyEventEditor.IntegrationTests
{
    public class FileEventStorageTest
    {
        private IEventStorage _storage;

        [SetUp]
        public void SetUp()
        {
            var initialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\initialEvents.json");
            
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\events.json");

            var initialJson = File.ReadAllText(initialPath);

            File.WriteAllText(path, initialJson);

            ConfigurationManager.AppSettings["jsonFilePath"] = path;

            _storage = new FileEventStorage();
        }

        [Test]
        public async Task Create_MultipleThreads_CreatesEvents()
        {
            // Arrange
            var expectedEventsBeforeCreate = new List<ThirdPartyEvent>()
            {
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("403dfbd3-a4ac-4d41-b495-59907c7079eb"),
                    Name = "First event",
                    Description = "Description 1",
                    StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2023, 1, 1, 12, 0, 0),
                    PosterImage = "Image 1",
                },
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("48be7ed9-acc2-4723-9ba2-deb2b62747cd"),
                    Name = "Second event",
                    Description = "Description 2",
                    StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2024, 1, 1, 12, 0, 0),
                    PosterImage = "Image 2",
                }
            };

            var actualEventsBeforeCreate = await _storage.GetAllAsync();

            var eventsToCreate = new List<ThirdPartyEvent>()
            {
                new ThirdPartyEvent
                {
                    Name = "Name 1",
                    Description = "Description 1",
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(3),
                    PosterImage = "Image 1",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 2",
                    Description = "Description 2",
                    StartDate = DateTime.Now.AddDays(4),
                    EndDate = DateTime.Now.AddDays(5),
                    PosterImage = "Image 2",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 3",
                    Description = "Description 3",
                    StartDate = DateTime.Now.AddDays(7),
                    EndDate = DateTime.Now.AddDays(8),
                    PosterImage = "Image 3",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 4",
                    Description = "Description 4",
                    StartDate = DateTime.Now.AddDays(9),
                    EndDate = DateTime.Now.AddDays(10),
                    PosterImage = "Image 4",
                }
            };

            var expectedEvents = new List<ThirdPartyEvent>()
            {
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("403dfbd3-a4ac-4d41-b495-59907c7079eb"),
                    Name = "First event",
                    Description = "Description 1",
                    StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2023, 1, 1, 12, 0, 0),
                    PosterImage = "Image 1",
                },
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("48be7ed9-acc2-4723-9ba2-deb2b62747cd"),
                    Name = "Second event",
                    Description = "Description 2",
                    StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2024, 1, 1, 12, 0, 0),
                    PosterImage = "Image 2",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 1",
                    Description = "Description 1",
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = DateTime.Now.AddDays(3),
                    PosterImage = "Image 1",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 2",
                    Description = "Description 2",
                    StartDate = DateTime.Now.AddDays(4),
                    EndDate = DateTime.Now.AddDays(5),
                    PosterImage = "Image 2",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 3",
                    Description = "Description 3",
                    StartDate = DateTime.Now.AddDays(7),
                    EndDate = DateTime.Now.AddDays(8),
                    PosterImage = "Image 3",
                },
                new ThirdPartyEvent
                {
                    Name = "Name 4",
                    Description = "Description 4",
                    StartDate = DateTime.Now.AddDays(9),
                    EndDate = DateTime.Now.AddDays(10),
                    PosterImage = "Image 4",
                }
            };

            Task[] tasks = new Task[eventsToCreate.Count];

            // Act
            for (int i = 0; i < eventsToCreate.Count; i++)
            {
                var index = i;
                tasks[index] = Task.Run(() => _storage.CreateAsync(eventsToCreate[index]));
            }

            Task.WaitAll(tasks);

            var actualEvents = await _storage.GetAllAsync();

            // Assert
            actualEventsBeforeCreate.Should().BeEquivalentTo(expectedEventsBeforeCreate);
            actualEvents.Should().BeEquivalentTo(expectedEvents, opt => opt.Excluding(e => e.Id));
        }

        [Test]
        public async Task Delete_MultipleThreads_DeletesEvents()
        {
            // Arrange
            var expectedEventsBeforeUpdate = new List<ThirdPartyEvent>()
            {
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("403dfbd3-a4ac-4d41-b495-59907c7079eb"),
                    Name = "First event",
                    Description = "Description 1",
                    StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2023, 1, 1, 12, 0, 0),
                    PosterImage = "Image 1",
                },
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("48be7ed9-acc2-4723-9ba2-deb2b62747cd"),
                    Name = "Second event",
                    Description = "Description 2",
                    StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2024, 1, 1, 12, 0, 0),
                    PosterImage = "Image 2",
                }
            };

            var actualEventsBeforeUpdate = await _storage.GetAllAsync();

            var idsToDelete = new List<Guid>()
            {
                Guid.Parse("403dfbd3-a4ac-4d41-b495-59907c7079eb"),
                Guid.Parse("48be7ed9-acc2-4723-9ba2-deb2b62747cd"),
            };

            var expectedEvents = new List<ThirdPartyEvent>();

            Task[] tasks = new Task[idsToDelete.Count];

            // Act
            for (int i = 0; i < idsToDelete.Count; i++)
            {
                var index = i;
                tasks[index] = Task.Run(() => _storage.DeleteAsync(idsToDelete[index]));
            }

            Task.WaitAll(tasks);

            var actualEvents = await _storage.GetAllAsync();

            // Assert
            actualEventsBeforeUpdate.Should().BeEquivalentTo(expectedEventsBeforeUpdate);
            actualEvents.Should().BeEquivalentTo(expectedEvents);
        }

        [Test]
        public async Task Update_MultipleThreads_UpdatesEvents()
        {
            // Arrange
            var expectedEventsBeforeUpdate = new List<ThirdPartyEvent>()
            {
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("403dfbd3-a4ac-4d41-b495-59907c7079eb"),
                    Name = "First event",
                    Description = "Description 1",
                    StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2023, 1, 1, 12, 0, 0),
                    PosterImage = "Image 1",
                },
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("48be7ed9-acc2-4723-9ba2-deb2b62747cd"),
                    Name = "Second event",
                    Description = "Description 2",
                    StartDate = new DateTime(2024, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2024, 1, 1, 12, 0, 0),
                    PosterImage = "Image 2",
                }
            };

            var actualEventsBeforeUpdate = await _storage.GetAllAsync();

            var eventsToUpdate = new List<ThirdPartyEvent>()
            {
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("403dfbd3-a4ac-4d41-b495-59907c7079eb"),
                    Name = "First updated event",
                    Description = "Updated description 1",
                    StartDate = new DateTime(2023, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2023, 1, 1, 12, 0, 0),
                    PosterImage = "Updated image 1",
                },
                new ThirdPartyEvent
                {
                    Id = Guid.Parse("48be7ed9-acc2-4723-9ba2-deb2b62747cd"),
                    Name = "Second updated event",
                    Description = "Updated description 2",
                    StartDate = new DateTime(2025, 1, 1, 10, 0, 0),
                    EndDate = new DateTime(2025, 1, 3, 10, 0, 0),
                    PosterImage = "Updated image 2",
                }
            };

            Task[] tasks = new Task[eventsToUpdate.Count];

            // Act
            for (int i = 0; i < eventsToUpdate.Count; i++)
            {
                var index = i;
                tasks[index] = Task.Run(() => _storage.UpdateAsync(eventsToUpdate[index]));
            }

            Task.WaitAll(tasks);

            var actualEvents = await _storage.GetAllAsync();

            // Assert
            actualEventsBeforeUpdate.Should().BeEquivalentTo(expectedEventsBeforeUpdate);
            actualEvents.Should().BeEquivalentTo(eventsToUpdate);
        }
    }
}