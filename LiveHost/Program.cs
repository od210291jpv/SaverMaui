using LiveHost.Configuration;
using LiveHost.Services;
using Microsoft.AspNetCore.HttpOverrides;
using SaverBackend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqListener>();
builder.Services.AddHostedService<ContentDownloadService>();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(
            ApplicationContext.ConnectionString,
            ServerVersion.AutoDetect("Server=192.168.88.252;Database=mobilesdb;Uid=user;Pwd=password;")));

//builder.Services.AddDbContext<BananasGamblerApplicationContext>(opt => 
//    opt.UseMySql(
//        BananasGamblerApplicationContext.ConnectionString,
//        ServerVersion.AutoDetect(BananasGamblerApplicationContext.ConnectionString)));

//builder.Configuration.AddJsonFile("appsettings.json");

//ApplicationContext.ConnectionString = builder.Configuration.GetSection(nameof(ConnectionStrings))["DefaultConnection"];
//BananasGamblerApplicationContext.ConnectionString = builder.Configuration.GetSection(nameof(ConnectionStrings))["BananasGamblerDbConnection"];

builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.Run();
