using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Data.Models
{
	[Table("log")]
	public class Log
	{
		public int Id { get; set; }

		[Column("dt")] public DateTime? Created { get; set; }
		[Column("type")] public TypeEnum Type { get; set; }
		public string? Remark { get; set; }


		public enum TypeEnum
		{
			Info = 0,
			Import = 1,
			Error = 2
		}
	}
}
