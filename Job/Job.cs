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
			var pregctrl = new PregnancyController(settings);

			pregctrl.Log(Log.TypeEnum.Info, "Token request");
			var token = pregctrl.GetToken();
			if (token != null && !string.IsNullOrEmpty(token.AccessToken))
			{
				var result = pregctrl.GetRegistrations(token);
			}

			var send = pregctrl.SendRegistrations();
		}
	}
}