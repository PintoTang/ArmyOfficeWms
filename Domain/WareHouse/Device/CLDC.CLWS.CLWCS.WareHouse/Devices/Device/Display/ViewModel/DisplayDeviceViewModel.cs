using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.ViewModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Display.ViewModel
{
    public class DisplayDeviceViewModel : DeviceViewModelAbstract<DisplayDeviceAbstract>
	{
        public DisplayDeviceViewModel(DisplayDeviceAbstract device) : base(device)
        {
            
        }

       

        private void ClearPackageMsg()
		{
			lock (PackageLst)
			{
				if (PackageLst.Count >= 100)
				{
					PackageLst.RemoveAt(0);
				}
			}
		}

		public void ShowPackage(Package package)
		{
			if (System.Windows.Application.Current.Dispatcher != null)
				System.Windows.Application.Current.Dispatcher.Invoke(new Action<Package>(p =>
				{
					CurrentPackage = p;
					ClearPackageMsg();
					PackageLst.Add(p);
				}),DispatcherPriority.Background,package);
		}


		private ObservableCollection<Package> packageLst = new ObservableCollection<Package>();

		public ObservableCollection<Package> PackageLst
		{
			get
			{
				return packageLst;
			}
			set
			{
				packageLst = value;
				RaisePropertyChanged("PackageLst");
			}
		}

		private Package currentPackage;
		public Package CurrentPackage
		{
			get
			{
				return currentPackage;
			}
			set
			{
				if (!currentPackage.PackageId.Equals(value.PackageId))
				{
					currentPackage = value;
					RaisePropertyChanged("CurrentPackage");
				}
			}
		}
        private string _CurSendContent = "test SendContent";
        /// <summary>
        /// 发送的内容
        /// </summary>
        public string CurSendContent
        {
            get { return _CurSendContent; }
            set
            {
                _CurSendContent = value;
                RaisePropertyChanged("CurSendContent");
            }
        }
        private string _CurReceiveContent = "test ReceiveContent";
        /// <summary>
        /// 接收的内容
        /// </summary>
        public string CurReceiveContent
        {
            get { return _CurReceiveContent; }
            set
            {
                _CurReceiveContent = value;
                RaisePropertyChanged("CurReceiveContent");
            }
        }

        private MyCommand _cmdClearScreen;
        /// <summary>
        /// 清屏
        /// </summary>
        public MyCommand CmdClearScreen
        {
            get
            {
                if (_cmdClearScreen == null)
                    _cmdClearScreen = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              OperateResult opResult = Device.ClearScreen();
                              if(opResult.IsSuccess)
                              {
                                  CurReceiveContent ="清理成功!";
                              }
                              else
                              {
                                  CurReceiveContent = opResult.Message;
                              }
                          }));
                return _cmdClearScreen;
            }
        }

        private MyCommand _cmdSendTitle;
        /// <summary>
        /// 发送标题
        /// </summary>
        public MyCommand CmdSendTitle
        {
            get
            {
                if (_cmdSendTitle == null)
                    _cmdSendTitle = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              OperateResult opResult = Device.SendTitle(CurSendContent);
                              if (opResult.IsSuccess)
                              {
                                  CurReceiveContent = "发送标题 成功!";
                              }
                              else
                              {
                                  CurReceiveContent = opResult.Message;
                              }
                          }));
                return _cmdSendTitle;
            }
        }
        private MyCommand _cmdSendContent;
        /// <summary>
        /// 发送内容
        /// </summary>
        public MyCommand CmdSendContent
        {
            get
            {
                if (_cmdSendContent == null)
                    _cmdSendContent = new MyCommand(new Action<object>
                        (
                          o =>
                          {
                              OperateResult opResult = Device.SendContent(CurSendContent);
                              if (opResult.IsSuccess)
                              {
                                  CurReceiveContent = "发送内容 成功!";
                              }
                              else
                              {
                                  CurReceiveContent = opResult.Message;
                              }
                          }));
                return _cmdSendContent;
            }
        }


		public override void NotifyAttributeChange(string attributeName, object newValue)
		{
			if (attributeName.Equals("CurrentPackage"))
			{
				Package package = (Package)newValue;
				ShowPackage(package);
			}
		}
	}
}
