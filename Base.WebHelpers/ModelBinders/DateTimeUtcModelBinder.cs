using Microsoft.Extensions.DependencyInjection;

namespace Base.WebHelpers.ModelBinders;

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class DateTimeUtcModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var valueAsString = valueProviderResult.FirstValue;

        if (DateTime.TryParse(valueAsString, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dateTimeValue))
        {
            if (dateTimeValue.Kind != DateTimeKind.Utc && !valueAsString.EndsWith("Z"))
            {
                dateTimeValue = DateTime.SpecifyKind(dateTimeValue, DateTimeKind.Utc);
            }

            bindingContext.Result = ModelBindingResult.Success(dateTimeValue);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid datetime format.");
        }

        return Task.CompletedTask;
    }
}

public class DateTimeUtcModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(DateTime) ? new DateTimeUtcModelBinder() : null;
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddDateTimeUtcModelBinder(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new DateTimeUtcModelBinderProvider());
        });
    }
}