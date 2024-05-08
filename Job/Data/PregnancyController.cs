using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Job.Data.Models;
using Job.Data.Models.Gates;
using Job.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Job.Data
{
	public class PregnancyController
	{
		private IConfigurationSection _settings;
		private JsonSerializerOptions _jsonoptions;

		public PregnancyController(IConfigurationSection settings)
		{
			_settings = settings;

			_jsonoptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy        = new LowerCaseNamingPolicy(),
				PropertyNameCaseInsensitive = true,
				WriteIndented               = true,
				Encoder                     = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};
		}

		public void Log(Log.TypeEnum type, string? remark)
		{
			using (var ctx = new PregnancyContext())
			{
				var log = new Log
				{
					Created = DateTime.Now,
					Type    = type,
					Remark  = remark
				};
				ctx.Logs.Add(log);
				ctx.SaveChanges();
			}
		}

		public Token? GetToken()
		{
			Token? token = null;
			using (var ctx = new PregnancyContext())
			{
				// get last token
				token = ctx.Tokens.OrderByDescending(t => t.Id).FirstOrDefault();
				if (token is { Expired: true })
				{
					// get new token
					var api  = new Api(_settings);
					Log(Models.Log.TypeEnum.Info, "Token request");
					var auth = api.Auth();

					if (!string.IsNullOrEmpty(auth))	
					{
						token = JsonSerializer.Deserialize<Token>(auth);
						if (token != null)
						{
							// save new token
							token.Created = DateTime.Now;
							ctx.Tokens.Add(token);
							ctx.SaveChanges();
							Log(Models.Log.TypeEnum.Info, "Token created");
						}
						else
						{
							Log(Models.Log.TypeEnum.Error, "Error 401 - authentication failed");
							Helper.Email("Error Pregnancy+", "Error 401 - authentication failed\n\n");
						}
					}
					else
					{
						Log(Models.Log.TypeEnum.Error, "Error 400 - connection failed");
						Helper.Email("Error Pregnancy+", "Error 400 - connection failed\n\n");
					}
				}
			}
			return token;
		}



		public bool GetRegistrations(Token token)
		{
			using (var ctx = new PregnancyContext())
			{
				var api           = new Api(_settings);
				var optinFrom = ctx.Registrations.OrderByDescending(r => r.Created).FirstOrDefault()?.Created;
				var registrations    = api.GetRegistrations(token, optinFrom);
				var listRegistration = new List<Registration>();

				if (registrations != null && registrations is { Total: > 0, Result.Count: > 0 })
				{
					foreach (var registration in registrations.Result)
					{
						// check if registration exists
						var exists = ctx.Registrations.FirstOrDefault(r => r.Email == registration.Email);
						if (exists == null)
						{
							if (registration.DueDate != null && registration.OptinDate != null)
							{
								var dueDate   = DateTime.Parse(registration.DueDate);
								var optinDate = DateTime.Parse(registration.OptinDate);

								// add registration to list
								listRegistration.Add(new Registration
								{
									Created     = DateTime.Now,
									Email       = registration.Email,
									FirstName   = registration.FirstName,
									LastName    = registration.LastName,
									DueDate     = dueDate,
									OptinDate   = optinDate,
									ConsentText = registration.ConsentText,
									Source      = registration.Source,
									Country     = registration.Country,
									Os          = registration.Os,
									Locale       = registration.Locale,
									Processed   = false
								});
							}
						}
					}

					// save new registrations
					ctx.Registrations.AddRange(listRegistration);
					ctx.SaveChanges();
					Log(Models.Log.TypeEnum.Import, $"{listRegistration.Count} Registrations imported");

					return true;
				}
				else
				{
					Log(Models.Log.TypeEnum.Info, "No records");
					//Helper.Email("Error Pregnancy+", "Error 400 - connection failed\n\n");
				}
			}

			return false;
		}


		public bool SendRegistrations()
		{
			using (var ctx = new PregnancyContext())
			{
				var registrations = ctx.Registrations.Where(r => r.Processed == false).ToList();



				// gate: APPPLUS_SETREGISTRATIONS
				var json = JsonSerializer.Serialize(registrations, _jsonoptions);

				if (!string.IsNullOrEmpty(json))
				{
					try
					{
						var obj = new GateSetRegistrations
						{
							GateInput = new GateSetRegistrationsInputObject
							{
								Data = json
							}
						};


						var gate = JsonSerializer.Serialize(obj, _jsonoptions);

						using (var campaign = new ApiSelligent(_settings))
						{
							var response = campaign.Post("sync/campaigns/TriggerbyJsonWithResult", gate);
							var respObj  = JsonSerializer.Deserialize<DtoSelligent.GeneralResponseSelligent>(response);

							if (respObj != null && respObj.IsSuccess)
							{
								var data = HttpUtility.HtmlDecode(respObj.Result.Trim('[', ']'));

								foreach (var registration in registrations)
								{
									//registration.Processed         = true;
									ctx.Entry(registration).State = EntityState.Modified;
								}

								ctx.SaveChanges();
								return true;
							}


							Log(Models.Log.TypeEnum.Error, $"Selligent: {respObj?.Error} - {json}");
							Helper.Email("Error Pregnancy+", "Error - Data error Selligent\n\n");
						}
					}
					catch (Exception e)
					{
						Log(Models.Log.TypeEnum.Error, $"Selligent: {e?.InnerException?.Message} - {json}");
						Helper.Email("Error Pregnancy+", "Error - Something got wrong, while sending data to Selligent.. \n\n");
					}
				}
			}

			return false;

		}
	}
}
