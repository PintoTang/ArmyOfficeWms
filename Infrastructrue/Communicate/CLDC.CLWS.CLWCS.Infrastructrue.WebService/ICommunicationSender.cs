using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService
{
	public interface ICommunicationSender
	{
		string Send(string message);
	}
}
