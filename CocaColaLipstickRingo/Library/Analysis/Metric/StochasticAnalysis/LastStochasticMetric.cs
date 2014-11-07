using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Market.Source;

namespace CCLR.Analysis.Metric.StochasticAnalysis
{
	public class LastStochasticMetric : IAnalysisMetric
	{
		public int Evaluate(IMarketSource marketSource, String currencyCode, String marketCode)
		{
			throw new NotImplementedException();
		}


		private Decimal GetLastClosingPrice()
		{
			throw new NotImplementedException();
		}


		private Dictionary<DateTime, Decimal> GetLanesStochastic(Int32 periodCount, TimeSpan periodLength)
		{
			throw new NotImplementedException();
		}


		private Decimal GetLanesStochasticForPeriod(Decimal lastClosingPrice, DateTime start, TimeSpan periodLength)
		{
			ICollection<Decimal> prices = GetPricesInPeriod(start, periodLength);
			Decimal high = GetHighestPriceInPeriod(prices);
			Decimal low = GetLowestPriceInPeriod(prices);

			Decimal lanes = CalculateLanesStochastic(lastClosingPrice, low, high);

			return lanes;
		}

		private ICollection<Decimal> GetPricesInPeriod(DateTime start, TimeSpan periodLength)
		{
			List<Decimal> prices = new List<Decimal>();

			throw new NotImplementedException();

			return prices;
		}

		private Decimal GetHighestPriceInPeriod(ICollection<Decimal> prices)
		{
			Decimal high = 0;

			foreach (Decimal price in prices)
			{
				high = Math.Max(price, high);
			}

			return high;
		}

		private Decimal GetLowestPriceInPeriod(ICollection<Decimal> prices)
		{
			Decimal low = 0;

			foreach (Decimal price in prices)
			{
				low = Math.Min(price, low);
			}

			return low;
		}

		private Decimal CalculateLanesStochastic(Decimal latestClosingPrice, Decimal periodLowPrice, Decimal periodHighPrice)
		{
			Decimal lanesStochastic = ((latestClosingPrice - periodLowPrice) / (periodHighPrice - periodLowPrice)) * 100;
			return lanesStochastic;
		}



	}
}
