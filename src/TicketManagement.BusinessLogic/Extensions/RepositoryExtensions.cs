﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task CheckIfIdExistsAsync<T>(this IRepository<T> repository, int id)
        {
            var entity = await repository.GetByIdAsync(id);

            if (entity is null)
            {
                throw new ValidationException("Entity was not found.");
            }
        }
    }
}
