using RestEase.HttpClientFactory;
using Serilog;
using Serilog.Events;
using TicketManagement.Core.Clients.EventApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(builder.Configuration["LogsPath"])
            .CreateLogger();

builder.Services.AddControllersWithViews();

builder.Services.AddRestEaseClient<IEventClient>(builder.Configuration["EventApi:BaseAddress"]);
builder.Services.AddRestEaseClient<IEventAreaClient>(builder.Configuration["EventApi:BaseAddress"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
