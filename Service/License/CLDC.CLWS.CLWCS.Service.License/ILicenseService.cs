using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.License.DataModel;

namespace CLDC.CLWS.CLWCS.Service.License
{
    /// <summary>
    /// 验证码服务接口
    /// </summary>
    public interface ILicenseService
    {
        /// <summary>
        /// 获取验证码信息
        /// </summary>
        /// <returns></returns>
        OperateResult<SerialNumberDataModel> GetLicense();

        /// <summary>
        /// 获取验证码是否可用
        /// </summary>
        /// <returns></returns>
        OperateResult IsLicenseAvailable(bool isShowRegisterView);

        /// <summary>
        /// 是否需要通知过期，如果需要返回过期天数
        /// </summary>
        /// <returns></returns>
        OperateResult<int> IsNeedNoticeExpired();

        /// <summary>
        /// 开启检查验证码信息
        /// </summary>
        /// <returns></returns>
        OperateResult StartCheckLicense();

        /// <summary>
        /// 显示并且注册界面 返回注册结果
        /// </summary>
        OperateResult ShowAndRegisterLicense();

        /// <summary>
        /// 检测序列号是否可用
        /// </summary>
        /// <param name="serialNum"></param>
        /// <returns></returns>
        OperateResult CheckSerialNumIsAvailable(string serialNum);

        /// <summary>
        /// 更新注册信息
        /// </summary>
        /// <param name="serialNum"></param>
        /// <returns></returns>
        OperateResult UpdateLicense(string serialNum);


    }
}
