// See https://aka.ms/new-console-template for more information
using Flurl.Http;
using Microsoft.EntityFrameworkCore;
using SaverBackend.Models;

string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

Console.WriteLine("Hello, World!");

DbContextOptions<ApplicationContext> options = new DbContextOptionsBuilder<ApplicationContext>()
    .UseMySql(
            "Server=192.168.88.252;Database=mobilesdb2;Uid=user;Pwd=password;",
            ServerVersion.AutoDetect("Server=192.168.88.252;Database=mobilesdb2;Uid=user;Pwd=password;"))
    .Options;

ApplicationContext context = new ApplicationContext(options);

var content = context.Contents.ToArray();


Console.WriteLine(content.Length);

int length = content.Length;
int counter = 0;

foreach (var cont in content) 
{
    await cont.ImageUri.DownloadFileAsync($"{directory}/data");
    counter++;
    Console.WriteLine($"Downloaded {counter} of {length}");
}