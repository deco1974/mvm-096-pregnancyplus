using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
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
}
