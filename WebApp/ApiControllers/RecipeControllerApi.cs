using AutoMapper;
using Base.WebHelpers;
using DAL;
using Domain;

namespace WebApp.ApiControllers;

public class RecipeControllerApi : BaseDbControllerApi<AppDbContext, Recipe>
{
    public RecipeControllerApi(AppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }
}