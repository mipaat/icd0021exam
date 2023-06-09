using BLL.DTO;
using BLL.DTO.Exceptions;
using DAL;
using Domain;

namespace BLL;

public static class RecipeHelpers
{
    public static float TotalAmount(this Product product) =>
        product.ProductExistences!.Sum(e => e.Amount);

    public static float TotalAmount(this RecipeProduct recipeProduct) =>
        recipeProduct.Product!.TotalAmount();

    public static bool IsMissingIngredient(this RecipeProduct recipeProduct)
    {
        return recipeProduct.TotalAmount() < recipeProduct.Amount;
    }

    public static float MissingAmount(this RecipeProduct recipeProduct) =>
        recipeProduct.Amount - recipeProduct.TotalAmount();

    public static List<MissingIngredient> GetMissingIngredients(this Recipe recipe)
    {
        var result = new List<MissingIngredient>();
        foreach (var recipeProduct in recipe.RecipeProducts!)
        {
            var totalAmount = recipeProduct.Product!.ProductExistences!.Sum(e => e.Amount);
            if (totalAmount < recipeProduct.Amount)
            {
                result.Add(new MissingIngredient(recipeProduct.Product, recipeProduct.Amount - totalAmount));
            }
        }

        return result;
    }

    public static bool IsPreparable(this Recipe recipe)
    {
        return recipe.GetMissingIngredients().Count == 0;
    }

    public static void PrepareRecipe(AppDbContext dbContext, Recipe recipe)
    {
        var missing = new List<MissingIngredient>();
        foreach (var recipeProduct in recipe.RecipeProducts!)
        {
            var amount = recipeProduct.Amount;
            foreach (var productExistence in recipeProduct.Product!.ProductExistences!)
            {
                if (amount <= 0) break;
                var amountToRemove = Math.Min(amount, productExistence.Amount);
                productExistence.Amount -= amountToRemove;
                amount -= amountToRemove;
                dbContext.Entry(productExistence).Property(e => e.Amount).IsModified = true;
            }

            if (amount > 0)
            {
                missing.Add(new MissingIngredient(recipeProduct.Product, amount));
            }
        }

        if (missing.Count > 0) throw new NotEnoughIngredientsException(missing);
    }
}