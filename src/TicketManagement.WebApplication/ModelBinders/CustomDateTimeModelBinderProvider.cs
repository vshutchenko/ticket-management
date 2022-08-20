using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace TicketManagement.WebApplication.ModelBinders
{
    public class CustomDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (CustomDateTimeModelBinder.SupportedTypes.Contains(context.Metadata.ModelType))
            {
                return new BinderTypeModelBinder(typeof(CustomDateTimeModelBinder));
            }

            return null!;
        }
    }
}
