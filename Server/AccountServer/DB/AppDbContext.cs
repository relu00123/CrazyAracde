using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountServer.DB
{
	public class AppDbContext : DbContext
	{
		public DbSet<AccountDb> Accounts { get; set; }

        private readonly string _connectionString;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
		{
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options)
        {
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
			if (!options.IsConfigured)
				options.UseSqlServer(_connectionString);
        }



        protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<AccountDb>()
				.HasIndex(a => a.AccountName)
				.IsUnique();
		}

       


    }
}
