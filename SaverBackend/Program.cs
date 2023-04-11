using Microsoft.EntityFrameworkCore;

using SaverBackend.Configuration;

using SaverBackend.Hubs;
using SaverBackend.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(
            ApplicationContext.ConnectionString,
            ServerVersion.AutoDetect(ApplicationContext.ConnectionString)));

builder.Configuration.AddJsonFile("appsettings.json");

ApplicationContext.ConnectionString = builder.Configuration.GetSection(nameof(ConnectionStrings))["DatabaseConnection"];

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddCoreAdmin("user");

//builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

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
