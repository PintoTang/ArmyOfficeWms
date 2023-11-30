using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CL.Framework.OPCClientSimulatePckg
{
	public class OPCClientSimulateFactory : OPCClientSimulateAbstract
	{

		public OPCReadOrWriteAbstract Make(OPCClientSimulate opc)
		{
			OPCReadOrWriteAbstract opCReadOrWriteAbstract = new OPCReadOrWriteForm(opc);
			return opCReadOrWriteAbstract;
		}
	}
}
