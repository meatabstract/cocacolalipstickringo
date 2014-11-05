﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Market.Source.Messages
{
	public class TransactionsRequest
	{
		public String CurrencyCode { get; set; }
		public String Market { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Int32 Limit { get; set; }
	}
}
