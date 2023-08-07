using BananasGamblerBackend.Configuration;
using BananasGamblerBackend.Database;
using BananasGamblerBackend.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHostedService<RabbitMqListener>();

// I can add another context with another models and host it as separate db service
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseMySql(
            ApplicationContext.ConnectionString,
            ServerVersion.AutoDetect(ApplicationContext.ConnectionString)));

builder.Configuration.AddJsonFile("appsettings.json");
ApplicationContext.ConnectionString = builder.Configuration.GetSection(nameof(ConnectionStrings))["DefaultConnection"];
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
