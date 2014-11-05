using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Utils.Messages;

namespace CCLR.Market.Source.Messages
{
	public class MarketsResponse : ResponseBase
	{
		public ICollection<String> Markets { get; set; }
	}
}
