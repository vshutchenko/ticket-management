using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class LayoutService : ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IValidator<Layout> _layoutValidator;

        public LayoutService(IRepository<Layout> layoutRepository, IValidator<Layout> layoutValidator)
        {
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _layoutValidator = layoutValidator ?? throw new ArgumentNullException(nameof(layoutValidator));
        }

        public Task<int> CreateAsync(Layout layout)
        {
            if (layout is null)
            {
                throw new ValidationException("Layout is null.");
            }

            layout.Id = 0;

            _layoutValidator.Validate(layout);

            return _layoutRepository.CreateAsync(layout);
        }

        public async Task DeleteAsync(int id)
        {
            await _layoutRepository.CheckIfIdExistsAsync(id);

            await _layoutRepository.DeleteAsync(id);
        }

        public IEnumerable<Layout> GetAll()
        {
            return _layoutRepository.GetAll();
        }

        public async Task<Layout> GetByIdAsync(int id)
        {
            await _layoutRepository.CheckIfIdExistsAsync(id);

            return await _layoutRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(Layout layout)
        {
            if (layout is null)
            {
                throw new ValidationException("Layout is null.");
            }

            await _layoutRepository.CheckIfIdExistsAsync(layout.Id);

            _layoutValidator.Validate(layout);

            await _layoutRepository.UpdateAsync(layout);
        }
    }
}
