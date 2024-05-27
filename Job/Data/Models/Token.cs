using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Job.Data.Models
{
	[Table("token")]
	public class Token
	{
		[Key]
		public int Id { get; set; }

		[Column("dt")]
		public DateTime Created { get; set; }

		[NotMapped]
		[JsonPropertyName("scope")]
		public string? Scope { get; set; }

		[NotMapped]
		[JsonPropertyName("token_type")]
		public string? TokenType { get; set; }

		[Column("expire")]
		[JsonPropertyName("expires_in")]
		public int? ExpiresIn { get; set; }

		[Column("bearer")]
		[JsonPropertyName("access_token")]
		public string? AccessToken { get; set; }

		[Column("live")]
		[JsonIgnore]
		public bool Live { get; set; }

		[NotMapped]
		public bool Expired => Created.AddSeconds(Convert.ToDouble(ExpiresIn)) < DateTime.Now;
	}
}
