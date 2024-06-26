using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.HttpOverrides;

using SaverBackend.Configuration;

using SaverBackend.Hubs;
using SaverBackend.Models;
using SaverBackend.Services.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();



builder.Configuration.AddJsonFile("appsettings.json");

ApplicationContext.ConnectionString = builder.Configuration.GetSection(nameof(ConnectionStrings))["DefaultConnection"];

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(
            ApplicationContext.ConnectionString,
            ServerVersion.AutoDetect(ApplicationContext.ConnectionString)));

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddCoreAdmin("user");

//builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || !app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseStaticFiles();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapDefaultControllerRoute();
app.MapHub<MainNotificationsHub>("/notify");
app.Run();
