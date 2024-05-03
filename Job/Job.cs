using System.Text.Json;
using Job.Data;
using Job.Data.Models;
using Job.System;
using Microsoft.Extensions.Configuration;

namespace Job
{
	internal class Job
	{
		static void Main(string[] args)
		{
			var config = new ConfigurationBuilder();
			var settings = config.AddJsonFile("appsettings.json").AddEnvironmentVariables().Build().GetSection("Settings");
			var token = new Token();

			

			// check valid bearer token
			using (var context = new PregnancyContext())
			{
				// convert integer to double
				token = context.Tokens.OrderByDescending(t => t.Id).FirstOrDefault();
				if (token is { Expired: true })
				{
					var api  = new Api(settings);
					var auth = api.Auth();

					if (!string.IsNullOrEmpty(auth))
					{
						token = JsonSerializer.Deserialize<Token>(auth);
						if (token != null)
						{
							token.Created = DateTime.Now;
							context.Tokens.Add(token);
							context.SaveChanges();
						}
						else
						{
							Helper.Email("Error Pregnancy+", "Error 401 - authentication failed\n\n");
						}
					}
					else
					{
						Helper.Email("Error Pregnancy+", "Error 400 - connection failed\n\n");
					}
				}

			}

			
			if (token != null && !string.IsNullOrEmpty(token.AccessToken))
			{
				// do something
				
			}
		}
	}
}