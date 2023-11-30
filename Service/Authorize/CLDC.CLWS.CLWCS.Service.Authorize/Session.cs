using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;

namespace CLDC.CLWS.CLWCS.Service.Authorize
{
    /// <summary>
    /// 系统登陆Session信息
    /// </summary>
    public class Session
    {
        public Session()
        {
            SessionId = Guid.NewGuid().ToString("N");
            CreateTime = DateTime.Now;
        }
        public string SessionId { get; set; }

        UserInformationMode _userInfo = new UserInformationMode();
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInformationMode UserInfo
        {
            get { return _userInfo; }
            set { _userInfo = value; }
        }

        public RoleLevelEnum RoleLevel
        {
            get { return _userInfo.Account.RoleLevel.GetValueOrDefault(); }
        }
        /// <summary>
        /// Session的创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
