using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using RestEase.HttpClientFactory;
using Serilog;
using TicketManagement.WebApplication.Clients.EventApi;
using TicketManagement.WebApplication.Clients.PurchaseApi;
using TicketManagement.WebApplication.Clients.UserApi;
using TicketManagement.WebApplication.Clients.VenueApi;
using TicketManagement.WebApplication.Filters;
using TicketManagement.WebApplication.Infrastructure;
using TicketManagement.WebApplication.JwtAuthentication;
using TicketManagement.WebApplication.ModelBinders;
using TicketManagement.WebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(provider => new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
})
.CreateMapper());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(JwtAutheticationConstants.SchemeName)
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(JwtAutheticationConstants.SchemeName, null);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
    options.Filters.Add<UnexpectedExceptionFilter>();
})
    .AddRazorRuntimeCompilation()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("ru-RU"), new CultureInfo("be-BY") };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    var requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
    options.RequestCultureProviders.Remove(requestProvider);
});

builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddRestEaseClient<IEventClient>(builder.Configuration["EventApi:BaseAddress"]);
builder.Services.AddRestEaseClient<IEventAreaClient>(builder.Configuration["EventApi:BaseAddress"]);

builder.Services.AddRestEaseClient<IPurchaseClient>(builder.Configuration["PurchaseApi:BaseAddress"]);

builder.Services.AddRestEaseClient<IVenueClient>(builder.Configuration["VenueApi:BaseAddress"]);
builder.Services.AddRestEaseClient<ILayoutClient>(builder.Configuration["VenueApi:BaseAddress"]);

builder.Services.AddRestEaseClient<IUserClient>(builder.Configuration["UserApi:BaseAddress"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    var tokenService = context.RequestServices.GetRequiredService<ITokenService>();

    var token = tokenService.GetToken();

    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Add("Authorization", $"{tokenService.Scheme} {token}");
    }

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=Index}");

app.Run();
