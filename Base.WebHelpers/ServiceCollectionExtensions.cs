using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Base.WebHelpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection DisableApiErrorRedirects(this IServiceCollection services,
        string apiPrefix = "/api", Func<RedirectContext<CookieAuthenticationOptions>, Task>? accessDeniedRedirect = null,
        Func<RedirectContext<CookieAuthenticationOptions>, Task>? loginRedirect = null)
    {
        services.ConfigureApplicationCookie(options =>
        {
            var defaultLoginRedirect = loginRedirect ?? options.Events.OnRedirectToLogin;
            options.Events.OnRedirectToLogin = async context =>
            {
                if (context.Request.Path.StartsWithSegments(apiPrefix))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                await defaultLoginRedirect(context);
            };

            var defaultAccessDeniedRedirect = accessDeniedRedirect ?? options.Events.OnRedirectToAccessDenied;
            options.Events.OnRedirectToAccessDenied = async context =>
            {
                if (context.Request.Path.StartsWithSegments(apiPrefix))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

                await defaultAccessDeniedRedirect(context);
            };
        });

        return services;
    }
}