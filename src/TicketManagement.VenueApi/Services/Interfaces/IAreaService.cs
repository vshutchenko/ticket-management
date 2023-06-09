﻿using TicketManagement.VenueApi.Models;

namespace TicketManagement.VenueApi.Services.Interfaces
{
    public interface IAreaService
    {
        IEnumerable<AreaModel> GetAll();

        Task<IEnumerable<AreaModel>> GetByLayoutIdAsync(int layoutId);

        Task<AreaModel> GetByIdAsync(int id);

        Task<int> CreateAsync(AreaModel areaModel);

        Task UpdateAsync(AreaModel areaModel);

        Task DeleteAsync(int id);
    }
}
