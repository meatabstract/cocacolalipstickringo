using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Utils.Messages;

namespace CCLR.Market.Source.Messages
{
	public class CurrenciesResponse : ResponseBase
	{
		public ICollection<String> CurrencyCodes { get; set; }
	}
}
