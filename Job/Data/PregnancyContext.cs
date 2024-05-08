using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Job.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Job.Data
{
	public class PregnancyContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();

			var connectionString = configuration.GetConnectionString("CsPregnancy");
			optionsBuilder.UseSqlServer(connectionString);
		}

		public virtual DbSet<Registration> Registrations { get; set; }
		public virtual DbSet<Token> Tokens { get; set; }
		public virtual DbSet<Log> Logs { get; set; }
	}
}
