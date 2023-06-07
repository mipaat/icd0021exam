using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Base.WebHelpers.ModelBinders;

// https://stackoverflow.com/questions/65544525/asp-net-core-comma-separated-array-in-query-string-binder
public class CommaSeparatedArrayModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        return CommaSeparatedArrayModelBinder.IsSupportedModelType(context.Metadata.ModelType) ? new CommaSeparatedArrayModelBinder() : null;
    }
}