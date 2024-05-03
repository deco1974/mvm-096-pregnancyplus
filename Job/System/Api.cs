using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Job.Data.Models;
using Microsoft.Extensions.Configuration;

namespace Job.System
{
	public class Api
	{
		private IConfiguration _settings;
		public Api(IConfiguration settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// 
		/// </summary>
		public string? Auth()
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://digitalplus-test.eu.auth0.com/oauth/token");

			var objAuth = new ApiAuth
			{
				Audience     = _settings["audience"],
				GrantType    = "client_credentials",
				ClientId     = _settings["client_id"],
				ClientSecret = _settings["client_secret"]
			};

			var json = JsonSerializer.Serialize(objAuth);
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = client.Send(request);
			response.EnsureSuccessStatusCode();

			return response.Content.ReadAsStringAsync().Result;
		}
	}
}
