using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CL.Framework.OPCClientAbsPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.OperateLog;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel.ViewModel
{
    public class OpcElementViewModel : ViewModelBase
    {
        public Action RefreshAllData;
        public Func<DataBlockNameEnum, object, OperateResult> Write;
       
        private double _opcElementViewHeight = 50.0;
        private OpcElement _deviceOpcElement;

        public double OpcElementViewHeight
        {
            get { return _opcElementViewHeight; }
            set
            {
                _opcElementViewHeight = value;
                RaisePropertyChanged();
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
        public OpcElement DeviceOpcElement
        {
            get { return _deviceOpcElement; }
            set
            {
                _deviceOpcElement = value;
                RaisePropertyChanged();
            }
        }

        public MyCommand _refreshDataCommand;
        public MyCommand RefreshDataCommand
        {
            get
            {
                if (_refreshDataCommand == null)
                {
                    _refreshDataCommand = new MyCommand(RefreshData);
                }
                return _refreshDataCommand;
            }
        }
        private async void RefreshData(object obj)
        {
            await RefreshDataAsyn();
        }

        private Task RefreshDataAsyn()
        {
            var result = Task.Run(() =>
            {
                if (RefreshAllData == null)
                {
                    SnackbarQueue.MessageQueue.Enqueue("RefreshAllData 方法没有注册");
                    return;
                }
                RefreshAllData();
                SnackbarQueue.MessageQueue.Enqueue("刷新数据成功");
            });
            return result;
        }


        private MyCommand _updateDataCommand;
        public MyCommand UpdateDataCommand
        {
            get
            {
                if (_updateDataCommand == null)
                {
                    _updateDataCommand = new MyCommand(UpdateData);
                }
                return _updateDataCommand;
            }
        }

        private Task UpdateDataAsyn(Datablock datablock)
        {
            var result = Task.Run(() =>
            {
                if (Write == null)
                {
                    SnackbarQueue.MessageQueue.Enqueue("Write 方法没有注册");
                    return;
                }
                OperateResult updateResult = Write(datablock.DatablockEnum, datablock.UpdateValue);
                string msg = string.Empty;
                if (updateResult.IsSuccess)
                {
                    msg = "更新成功";
                }
                else
                {
                    msg = string.Format("更新失败：{0} ", updateResult.Message);
                }
                SnackbarQueue.MessageQueue.Enqueue(msg);


            });
            return result;
        }

        private async void UpdateData(object obj)
        {
            if (obj is Datablock)
            {
                Datablock db = obj as Datablock;
                MessageBoxResult mbr = MessageBoxEx.Show(string.Format("确定更新为：{0}  ？", db.UpdateValue), "警告", MessageBoxButton.YesNo);
                if (mbr.Equals(MessageBoxResult.No))
                {
                    return;
                }
                string recordMsg = string.Format("把 {0} 的值由：{2} 更新为：{1}",db.RealDataBlockAddr,db.UpdateValue,db.RealValue);
                OperateLogHelper.Record(recordMsg);
                await UpdateDataAsyn(db);
            }
        }



    }
}
