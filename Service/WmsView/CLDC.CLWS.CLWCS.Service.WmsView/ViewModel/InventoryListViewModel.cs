using CL.WCS.SystemConfigPckg.Model;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.Sockets;
using CLDC.CLWS.CLWCS.Service.WmsView.DataModel;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;
using GalaSoft.MvvmLight;
using Infrastructrue.Ioc.DependencyFactory;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Windows;

namespace CLDC.CLWS.CLWCS.Service.WmsView.ViewModel
{
    /// <summary>
    /// 库存管理VM
    /// </summary>
    public class InventoryListViewModel : ViewModelBase
    {
        private string _curMaterial = string.Empty;
        private string _curArea;
        private string _curTeam;
        private InvStatusEnum? _curInvStatus;
        private string _curTaskType;
        private int? _totalQty = 0;
        private WmsDataService _wmsDataService;
        public ObservableCollection<Inventory> InventoryList { get; set; }
        public ObservableCollection<Area> AreaList { get; set; }
        public ObservableCollection<AreaTeam> TeamList { get; set; }

        /// <summary>
        /// 当前搜索的装备
        /// </summary>
        public string CurMaterial
        {
            get { return _curMaterial; }
            set
            {
                _curMaterial = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的区域
        /// </summary>
        public string CurArea
        {
            get { return _curArea; }
            set
            {
                _curArea = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的分队
        /// </summary>
        public string CurTeam
        {
            get { return _curTeam; }
            set
            {
                _curTeam = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的库存状态
        /// </summary>
        public InvStatusEnum? CurInvStatus
        {
            get { return _curInvStatus; }
            set
            {
                _curInvStatus = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前搜索的任务分类
        /// </summary>
        public string CurTaskType
        {
            get { return _curTaskType; }
            set
            {
                _curTaskType = value;
                RaisePropertyChanged();
            }
        }
        public int? TotalQty
        {
            get { return _totalQty; }
            set
            {
                _totalQty = value;
                RaisePropertyChanged();
            }
        }

        private readonly Dictionary<InvStatusEnum, string> _invStatusDict = new Dictionary<InvStatusEnum, string>();
        public Dictionary<InvStatusEnum, string> InvStatusDict
        {
            get
            {
                if (_invStatusDict.Count == 0)
                {
                    foreach (var value in Enum.GetValues(typeof(InvStatusEnum)))
                    {
                        InvStatusEnum em = (InvStatusEnum)value;
                        _invStatusDict.Add(em, em.GetDescription());
                    }
                }
                return _invStatusDict;

            }
        }

        public InventoryListViewModel()
        {
            InventoryList = new ObservableCollection<Inventory>();
            AreaList = new ObservableCollection<Area>();
            TeamList = new ObservableCollection<AreaTeam>();
            _wmsDataService = DependencyHelper.GetService<WmsDataService>();
            InitCbArea(); InitTeam();
        }

        private void InitCbArea()
        {
            AreaList.Clear();
            try
            {
                List<Area> accountListResult = _wmsDataService.GetAreaList(string.Empty);
                if (accountListResult.Count > 0)
                {
                    accountListResult.ForEach(ite => AreaList.Add(ite));
                }
            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private void InitTeam()
        {
            TeamList.Clear();
            {
                for (int i = 1; i < 4; i++)
                {
                    AreaTeam team = new AreaTeam();
                    team.Id = i; team.Name = i + "排"; team.Remark = string.Empty;
                    TeamList.Add(team);
                }
                TeamList.Add(new AreaTeam { Id = 4, Name = "首长机关" });
                TeamList.Add(new AreaTeam { Id = 5, Name = "民兵" });
            }
        }

        private RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(Search);
                }
                return _searchCommand;
            }
        }

        private void Search()
        {
            if (ValidData.CheckSearchParmsLenAndSpecialCharts(CurMaterial))
            {
                MessageBoxEx.Show("输入的字符长度超过20或包含特殊字符，请重新输入!");
                return;
            }
            InventoryList.Clear();
            TotalQty = 0;
            try
            {
                var where = CombineSearchSql();
                OperateResult<List<Inventory>> accountListResult = _wmsDataService.GetInventoryPageList(sqlWhere);
                if (!accountListResult.IsSuccess)
                {
                    SnackbarQueue.MessageQueue.Enqueue("查询出错：" + accountListResult.Message);
                    return;
                }
                if (accountListResult.Content != null && accountListResult.Content.Count > 0)
                {
                    accountListResult.Content.ForEach(ite =>
                    {
                        InventoryList.Add(ite);
                        TotalQty += ite.Qty;
                    });
                }




            }
            catch (Exception ex)
            {
                SnackbarQueue.MessageQueue.Enqueue("查询异常：" + ex.Message);
            }
        }

        private string sqlWhere;
        private Expression<Func<Inventory, bool>> CombineSearchSql()
        {
            sqlWhere = string.Empty;
            Expression<Func<Inventory, bool>> whereLambda = t => t.IsDeleted == false;
            if (!string.IsNullOrEmpty(CurMaterial))
            {
                whereLambda = whereLambda.AndAlso(t => t.MaterialDesc.Contains(CurMaterial));
            }
            if (!string.IsNullOrEmpty(CurArea))
            {
                whereLambda = whereLambda.AndAlso(t => t.AreaCode == CurArea);
                sqlWhere += " And AreaCode='" + CurArea+"'";
            }
            if (!string.IsNullOrEmpty(CurTeam))
            {
                whereLambda = whereLambda.AndAlso(t => t.AreaTeam == CurTeam);
                sqlWhere += " And AreaTeam='" + CurTeam+"'";
            }
            if (CurInvStatus.HasValue)
            {
                whereLambda = whereLambda.AndAlso(t => t.Status == CurInvStatus.Value);
                sqlWhere += " And Status='" + CurInvStatus.Value+"'";
            }
            return whereLambda;
        }

        private RelayCommand _exportCommand;
        public RelayCommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(ExportToExcel);
                }
                return _exportCommand;
            }
        }
        public void ExportToExcel()
        {
            var where = CombineSearchSql();
            OperateResult<List<Inventory>> accountListResult = _wmsDataService.GetInventoryPageList(sqlWhere);
            if (!accountListResult.IsSuccess)
            {
                SnackbarQueue.MessageQueue.Enqueue("导出异常：" + accountListResult.Message);
                return;
            }
            if (accountListResult.Content != null && accountListResult.Content.Count > 0)
            {
                var list = accountListResult.Content;
                //第一步：创建Excel对象
                NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                //第二步：创建Excel对象的工作簿
                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet();
                sheet.SetColumnWidth(0, 5 * 256);
                sheet.SetColumnWidth(1, 15 * 256);
                sheet.SetColumnWidth(2, 10 * 256);
                sheet.SetColumnWidth(3, 30 * 256);
                sheet.SetColumnWidth(4, 20 * 256);
                sheet.SetColumnWidth(6, 20 * 256);
                sheet.DefaultRowHeight = 20 * 20;

                //第三步：Excel表头设置
                //给sheet添加第一行的头部标题
                NPOI.SS.UserModel.IRow row1 = sheet.CreateRow(0);//创建行
                row1.CreateCell(0).SetCellValue("序号");
                row1.CreateCell(1).SetCellValue("任务分类");
                row1.CreateCell(2).SetCellValue("装备名称");
                row1.CreateCell(3).SetCellValue("装备标签");
                row1.CreateCell(4).SetCellValue("出入库事由");
                row1.CreateCell(5).SetCellValue("状态");
                row1.CreateCell(6).SetCellValue("时间");
                //第四步：for循环给sheet的每行添加数据
                for (int i = 0; i < list.Count; i++)
                {
                    NPOI.SS.UserModel.IRow row = sheet.CreateRow(i + 1);
                    row.CreateCell(0).SetCellValue(i + 1);
                    row.CreateCell(1).SetCellValue(list[i].AreaName);
                    row.CreateCell(2).SetCellValue(list[i].MaterialDesc);
                    row.CreateCell(3).SetCellValue(list[i].Barcode);
                    row.CreateCell(4).SetCellValue(list[i].Reason);
                    row.CreateCell(5).SetCellValue(list[i].Status.ToString());
                    row.CreateCell(6).SetCellValue(list[i].CreatedTime.ToString());
                }

                //把Excel转化为文件流，输出
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "选择要保存的路径";
                saveFileDialog.Filter = "Excel文件|*.xls|所有文件|*.*";
                saveFileDialog.FileName = string.Empty;
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = "xls";
                saveFileDialog.CreatePrompt = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    FileStream BookStream = new FileStream(saveFileDialog.FileName.ToString(), FileMode.Create, FileAccess.Write);//定义文件流
                    book.Write(BookStream);//将工作薄写入文件流                  
                    BookStream.Seek(0, SeekOrigin.Begin); //输出之前调用Seek（偏移量，游标位置）方法：获取文件流的长度
                    BookStream.Close();
                }
                else
                {
                    MessageBox.Show("导出保存失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private GalaSoft.MvvmLight.Command.RelayCommand<string> _soundLightCommand;
        public GalaSoft.MvvmLight.Command.RelayCommand<string> SoundLightCommand
        {
            get
            {
                if (_soundLightCommand == null)
                {
                    _soundLightCommand = new GalaSoft.MvvmLight.Command.RelayCommand<string>(SendTcpCommand);
                }
                return _soundLightCommand;
            }
        }
        private void SendTcpCommand(string area)
        {
            try
            {
                if(string.IsNullOrEmpty(area))
                {
                    MessageBoxEx.Show("未选择任务分类，不能定位!");
                    return;
                }

                TcpCom tcp = new TcpCom();
                tcp.RemoteIp = IPAddress.Parse(SystemConfig.Instance.RemoteIp);
                tcp.RemoteIpPort = 20108;
                if (tcp.Connected == false)
                {
                    tcp.Connect();
                }
                if (tcp.Connected == false)
                {
                    SnackbarQueue.MessageQueue.Enqueue("TCP连接失败！");
                }

                var command = SoundLightConfig.Instance.CommandList.FirstOrDefault(x => x.Area == area);
                if (command == null)
                {
                    MessageBoxEx.Show("未配置此区域的声光报警指令，请重新配置!");
                    return;
                }
                else
                {
                    string[] strCode=command.Code.Split(' ');
                    byte[] buffer = new byte[strCode.Length];
                    buffer = ToBytesFromHexString(command.Code);
                    tcp.Send(buffer);
                    Thread.Sleep(250);
                    tcp.Send(buffer);
                }
            }
            catch { }
        }


        /// <summary>
        /// 16进制格式字符串转字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] ToBytesFromHexString(string hexString)
        {
            //以 ' ' 分割字符串，并去掉空字符
            string[] chars = hexString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] returnBytes = new byte[chars.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < chars.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(chars[i], 16);
            }
            return returnBytes;
        }

    }
}
