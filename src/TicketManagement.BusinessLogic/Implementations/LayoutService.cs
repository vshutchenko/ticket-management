using System;
using System.Collections.Generic;
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

        public int Create(Layout layout)
        {
            if (layout is null)
            {
                throw new ValidationException("Layout is null.");
            }

            layout.Id = 0;

            _layoutValidator.Validate(layout);

            return _layoutRepository.Create(layout);
        }

        public void Delete(int id)
        {
            _layoutRepository.CheckIfIdExists(id);

            _layoutRepository.Delete(id);
        }

        public IEnumerable<Layout> GetAll()
        {
            return _layoutRepository.GetAll();
        }

        public Layout GetById(int id)
        {
            _layoutRepository.CheckIfIdExists(id);

            return _layoutRepository.GetById(id);
        }

        public void Update(Layout layout)
        {
            if (layout is null)
            {
                throw new ValidationException("Layout is null.");
            }

            _layoutRepository.CheckIfIdExists(layout.Id);

            _layoutValidator.Validate(layout);

            _layoutRepository.Update(layout);
        }
    }
}
