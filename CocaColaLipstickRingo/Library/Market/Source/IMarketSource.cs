using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Market.Source.Messages;

namespace CCLR.Market.Source
{
	/// <summary>
	/// Defines an interface for accessing known information
	/// about markets. Such as the last closing prices.
	/// </summary>
	public interface IMarketSource
	{
		/// <summary>
		/// Gets all transactions for a currency, within a market, over 
		/// a given time period.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		TransactionsResponse GetTransactions(TransactionsRequest request);

		/// <summary>
		/// Get all currencies for a given market
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		CurrenciesResponse GetCurrencies(GetCurrenciesRequest request);

		/// <summary>
		/// Gets all markets available to this market source
		/// </summary>
		/// <returns></returns>
		MarketsResponse GetMarkets();
	}
}
