using AutoMapper;
using RestEase.HttpClientFactory;
using Serilog;
using TicketManagement.VenueApi.Clients.UserApi;
using TicketManagement.VenueApi.DependencyResolving;
using TicketManagement.VenueApi.JwtAuthentication;
using TicketManagement.VenueApi.MappingConfig;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

builder.Services.AddScoped(provider => new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
})
.CreateMapper());

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("TicketManagement.Database");

builder.Services.AddEntityFrameworkServices(connectionString);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtAutheticationConstants.SchemeName)
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(JwtAutheticationConstants.SchemeName, null);

builder.Services.AddRestEaseClient<IUserClient>(builder.Configuration["UserApi:BaseAddress"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
