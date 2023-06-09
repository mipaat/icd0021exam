namespace BLL.DTO.Exceptions;

public class NotEnoughIngredientsException : Exception
{
    public List<MissingIngredient> MissingIngredients { get; }
    
    public NotEnoughIngredientsException(List<MissingIngredient> missingIngredients)
    {
        MissingIngredients = missingIngredients;
    }
}