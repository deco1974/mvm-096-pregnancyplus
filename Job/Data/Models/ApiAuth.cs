using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Job.Data.Models
{
	public class ApiAuth
	{
		[JsonPropertyName("audience")]
		public string? Audience { get; set; }
		[JsonPropertyName("grant_type")]
		public string GrantType { get; set; }
		[JsonPropertyName("client_id")]
		public string? ClientId { get; set; }
		[JsonPropertyName("client_secret")]
		public string? ClientSecret { get; set; }
	}
}
