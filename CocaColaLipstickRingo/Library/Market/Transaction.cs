using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Market
{
	public class Transaction
	{
		/// <summary>
		/// The total number of coins gained
		/// of the target currency
		/// </summary>
		public Decimal Quantity { get; set; }

		/// <summary>
		/// The price-per-coin paid
		/// </summary>
		public Decimal PricePerCoin { get; set; }

		/// <summary>
		/// The total cost (in the source currency) of 
		/// the transaction
		/// </summary>
		public Decimal Total { get; set; }

		/// <summary>
		/// The currency being used to buy the target currency
		/// </summary>
		public Currency SourceCurrency { get; set; }

		/// <summary>
		/// The currency being bought
		/// </summary>
		public Currency TargetCurrency { get; set; }
	}
}
