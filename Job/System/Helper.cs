using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Job.System
{
	public class Helper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="bodytext"></param>
		/// <param name="toEmail"></param>
		/// <param name="ccEmail"></param>
		public static void Email(string subject, string bodytext, string? toEmail = null, string? ccEmail = null)
		{
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddEnvironmentVariables()
				.Build();
			var settings = config.GetRequiredSection("Settings");

			try
			{

                var message = new MailMessage();
                var smtp    = new SmtpClient();

                message.From = new MailAddress(settings["ErrorEmailFrom"] ?? string.Empty);
                message.To.Add((!string.IsNullOrEmpty(toEmail)) ? new MailAddress(toEmail) : new MailAddress(settings["ErrorEmailTo"] ?? string.Empty));
                if (!string.IsNullOrEmpty(settings["ErrorEmailCC"]))
                    message.CC.Add((!string.IsNullOrEmpty(ccEmail)) ? new MailAddress(ccEmail) : new MailAddress(settings["ErrorEmailCC"] ?? string.Empty));
                message.Subject            = subject;
                message.IsBodyHtml         = true;
                message.Body               = bodytext;
                smtp.Port                  = 25;
                smtp.Host                  = settings["SMTP"] ?? string.Empty;
                smtp.EnableSsl             = false;
                smtp.UseDefaultCredentials = true;
                smtp.DeliveryMethod        = SmtpDeliveryMethod.Network;

                smtp.Send(message);

			}
			catch (Exception e)
			{
				Console.WriteLine(e.InnerException);
			}
		}



	}

	static class ExtensionMethods
	{
		public static string? ToHex(this byte[]? data)
		{
			if (data == null)
			{
				return null;
			}

			var chars = new char[checked(data.Length * 2)];
			for (var index = 0; index < data.Length; ++index)
			{
				var num = data[index];
				chars[2 * index]       = ((byte)((uint)num >> 4)).NibbleToHex();
				chars[(2 * index) + 1] = ((byte)(num & 15U)).NibbleToHex();
			}

			return new string(chars);
		}

		private static char NibbleToHex(this byte nibble)
		{
			return (int)nibble < 10 ? (char)(nibble + 48) : (char)(nibble - 10 + 65);
		}

	}


	public class LowerCaseNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName(string name)
		{
			if (string.IsNullOrEmpty(name) || !char.IsUpper(name[0]))
				return name;

			if (name.Length >= 1)
			{
				return char.ToLowerInvariant(name[0]) + name[1..];
			}

			return name.ToLower();
		}
	}



	public class UpperCaseNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName(string name)
		{
			if (string.IsNullOrEmpty(name) || !char.IsLower(name[0]))
				return name;

			if (name.Length >= 1)
			{
				return char.ToUpperInvariant(name[0]) + name[1..];
			}

			return name.ToUpper();
		}
	}
}
