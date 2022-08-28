using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using RestEase.HttpClientFactory;
using Serilog;
using Serilog.Events;
using TicketManagement.Core.Clients.UserApi;
using TicketManagement.Core.JwtAuthentication;
using TicketManagement.Core.Validation;
using TicketManagement.DataAccess.DependencyResolving;
using TicketManagement.UserApi.Data;
using TicketManagement.UserApi.MappingConfig;
using TicketManagement.UserApi.Services.Implementations;
using TicketManagement.UserApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(builder.Configuration["LogsPath"])
            .CreateLogger();

builder.Services.AddScoped(provider => new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
})
.CreateMapper());

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationExceptionFilter>();
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>(
    p => new TokenService(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Audience"], builder.Configuration["Jwt:Issuer"]));

var connectionString = builder.Configuration.GetConnectionString("TicketManagement.Database");

builder.Services.AddEntityFrameworkRepositories(connectionString);

builder.Services.AddScoped<ContextSeeder>();

builder.Services.AddAuthentication(JwtAutheticationConstants.SchemeName)
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(JwtAutheticationConstants.SchemeName, null);

builder.Services.AddRestEaseClient<IUserClient>(builder.Configuration["UserApi:BaseAddress"]);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User API",
        Version = "v1",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Description = "Jwt Token is required to access the endpoints",
        In = ParameterLocation.Header,
        Name = "JWT Authentication",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme,
        },
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() },
    });
});

builder.Host.UseSerilog();

var app = builder.Build();

using var scope = app.Services.CreateScope();

var seeder = scope.ServiceProvider.GetRequiredService<ContextSeeder>();

await seeder.SeedInitialDataAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
