using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;

namespace Base.WebHelpers.ModelBinders;

// https://stackoverflow.com/questions/65544525/asp-net-core-comma-separated-array-in-query-string-binder
public static class CommaSeparatedArrayModelBinderServiceCollectionExtensions
    {
        private static int FirstIndexOfOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var result = 0;

            foreach (var item in source)
            {
                if (predicate(item))
                    return result;

                result++;
            }

            return -1;
        }

        private static int FindModelBinderProviderInsertLocation(this IList<IModelBinderProvider> modelBinderProviders)
        {
            var index = modelBinderProviders.FirstIndexOfOrDefault(i => i is FloatingPointTypeModelBinderProvider);
            return index < 0 ? index : index + 1;
        }

        public static void InsertCommaSeparatedArrayModelBinderProvider(this IList<IModelBinderProvider> modelBinderProviders)
        {
            // Argument Check
            if (modelBinderProviders == null)
                throw new ArgumentNullException(nameof(modelBinderProviders));

            var providerToInsert = new CommaSeparatedArrayModelBinderProvider();

            // Find the location of SimpleTypeModelBinder, the CommaSeparatedArrayModelBinder must be inserted before it.
            var index = modelBinderProviders.FindModelBinderProviderInsertLocation();

            if (index != -1)
                modelBinderProviders.Insert(index, providerToInsert);
            else
                modelBinderProviders.Add(providerToInsert);
        }

        public static MvcOptions AddCommaSeparatedArrayModelBinderProvider(this MvcOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            options.ModelBinderProviders.InsertCommaSeparatedArrayModelBinderProvider();
            return options;
        }

        public static IMvcBuilder AddCommaSeparatedArrayModelBinderProvider(this IMvcBuilder builder)
        {
            builder.AddMvcOptions(options => AddCommaSeparatedArrayModelBinderProvider(options));
            return builder;
        }
    }