using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Utils.Messages;

namespace CCLR.Market.Source.Messages
{
	public class TransactionsResponse : ResponseBase
	{
		public ICollection<Transaction> Transactions { get; set; }
	}
}
