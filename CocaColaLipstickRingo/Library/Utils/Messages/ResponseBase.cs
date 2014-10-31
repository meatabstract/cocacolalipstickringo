using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Utils.Messages
{
	public class ResponseBase
	{
		public ICollection<String> Errors { get; set; }
		public ICollection<String> Warnings { get; set; }
		public Boolean IsSuccess { get; set; }
	}
}
