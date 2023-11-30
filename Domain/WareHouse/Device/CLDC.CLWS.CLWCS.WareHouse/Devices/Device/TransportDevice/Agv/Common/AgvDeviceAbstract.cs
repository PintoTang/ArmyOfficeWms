using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.Framework.CmdDataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.TransportDevice.Common.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public abstract class AgvDeviceAbstract : TransportDeviceBaseAbstract
    {
        /// <summary>
        /// 处理步骤
        /// </summary>
        /// <param name="orderId">指令ID</param>
        /// <param name="moveStep">动作</param>
        /// <returns></returns>
        public abstract OperateResult HandleMoveStepChange(int orderId, AgvMoveStepEnum moveStep);

        private AgvMoveStepEnum _curMoveStep = AgvMoveStepEnum.Free;
        /// <summary>
        /// 当前动作
        /// </summary>

        public AgvMoveStepEnum CurMoveStep
        {
            get { return _curMoveStep; }

            set
            {
                if (_curMoveStep != value)
                {
                    _curMoveStep = value;
                }
            }
        }
    }
}
