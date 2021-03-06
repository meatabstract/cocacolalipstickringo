﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CCLR.Market.Source;

namespace CCLR.Analysis.Metric
{
	/// <summary>
	/// Defines an interface for a class that represents as a single numerical metric
	/// ranging from -100 to 100, with -100 being poor health, 0 being neutral and 100 being good health
	/// </summary>
	public interface IAnalysisMetric
	{
		Int32 Evaluate(IMarketSource marketSource, String currencyCode, String marketCode);
	}
}
