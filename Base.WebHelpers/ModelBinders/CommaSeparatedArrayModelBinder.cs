using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Base.WebHelpers.ModelBinders;

// https://stackoverflow.com/questions/65544525/asp-net-core-comma-separated-array-in-query-string-binder
public class CommaSeparatedArrayModelBinder : IModelBinder
{
    private static Task CompletedTask => Task.CompletedTask;

    private static readonly Type[] SupportedElementTypes = {
        typeof(int), typeof(long), typeof(short), typeof(byte),
        typeof(uint), typeof(ulong), typeof(ushort), typeof(Guid)
    };

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (!IsSupportedModelType(bindingContext.ModelType)) return CompletedTask;

        var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (providerValue == ValueProviderResult.None) return CompletedTask;

        // Each value self may contains a series of actual values, split it with comma
        var strings = providerValue.Values.SelectMany(s => s?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>()).ToList();

        if (!strings.Any() || strings.Any(string.IsNullOrWhiteSpace))
            return CompletedTask;

        var elementType = bindingContext.ModelType.GetElementType();
        if (elementType == null) return CompletedTask;

        var realResult = CopyAndConvertArray(strings, elementType);

        bindingContext.Result = ModelBindingResult.Success(realResult);

        return CompletedTask;
    }

    internal static bool IsSupportedModelType(Type modelType)
    {
        return modelType.IsArray && modelType.GetArrayRank() == 1
                && modelType.HasElementType
                && SupportedElementTypes.Contains(modelType.GetElementType());
    }

    private static Array CopyAndConvertArray(IList<string> sourceArray, Type elementType)
    {
        var targetArray = Array.CreateInstance(elementType, sourceArray.Count);
        if (sourceArray.Count > 0)
        {
            var converter = TypeDescriptor.GetConverter(elementType);
            for (var i = 0; i < sourceArray.Count; i++)
                targetArray.SetValue(converter.ConvertFromString(sourceArray[i]), i);
        }
        return targetArray;
    }
}