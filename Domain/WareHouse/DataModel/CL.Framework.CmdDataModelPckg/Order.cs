using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using SqlSugar;

namespace CL.Framework.CmdDataModelPckg
{
    /// <summary>
    /// 指令信息类。与协议一一对应。
    /// </summary>
    public class Order : ViewModelBase, IManageable
    {
        /// <summary>
        /// 指令优先级
        /// </summary>
        [SugarColumn(ColumnName = "Priority", DefaultValue = "0", IsNullable = false, ColumnDescription = "指令优先级，默认都是0")]
        public int OrderPriority
        {
            get { return _orderPriority; }
            set
            {
                if (value != _orderPriority)
                {
                    _orderPriority = value;
                    RaisePropertyChanged("OrderPriority");

                }
            }
        }

        /// <summary>
        /// 指令Id
        /// </summary>
        [SugarColumn(ColumnName = "OrderID", IsPrimaryKey = true, IsNullable = false, ColumnDescription = "指令ID")]
        public int OrderId
        {
            get { return _orderId; }
            set
            {
                if (_orderId!=value)
                {
                    _orderId = value;
                    RaisePropertyChanged("OrderId");
  
                }
            }
        }

        /// <summary>
        /// 垛号
        /// </summary>
        private string _pileNo = string.Empty;

        [SugarColumn(ColumnName = "Pile_NO", Length = 256, IsNullable = false, ColumnDescription = "垛号")]
        public string PileNo
        {
            get { return _pileNo; }
            set
            {
                if (value != _pileNo)
                {
                    _pileNo = value;
                    RaisePropertyChanged("PileNo");
                }
            }
        }

        /// <summary>
        /// 是否是入库
        /// </summary>
        [SugarColumn(ColumnName = "OrderType", Length = 100, IsNullable = true, ColumnDescription = "指令类型：In,Out,Move")]
        public string OrderTypeName
        {
            get;set;
        }

        [SugarColumn(IsIgnore = true)]
        public OrderTypeEnum OrderType
        {
            get { return _orderType; }
            set
            {
                this.OrderTypeName = value.ToString();
                if (value != _orderType)
                {
                    _orderType = value;
                    RaisePropertyChanged("OrderType");   
                }
            }
        }

        /// <summary>
        /// 货物数量
        /// </summary>
        [SugarColumn(ColumnName = "GOODS_COUNT", IsNullable = true, ColumnDescription = "货物数量")]
        public int GoodsCount
        {
            get { return _goodsCount; }
            set
            {
                _goodsCount = value;
                RaisePropertyChanged("GoodsCount");
            }
        }

        /// <summary>
        /// 货物类型
        /// </summary>
        [SugarColumn(ColumnName = "GOODS_TYPE", IsNullable = true, ColumnDescription = "货物类型")]
        public int GoodsType
        {
            get { return _goodsType; }
            set
            {
                _goodsType = value;
                RaisePropertyChanged();
            }
        }

        [SugarColumn(ColumnName = "Current_Addr", Length = 100, IsNullable = true, ColumnDescription = "指令的当前地址。遵循协议中地址命名规范")]
        public string CurrAddrName
        {
            get; set;
        }
        /// <summary>
        /// 当前地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Addr CurrAddr
        {
            get { return _currAddr; }
            set
            {
                this.CurrAddrName = value != null ? value.FullName : string.Empty;
                if (value != _currAddr)
                {
                    _currAddr = value;
                    RaisePropertyChanged("CurrAddr");

                    
                }
            }
        }

        [SugarColumn(ColumnName = "Destination_Add", Length = 64, IsNullable = false, ColumnDescription = "指令的目的地址。遵循协议中地址命名规范")]
        public string DestAddrName
        {
            get; set;
        }
        /// <summary>
        /// 目的地址
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Addr DestAddr
        {
            get { return _destAddr; }
            set
            {
                this.DestAddrName = value != null ? value.FullName : string.Empty;
                if (_destAddr != value)
                {
                    _destAddr = value;
                    RaisePropertyChanged("DestAddr");
                }
            }
        }
        [SugarColumn(ColumnName = "SOURCE", Length = 100, IsNullable = true, ColumnDescription = "指令的来源，UPPER上层系统，SELFWCS自己生成，MANNUAL人工手动生成")]
        public string SourceName
        {
            get;set;
        }

        /// <summary>
        /// 指令来源指令的来源，Upper上层系统，SelfWCS自己生成，Mannual人工手动生成
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public TaskSourceEnum Source
        {
            get { return _source; }
            set
            {
                this.SourceName = value.ToString();
                if (value != _source)
                {
                    _source = value;
                    RaisePropertyChanged("Source");
                }

            }
        }

        /// <summary>
        /// 数量
        /// </summary>
        [SugarColumn(ColumnName = "QTY", IsNullable = true, ColumnDescription = "数量")]
        public int Qty { get; set; }

        /// <summary>
        /// 是否需要上报上层系统
        /// </summary>
        [SugarColumn(ColumnName = "IsReport", Length = 100, IsNullable = true, ColumnDescription = "")]
        public bool IsReport
        {
            get { return _isReport; }
            set
            {
                if (value != _isReport)
                {
                    _isReport = value;
                    RaisePropertyChanged("IsReport");
                }
            }
        }
        [SugarColumn(ColumnName = "WH_Code", Length = 20, IsNullable = false, ColumnDescription = "仓库代码，该字段区分运行多套WCS系统。电力行业项目如果用ORACLE数据库也需要保存该字段")]
        public string WhCode
        {
            get;set;
        }

        [SugarColumn(IsIgnore =true)]
        public bool  IsEmptyTray
        {
            get { return _isEmptyTray; }
            set
            {
                if (value != _isEmptyTray)
                {
                    _isEmptyTray = value;
                    RaisePropertyChanged("IsEmptyTray");
                }
            }
        }
        /// <summary>
        /// 容器类型
        /// </summary>
        [SugarColumn(ColumnName = "TrayType", ColumnDescription = "容器类型")]
        public int? TrayType
        {
            get { return _trayType; }
            set
            {
                if (value != _trayType)
                {
                    _trayType = value;
                    RaisePropertyChanged("TrayType");
                }
            }
        }



        /// <summary>
        /// 建议的优先使用设备的名字集合字符串（用“;”隔开）
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PriorDeviceNamesString
        {
            get
            {
                string deviceNamesString = string.Empty;
                bool isFirst = true;
                if (priorDeviceList == null || priorDeviceList.Count < 1)
                {
                    return deviceNamesString;
                }
                lock (priorDeviceList)
                {
                    foreach (DeviceName deviceName in priorDeviceList)
                    {
                        if (true == isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            deviceNamesString += ";";
                        }

                        deviceNamesString += deviceName.ToString();
                    }
                }
                return deviceNamesString;
            }
        }

        private List<DeviceName> priorDeviceList = new List<DeviceName>();
        /// <summary>
        /// 建议的优先使用的设备名字链表。默认值为一个0长度的List。
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<DeviceName> PriorDeviceList
        {
            get
            {
                return priorDeviceList;
            }

            set
            {
                lock (priorDeviceList)
                {
                    priorDeviceList = value;
                    RaisePropertyChanged("PriorDeviceList");
                }
            }
        }

        /// <summary>
        /// Order对应的垛的尾箱的箱条码
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string BottomBoxBarcode
        {
            get
            {
                string[] boxBarcodeArr = this.PileNo.Split('_');
                return boxBarcodeArr[boxBarcodeArr.Length - 1];
            }
        }

        /// <summary>
        /// 上层系统的任务编号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string UpperTaskNo
        {
            get
            {
                return _upperTaskNo;
            }
            set
            {
                if (value != _upperTaskNo)
                {
                    _upperTaskNo = value;
                    RaisePropertyChanged("UpperTaskNo");
                }
            }
        }

        /// <summary>
        /// 设备任务号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string DeviceTaskNo
        {
             get
            {
                return _deviceTaskNo;
            }
            set
            {
                if (value != _deviceTaskNo)
                {
                    _deviceTaskNo = value;
                    RaisePropertyChanged("DeviceTaskNo");
                }
            }
        }

        /// <summary>
        /// 文档编号
        /// </summary>
        [SugarColumn(ColumnName = "DocumentCode", Length = 255, IsNullable = true, ColumnDescription = "指令的参考编号，可以为任务号或订单号等")]
        public string DocumentCode
        {
            get { return _documentCode; }
            set
            {
                if (value != _documentCode)
                {
                    _documentCode = value;
                    RaisePropertyChanged("DocumentCode");
                }
            }
        }

        /// <summary>
        /// 回库标识
        /// </summary>
        [SugarColumn(ColumnName = "Backflag", IsNullable = true, ColumnDescription = "是否回库标识(1是不回库，2是回库）")]
        public int BackFlag
        {
            get { return _backFlag; }
            set
            {
                if (value != _backFlag)
                {
                    _backFlag = value;
                    RaisePropertyChanged("BackFlag");
                }
            }
        }

        private DateTime createTime = DateTime.Now;
        /// <summary>
        /// 订单创建时间
        /// </summary>
        [SugarColumn(ColumnName = "Add_Time",  IsNullable = false, ColumnDescription = "创建指令时间，默认系统时间")]
        public DateTime CreateTime
        {
            get
            {
                return createTime;
            }
            set
            {
                if (createTime != value)
                {
                    createTime = value;
                    RaisePropertyChanged("CreateTime");
                }
            }
        }



        /// <summary>
        /// 上层系统源任务类型
        /// </summary>	
        [SugarColumn(ColumnName = "SOURCETASKTYPE",IsNullable = true, ColumnDescription = "上层系统的任务类型")]
        public SourceTaskEnum? SourceTaskType
        {
            get { return _sourceTaskType; }
            set
            {
                if (_sourceTaskType!=value)
                {
                    _sourceTaskType = value;
                    RaisePropertyChanged("SourceTaskType");
                }
            }
        }

        /// <summary>
        /// 指令的详细信息，可以用于打印日志
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("OrderID:{0};当前地址:{1};目的地址:{2};订单类型:{3};垛号:{4}。"
                , OrderId, CurrAddr, DestAddr, OrderType, PileNo);
        }


        [SugarColumn(IsIgnore = true)]
        public int Id
        {
            get { return OrderId; }
            set { OrderId = value; }
        }

        private string _name = "指令:";
        private int _orderPriority;
        private int _orderId;
        private OrderTypeEnum _orderType;
        private Addr _currAddr;
        private Addr _destAddr;
        private TaskSourceEnum _source;
        private bool _isReport;
        private bool _isEmptyTray;
        private string _documentCode;
        private int _backFlag;
        private SourceTaskEnum? _sourceTaskType;
        private int? _trayType;

        private string _upperTaskNo;

        private string _deviceTaskNo;
        private int _goodsCount;
        private int _goodsType;

        [SugarColumn(IsIgnore = true)]
        public string Name
        {
            get { return _name + Id; }
            set
            {
                if (_name != value)
                {
                    RaisePropertyChanged("Name");
                    _name = value;
                }
            }
        }
    }
}
