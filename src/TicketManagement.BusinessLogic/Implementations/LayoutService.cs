using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TicketManagement.BusinessLogic.Extensions;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Models;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class LayoutService : ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IValidator<Layout> _layoutValidator;
        private readonly IMapper _mapper;

        public LayoutService(IRepository<Layout> layoutRepository, IValidator<Layout> layoutValidator, IMapper mapper)
        {
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _layoutValidator = layoutValidator ?? throw new ArgumentNullException(nameof(layoutValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<int> CreateAsync(LayoutModel layoutModel)
        {
            if (layoutModel is null)
            {
                throw new ValidationException("Layout is null.");
            }

            layoutModel.Id = 0;

            var layout = _mapper.Map<Layout>(layoutModel);

            _layoutValidator.Validate(layout);

            return _layoutRepository.CreateAsync(layout);
        }

        public async Task DeleteAsync(int id)
        {
            await _layoutRepository.CheckIfIdExistsAsync(id);

            await _layoutRepository.DeleteAsync(id);
        }

        public IEnumerable<LayoutModel> GetAll()
        {
            var models = _layoutRepository.GetAll().Select(l => _mapper.Map<LayoutModel>(l));

            return models;
        }

        public async Task<LayoutModel> GetByIdAsync(int id)
        {
            await _layoutRepository.CheckIfIdExistsAsync(id);

            var layout = await _layoutRepository.GetByIdAsync(id);

            var model = _mapper.Map<LayoutModel>(layout);

            return model;
        }

        public async Task UpdateAsync(LayoutModel layoutModel)
        {
            if (layoutModel is null)
            {
                throw new ValidationException("Layout is null.");
            }

            await _layoutRepository.CheckIfIdExistsAsync(layoutModel.Id);

            var layout = _mapper.Map<Layout>(layoutModel);

            _layoutValidator.Validate(layout);

            await _layoutRepository.UpdateAsync(layout);
        }
    }
}
