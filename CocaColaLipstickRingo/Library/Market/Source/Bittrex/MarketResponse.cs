using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Market.Source.Bittrex
{
	class MarketResponse
	{
		public bool success { get; set; }
		public string message { get; set; }
		public List<Market> result { get; set; }
	}
}
