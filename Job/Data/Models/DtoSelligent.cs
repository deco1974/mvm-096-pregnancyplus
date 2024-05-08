using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Data.Models
{
	public class DtoSelligent
	{
		public class GeneralResponseSelligent
		{
			public bool IsSuccess { get; set; }
			public string Error { get; set; }
			public string Result { get; set; }
		}
	}
}
