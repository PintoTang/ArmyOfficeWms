using System;
using System.Collections.Generic;
using System.Xml;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.ViewModel
{
    public class DeviceParamsDisPlayViewModel : DeviceDataBase
    {
        #region 属性
        private bool isNeedClearScreen = true;
        public bool IsNeedClearScreen
        {
            get { return isNeedClearScreen; }
            set
            {
                isNeedClearScreen = value;
                RaisePropertyChanged("IsNeedClearScreen");
            }
        }
        private string clearScreenInterval = "2";
        public string ClearScreenInterval
        {
            get { return clearScreenInterval; }
            set
            {
                clearScreenInterval = value;
                RaisePropertyChanged("ClearScreenInterval");
            }
        }
        private string defaultContent = "欢迎光临";
        public string DefaultContent
        {
            get { return defaultContent; }
            set
            {
                defaultContent = value;
                RaisePropertyChanged("DefaultContent");
            }
        }
        private string defaultTitle = "默认的标题";
        public string DefaultTitle
        {
            get { return defaultTitle; }
            set
            {
                defaultTitle = value;
                RaisePropertyChanged("DefaultTitle");
            }
        }

        #endregion

        private Dictionary<string, string> dicIsNeedClearScreen = new Dictionary<string, string>();
        public Dictionary<string, string> DicIsNeedClearScreen
        {
            get
            {
                if (dicIsNeedClearScreen.Count == 0)
                {
                    dicIsNeedClearScreen.Add("True", "是");
                    dicIsNeedClearScreen.Add("False", "否");
                }
                return dicIsNeedClearScreen;
            }
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

        private DisplayDeviceViewModel DeviceViewModel { get; set; }
        /// <summary>
        /// 上一个对象 传递过来的 DataContext
        /// </summary>
        /// <param name="PreDataContext"></param>
        public DeviceParamsDisPlayViewModel(object PreDataContext)
        {
            DeviceViewModel = PreDataContext as DisplayDeviceViewModel;
            XmlNodeName="DisplayDevices";
            DeviceId = DeviceViewModel.Id;

            LoadXmlConfig();
            LoadXml_templateZero();
        }
        private void LoadXmlConfig()
        {
            string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
            XmlNodeList xmlNodelist = DeviceConfigManage.Instance.xmlDoc.SelectNodes(strPath);

            XmlNode configxmlNode = xmlNodelist[0].SelectSingleNode("Config");

            string strIsNeedClearScreen = configxmlNode.SelectSingleNode("IsNeedClearScreen").FirstChild.Value;
            if (!string.IsNullOrEmpty(strIsNeedClearScreen))
            {
                IsNeedClearScreen = strIsNeedClearScreen.Trim().ToUpper().Equals("TRUE");
            }
            ClearScreenInterval = configxmlNode.SelectSingleNode("ClearScreenInterval").FirstChild.Value;
            DefaultTitle = configxmlNode.SelectSingleNode("DefaultTitle").FirstChild.Value;
            DefaultContent = configxmlNode.SelectSingleNode("DefaultContent").FirstChild.Value;
        }



        /// <summary>
        /// 保存 （1、本地内存和xml）
        /// </summary>
        /// <param name="vmObj">对象ViewModel</param>
        public OperateResult Save(object vmObj)
        {
            OperateResult opResult = OperateResult.CreateFailedResult();
            try
            {
                DisplayDeviceViewModel deviceViewModel = vmObj as DisplayDeviceViewModel;
                deviceViewModel.Device.IsNeedClearScreen = IsNeedClearScreen;
                deviceViewModel.Device.ClearScreenInterval = float.Parse(ClearScreenInterval);
                deviceViewModel.Device.CurTitleContent = DefaultTitle;
                deviceViewModel.Device.DefaultContent = DefaultContent;
                SaveXml_Config();
                SaveXml_templateZero();
                opResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                opResult.IsSuccess = false;
                opResult.Message = ex.ToString();
            }
            return opResult;
        }

        /// <summary>
        /// 保存xml文件
        /// </summary>
        private void SaveXml_Config()
        {
            XmlElement xe = DeviceConfigManage.Instance.xmlDoc.DocumentElement;
            string strPath = string.Format("/Configuration/Devices/{0}/Device[@DeviceId=\"{1}\"]", XmlNodeName, DeviceId);
            XmlNode parentxmlNode = xe.SelectSingleNode(strPath);

            //Config
            XmlNode configXmlNode = parentxmlNode.SelectSingleNode("Config");
            XmlElement configSelectXe = (XmlElement)configXmlNode;
            string strIsNeedClearScreen = IsNeedClearScreen ? "True" : "False";
            configSelectXe.GetElementsByTagName("IsNeedClearScreen").Item(0).InnerText = strIsNeedClearScreen;
            configSelectXe.GetElementsByTagName("ClearScreenInterval").Item(0).InnerText = ClearScreenInterval;
            configSelectXe.GetElementsByTagName("DefaultTitle").Item(0).InnerText = DefaultTitle;
            configSelectXe.GetElementsByTagName("DefaultContent").Item(0).InnerText = DefaultContent;
            DeviceConfigManage.Instance.xmlDoc.Save(DeviceConfigManage.Instance.xmlFullPath);//保存的路径 新路径 To Do
        }

    }
}
