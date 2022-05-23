using System;
using System.Collections.Generic;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.BusinessLogic.Validation;
using TicketManagement.DataAccess.Entities;
using TicketManagement.DataAccess.Interfaces;

namespace TicketManagement.BusinessLogic.Implementations
{
    internal class LayoutService : ILayoutService
    {
        private readonly IRepository<Layout> _layoutRepository;
        private readonly IValidator<Layout> _validationService;

        public LayoutService(IRepository<Layout> layoutRepository, IValidator<Layout> validationService)
        {
            _layoutRepository = layoutRepository ?? throw new ArgumentNullException(nameof(layoutRepository));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        }

        public int Create(Layout layout)
        {
            if (layout is null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            _validationService.Validate(layout);

            return _layoutRepository.Create(layout);
        }

        public void Delete(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            _layoutRepository.Delete(id);
        }

        public IEnumerable<Layout> GetAll()
        {
            return _layoutRepository.GetAll();
        }

        public Layout GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException(nameof(id));
            }

            return _layoutRepository.GetById(id);
        }

        public void Update(Layout layout)
        {
            if (layout is null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            _validationService.Validate(layout);

            _layoutRepository.Update(layout);
        }
    }
}
