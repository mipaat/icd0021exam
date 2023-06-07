using BLL.Identity.Services;
using DAL;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCustomIdentity(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddIdentity<User, Role>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
        serviceCollection.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
        });
        serviceCollection.AddScoped<UserService>();
        serviceCollection.AddScoped<TokenService>();
        serviceCollection.AddScoped<IdentityUow>();
    }
}