using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Utils.Messages;

namespace CCLR.Market.Trader.Messages
{
	public class TransactionResponse : ResponseBase
	{
		public Transaction Transaction { get; set; }
	}
}
