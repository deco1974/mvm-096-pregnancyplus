using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Job.Data;
using Job.Data.Models;
using Microsoft.Extensions.Configuration;

namespace Job.System
{
	public class Api : IDisposable
	{
		private readonly IConfigurationSection _settings;
		public Api(IConfigurationSection settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// 
		/// </summary>
		public string? Auth()
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, _settings["endpoint_auth"]);

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
			try
			{
				response.EnsureSuccessStatusCode();
				return response.Content.ReadAsStringAsync().Result;
			}
			catch (HttpRequestException e)
			{
				new PregnancyController(_settings).Log(Log.TypeEnum.Error, "Error getting token:\n" + e.HttpRequestError);
			}

			return null;
		}


		/// <summary>
		/// Get registrations
		/// </summary>
		/// <param name="token"></param>
		/// <param name="from"></param>
		/// <returns></returns>
		public ApiRegistration.Response? GetRegistrations(Token token, DateTime? from = null, DateTime? to = null, int? page = null, int? limit = null)
		{
			var client  = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, _settings["endpoint_registration"]);
			request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

			var objRegistration = new ApiRegistration
			{
				OptinDateFrom = from?.ToString("yyyy-MM-ddTHH:mm:ss.000Z"),
			};

			var json = JsonSerializer.Serialize(objRegistration);
			request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = client.Send(request);
			try
			{
				response.EnsureSuccessStatusCode();
				var result = response.Content.ReadAsStringAsync().Result;
				

				return JsonSerializer.Deserialize<ApiRegistration.Response>(result, new JsonSerializerOptions { IgnoreNullValues = true});
			}
			catch (HttpRequestException e)
			{
				new PregnancyController(_settings).Log(Log.TypeEnum.Error, "Error getting registrations:\n" + e.StatusCode);
				Helper.Email("Error Pregnancy+", "Error getting registrations:\n\n" + e.StatusCode);
			}

			return null;
		}

		public void Dispose()
		{
			// TODO release managed resources here
		}
	}
}
