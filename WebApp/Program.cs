using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using Base.WebHelpers;
using Base.WebHelpers.ModelBinders;
using BLL.DTO.Exceptions;
using BLL.Identity;
using BLL.Identity.Config;
using BLL.Identity.Extensions;
using DAL;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Public.DTO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCustomIdentity();

builder.Services.AddScoped<IdentityAppDataInit>();

var jwtSettings = builder.Configuration.GetRequiredSection(JwtSettings.SectionKey).Get<JwtSettings>();

builder.Services.AddDateTimeUtcModelBinder();

builder.Services
    .AddAuthentication()
    .AddCookie(options => { options.SlidingExpiration = true; })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddLocalization();

builder.Services.DisableApiErrorRedirects(accessDeniedRedirect: context =>
{
    context.Response.Redirect("/Home/AccessDenied");
    return context.Response.CompleteAsync();
}, loginRedirect: context =>
{
    context.Response.Redirect($"/Identity/Account/Login?returnUrl={HttpUtility.UrlEncode(context.Request.GetFullPath())}");
    return context.Response.CompleteAsync();
});

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsAllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

builder.Services.AddHttpLogging(logging => { logging.LoggingFields = HttpLoggingFields.All; });

builder.Services.AddControllersWithViews()
    .AddCommaSeparatedArrayModelBinderProvider()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Register BLL Services

// TODO

// Build app

var app = builder.Build();

var defaultCulture = new CultureInfo("en-US").UseConstantDateTime();
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.UseConstantDateTime())
        .ToList(),
};

app.UseRequestLocalization(localizationOptions);

SetupAppData(app, builder.Configuration);

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("CorsAllowAll");

app.UseAuthentication();
app.UseAuthorization();

// app.MapAreaControllerRoute(name: "identity", areaName: "Identity",
//     pattern: "Identity/{controller}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (NotFoundException)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
});

app.Run();


static void SetupAppData(IApplicationBuilder app, IConfiguration configuration)
{
    using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
    var services = scope.ServiceProvider;

    // DbContext
    var dbContext = services.GetRequiredService<AppDbContext>();
    if (configuration.GetValue<bool>("DatabaseInit:DropDatabase"))
    {
        dbContext.Database.EnsureDeleted();
    }

    if (configuration.GetValue<bool>("DatabaseInit:Migrate"))
    {
        dbContext.Database.Migrate();
    }

    // Identity
    var identityAppDataInit = services.GetRequiredService<IdentityAppDataInit>();
    identityAppDataInit.RunInitAsync().Wait();

    dbContext.SaveChanges();
}