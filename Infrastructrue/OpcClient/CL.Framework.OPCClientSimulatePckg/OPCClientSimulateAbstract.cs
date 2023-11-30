using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace CL.Framework.OPCClientSimulatePckg
{
	public interface OPCClientSimulateAbstract
	{
		OPCReadOrWriteAbstract Make(OPCClientSimulate opc);
	}
}
