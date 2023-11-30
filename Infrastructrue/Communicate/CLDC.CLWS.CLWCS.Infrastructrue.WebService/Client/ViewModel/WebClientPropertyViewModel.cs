using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.Common;
using CLDC.CLWS.CLWCS.Service.Authorize;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.Infrastructrue.WebService.Client.ViewModel
{
    public class WebClientPropertyViewModel : ViewModelBase
    {
        public WebClientCommunicationProperty DataModel { get; set; }
        public WebClientPropertyViewModel(WebClientCommunicationProperty dataModel)
        {
            DataModel = dataModel;
        }

        public RoleLevelEnum CurUserLevel
        {
            get
            {
                if (CookieService.CurSession != null)
                {
                    return CookieService.CurSession.RoleLevel;
                }
                else
                {
                    return RoleLevelEnum.游客;
                }
            }
        }

        private readonly Dictionary<CommunicationModeEnum, string> _communicationModeDic = new Dictionary<CommunicationModeEnum, string>();
        public Dictionary<CommunicationModeEnum, string> CommunicationModeDic
        {
            get
            {
                if (_communicationModeDic.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(CommunicationModeEnum)))
                    {
                        CommunicationModeEnum em = (CommunicationModeEnum)value;
                        _communicationModeDic.Add(em, em.GetDescription());
                    }
                }
                return _communicationModeDic;

            }
        }     

    }
}
