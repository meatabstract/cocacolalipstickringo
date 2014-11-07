using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Market.Trader.Messages;

namespace CCLR.Market.Trader
{
	/// <summary>
	/// Interfaces with the market, provides all methods to buy and sell coins
	/// </summary>
	interface IMarketTrader
	{
		/// <summary>
		/// Request the current balance of a given coin.
		/// </summary>
		/// <param name="coin"></param>
		/// <returns></returns>
		Decimal RequestBalance(String coin);

		/// <summary>
		/// Request a transaction be made
		/// </summary>
		/// <param name="request">A transaction request describing the transaction to be made</param>
		/// <returns>A transaction response detailing the success, error and/or warnings</returns>
		TransactionResponse RequestTransaction(TransactionRequest request);
	}
}
