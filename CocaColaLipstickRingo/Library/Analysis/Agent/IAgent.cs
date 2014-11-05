using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLR.Agent
{
	/// <summary>
	/// Defines an agent which analyses a number of metrics and 
	/// issues buy/sell commands
	/// </summary>
	public interface IAgent
	{
		void Process();
	}
}
