using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TicketManagement.DataAccess.DependencyResolving;
using TicketManagement.UserApi.JwtAuthentication;
using TicketManagement.UserApi.MappingConfig;
using TicketManagement.UserApi.Services.Implementations;
using TicketManagement.UserApi.Services.Interfaces;

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

builder.Services.AddScoped<ITokenService, TokenService>(
    p => new TokenService(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Audience"], builder.Configuration["Jwt:Issuer"]));

var connectionString = builder.Configuration.GetConnectionString("TicketManagement.Database");

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddEntityFrameworkRepositories(connectionString);

builder.Services.AddAuthentication(JwtAutheticationConstants.SchemeName)
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(JwtAutheticationConstants.SchemeName, null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
