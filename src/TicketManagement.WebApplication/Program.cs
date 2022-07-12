using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TicketManagement.BusinessLogic.DependencyResolving;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.WebApplication.Filters;
using TicketManagement.WebApplication.Infrastructure;
using TicketManagement.WebApplication.ModelBinders;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped(provider => new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
    mc.AddProfile(new TicketManagement.BusinessLogic.MappingConfig.MappingProfile());
})
.CreateMapper());

string? connectionString = builder.Configuration.GetConnectionString("TicketManagement.Database");

builder.Services.AddEntityFrameworkServices(connectionString);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
        options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/AccessDenied");
    });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
    options.Filters.Add<UnexpectedExceptionFilter>();
    options.Filters.Add<ValidationExceptionFilter>();
})
    .AddRazorRuntimeCompilation()
    .AddDataAnnotationsLocalization()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo>? supportedCultures = new List<CultureInfo> { new CultureInfo("en-US"), new CultureInfo("ru-RU"), new CultureInfo("be-BY") };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    AcceptLanguageHeaderRequestCultureProvider? requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
    options.RequestCultureProviders.Remove(requestProvider);
});

WebApplication? app = builder.Build();

using IServiceScope? scope = app.Services.CreateScope();

IIdentityService? identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();

await identityService.SeedInitialData();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Event}/{action=Index}");
app.MapRazorPages();

app.Run();
