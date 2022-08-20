using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TicketManagement.WebApplication.ModelBinders
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        public static readonly Type[] SupportedTypes =
            new Type[] { typeof(DateTime), typeof(DateTime?) };
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (!SupportedTypes.Contains(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext
                .ValueProvider
                .GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext
                .ModelState
                .SetModelValue(modelName, valueProviderResult);
            var dateTimeToParse
                = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(dateTimeToParse))
            {
                return Task.CompletedTask;
            }

            var formattedDateTime = DateTime.Parse(dateTimeToParse, CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Name));

            bindingContext.Result = ModelBindingResult.Success(formattedDateTime);

            return Task.CompletedTask;
        }
    }
}
