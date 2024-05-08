using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Job.Data.Models
{
	public class ApiRegistration
	{
		[JsonPropertyName("optin_date_from")]
		public string? OptinDateFrom { get; set; }
		[JsonPropertyName("optin_date_to")]
		public string? OptinDateTo { get; set; }
		[JsonPropertyName("limit")]
		public int? Limit { get; set; }
		[JsonPropertyName("page")]
		public int? Page { get; set; }

		public class Response
		{
			[JsonPropertyName("total")]
			public int? Total { get; set; }
			[JsonPropertyName("currentpage")]
			public int? Currentpage { get; set; }
			[JsonPropertyName("limit")]
			public int? Limit { get; set; }
			[JsonPropertyName("result")]
			public List<Registration>? Result { get; set; }

			public class Registration
			{
				[JsonPropertyName("email")]
				public string Email { get; set; }
				[JsonPropertyName("first_name")]
				public string FirstName { get; set; }
				[JsonPropertyName("last_name")]
				public string LastName { get; set; }
				[JsonPropertyName("due_date")]
				public string? DueDate { get; set; }
				[JsonPropertyName("optin_date")]
				public string? OptinDate { get; set; }
				[JsonPropertyName("consent_text")]
				public string? ConsentText { get; set; }
				[JsonPropertyName("source")]
				public string? Source { get; set; }
				[JsonPropertyName("country")]
				public string? Country { get; set; }
				[JsonPropertyName("os")]
				public string? Os { get; set; }
				[JsonPropertyName("locale")]
				public string? Locale { get; set; }
			}
		}
	}
}
