using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore3CSharp8
{
    class Program
    {
        static void Main()
        {
            var host = Host
                .CreateDefaultBuilder()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlServer("Data Source=(localDB)\\MSSQLLocalDB;Initial Catalog=foo;Integrated Security=True")
                            .EnableSensitiveDataLogging()
                            .ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Information)));
                    });
                })
                .Build();

            using var applicationDbContext = host.Services.GetRequiredService<ApplicationDbContext>();

            Seeder.Seed(applicationDbContext);

            var message = applicationDbContext.Foos.Where(s => s.Id == 123).FirstOrDefault();
            message = applicationDbContext.Foos.Where(s => s.Required == "123").FirstOrDefault();
            message = applicationDbContext.Foos.Where(s => s.Nullable == "123").FirstOrDefault();

            long id = 123;
            string code = "123";
            message = applicationDbContext.Foos.Where(s => s.Id == id).FirstOrDefault();
            message = applicationDbContext.Foos.Where(s => s.Required == code).FirstOrDefault();
            message = applicationDbContext.Foos.Where(s => s.Nullable == code).FirstOrDefault();
        }
    }
}
