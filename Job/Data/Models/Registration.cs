using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Data.Models
{
	[Table("registration")]
	public class Registration
	{
		[Key]
		public int Id { get; set; }
		[Column("dt")]
		public DateTime? Created { get; set; }
		public string Email { get; set; }
		[Column("first_name")]
		public string FirstName { get; set; }
		[Column("last_name")]
		public string LastName { get; set; }
		[Column("due_date")]
		public string? DueDate { get; set; }
		[Column("optin_date")]
		public string? OptinDate { get; set; }
		[Column("consent_text")]
		public string? ConsentText { get; set; }
		public string? Source { get; set; }
		public string? Country { get; set; }
		public string? Os { get; set; }
		public string? Local { get; set; }
	}
}
