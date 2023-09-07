using BananasGamblerBackend.Configuration;
using BananasGamblerBackend.Database;
using BananasGamblerBackend.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                });

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
app.UseAuthentication();
app.MapControllers();
app.MapDefaultControllerRoute();
app.Run();
