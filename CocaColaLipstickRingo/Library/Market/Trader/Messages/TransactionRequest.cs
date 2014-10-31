using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Market.Trader.Messages
{
	public class TransactionRequest
	{
		public Decimal Quantity { get; set; }
		public Currency TargetCurrency { get; set; }
		public Currency SourceCurrency { get; set; }
	}
}
