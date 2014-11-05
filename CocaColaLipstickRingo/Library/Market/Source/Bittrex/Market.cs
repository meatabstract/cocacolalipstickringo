using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Market.Source.Bittrex
{
	public class Market
	{
		public string MarketCurrency { get; set; }
		public string BaseCurrency { get; set; }
		public string MarketCurrencyLong { get; set; }
		public string BaseCurrencyLong { get; set; }
		public string MinTradeSize { get; set; }
		public string MarketName { get; set; }
		public string IsActive { get; set; }
		public string Created { get; set; }
		public string Notice { get; set; }
		public string IsSponsored { get; set; }
		public string LogoUrl { get; set; }

	}
}
