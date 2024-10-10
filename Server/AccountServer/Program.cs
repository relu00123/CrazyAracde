using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AccountServer.DB;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace AccountServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
            // DB초기화 코드
            //var configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json")
            //.Build();

            //string connectionString = configuration.GetConnectionString("DefaultConnection");

            //RemoveAllDataTemp(connectionString);

            CreateHostBuilder(args).Build().Run();


             
        }

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();

                    // 10.10 추가한코드 
                    webBuilder.UseUrls("https://0.0.0.0:5001", "http://0.0.0.0:5000"); // 모든 IP에서 수신하도록 설정
                });

        public static void RemoveAllDataTemp(string connectionString)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                              .UseSqlServer(connectionString)
                              .Options;

            using (AppDbContext context = new AppDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

    }
}
