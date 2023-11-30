using System;
using System.Collections.Generic;
using System.Text;
using CL.Framework.CmdDataModelPckg;
using System.ComponentModel;
using System.Windows;
using CL.WCS.DataModelPckg.View;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using SqlSugar;

namespace CL.WCS.DataModelPckg
{

    /// <summary>
    /// 执行中的指令
    /// </summary>
    [SugarTable("T_BO_OrderOperate","整个WCS系统中的指令（命令）操作信息表")]
    public class ExOrder : Order, IUnique,IDisposable
    {
        [SugarColumn(IsIgnore = true)]
        public string UniqueCode
        {
            get
            {
                return OrderId.ToString();
            }
            set { OrderId = int.Parse(value); }
        }

        /// <summary>
        /// 当前执行的设备名称
        /// </summary>	
        [SugarColumn(ColumnName = "CURRENT_DEVICE",Length = 255,IsNullable = true, ColumnDescription = "当前处理的设备名称")]
        public int CurHandlerId
        {
            get { return _curHandlerId; }
            set
            {
                if (value != _curHandlerId)
                {
                    _curHandlerId = value;
                    RaisePropertyChanged("CurHandlerId");
                }
            }
        }


        /// <summary>
        /// 分配失败的次数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int AllocateFailTime
        {
            get { return _allocateFailTime; }
            set
            {
                if (_allocateFailTime != value)
                {
                    _allocateFailTime = value;
                    RaisePropertyChanged("AllocateFailTime");
                }
            }
        }

        /// <summary>
        ///  分配的次数
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int AllocateTime
        {
            get { return _allocateTime; }
            set
            {
                if (value != _allocateTime)
                {
                    _allocateTime = value;
                    RaisePropertyChanged("AllocateTime");
                }

            }
        }

        private string _startAddrName = "";
        /// <summary>
        /// 开始地址
        /// </summary>
        [SugarColumn(ColumnName = "Start_Addr", Length = 64, IsNullable = false, ColumnDescription = "指令的起始地址。遵循协议中地址命名规范")]
        public string StartAddrName
        {
            get;set;
        }

        [SugarColumn(IsIgnore = true)]
        public Addr StartAddr
        {
            get { return _startAddr; }
            set
            {
                this.StartAddrName = value!=null?value.FullName:string.Empty;
                if (value != _startAddr)
                {
                    _startAddr = value;
                    RaisePropertyChanged("StartAddr");
                }

            }
        }


        [SugarColumn(ColumnName = "Next_Addr", Length = 64, IsNullable = true, ColumnDescription = "指令的下一步地址。遵循协议中地址命名规范")]
        public string NextAddrName
        {
            get;set;
        }
        /// <summary>
        /// 下一步地址
        /// 软件给硬件下指令后会给下一步地址赋值，暂时没有清空处理
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Addr NextAddr
        {
            get { return _nextAddr; }
            set
            {
                this.NextAddrName = value != null ? value.FullName : string.Empty;
                if (_nextAddr != value)
                {
                    _nextAddr = value;
                    RaisePropertyChanged("NextAddr");
                }
            }
        }

        /// <summary>
        /// 指令完成的类型
        /// </summary>
        [SugarColumn(ColumnName = "FINISH_TYPE", IsNullable = true, ColumnDescription = "指令的完成方式：0自动完成1单步完成2强制完成3异常完成4取消")]
        public FinishType? FinishType
        {
            get { return _finishType; }
            set
            {
                if (value != _finishType)
                {
                    _finishType = value;
                    RaisePropertyChanged("FinishType");
                }
            }
        }

        private StatusEnum _status = StatusEnum.Waiting;
        /// <summary>
        /// 指令当前状态
        /// </summary>
        [SugarColumn(ColumnName = "Status", DefaultValue = "0", IsNullable = false, ColumnDescription = "指令状态，默认为0")]
        public StatusEnum Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    RaisePropertyChanged("Status"); 
                }
            }
        }

        private bool _isAllocated = false;
        /// <summary>
        /// 指令是否已经分发
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public bool IsAllocated
        {
            get { return _isAllocated; }
            set
            {
                if (_isAllocated!=value)
                {
                    _isAllocated = value;
                    RaisePropertyChanged("IsAllocated");
                }
            }
        }

        private int helperRGVNo = -0x7FFFFFFF;
        private int _curHandlerId;
        private int _allocateFailTime;
        private int _allocateTime;
        private Addr _startAddr;
        private Addr _nextAddr;
        private FinishType? _finishType;

        /// <summary>
        /// 
        /// </summary>	
        [SugarColumn(ColumnName = "Prior_RGV_NO",IsNullable = true, ColumnDescription = "建议的设备号")]
        public int HelperRGVNo
        {
            get { return helperRGVNo; }
            set
            {
                if (value != helperRGVNo)
                {
                    helperRGVNo = value;
                    RaisePropertyChanged("HelperRGVNo");
                }
            }
        }

        /// <summary>
        /// Order转换成ExOrder
        /// </summary>
        /// <param name="order">order</param>
        /// <returns></returns>
        public static ExOrder OrderToExOrder(Order order)
        {
            ExOrder exOrder = new ExOrder();

            //由于是第一次转换，需要填写StartAddr和计算PileId
            exOrder.StartAddr = order.CurrAddr.Clone();

            //其他信息直接从输入参数中copy
            exOrder.OrderId = order.OrderId;
            exOrder.PileNo = order.PileNo;
            exOrder.CurrAddr = order.CurrAddr;
            exOrder.DestAddr = order.DestAddr;
            exOrder.OrderType = order.OrderType;
            exOrder.PriorDeviceList = order.PriorDeviceList;
            exOrder.OrderPriority = order.OrderPriority;
            exOrder.DocumentCode = order.DocumentCode;
            exOrder.BackFlag = order.BackFlag;
            exOrder.CreateTime = order.CreateTime;
            exOrder.IsReport = order.IsReport;
            exOrder.Source = order.Source;
            exOrder.Status = StatusEnum.Waiting;
            exOrder.Qty = order.Qty;
            exOrder.GoodsCount = order.GoodsCount;
            exOrder.GoodsType = order.GoodsType;
            exOrder.UpperTaskNo = order.UpperTaskNo;
            exOrder.DeviceTaskNo = order.DeviceTaskNo;
            exOrder.SourceTaskType = order.SourceTaskType;
            exOrder.TrayType = order.TrayType;
            return exOrder;
        }

        public void ChangeContentFrom(ExOrder srcOrder)
        {
            this.CurrAddr = srcOrder.CurrAddr;
            this.DestAddr = srcOrder.DestAddr;//未来“重分配仓位功能”用，现在的代码会保持左右一致
            this.Status = srcOrder.Status;
            this.HelperRGVNo = srcOrder.HelperRGVNo;
            this.OrderPriority = srcOrder.OrderPriority;
        }

        ///// <summary>
        ///// 返回ExOrder的克隆副本
        ///// </summary>
        ///// <returns></returns>
        public ExOrder DeepClone()
        {
            return new ExOrder
            {
                OrderId = this.OrderId,
                OrderType = this.OrderType,
                Status = this.Status,
                StartAddr = this.StartAddr != null ? StartAddr.Clone() : null,
                DestAddr = this.DestAddr != null ? DestAddr.Clone() : null,
                PileNo = this.PileNo,
                DocumentCode = this.DocumentCode,
                BackFlag = this.BackFlag,
                IsAllocated = this.IsAllocated,
                CurrAddr = this.CurrAddr != null ? CurrAddr.Clone() : null,
                OrderPriority = this.OrderPriority,
                HelperRGVNo = this.HelperRGVNo,
                PriorDeviceList = this.PriorDeviceList,
                NextAddr = this.NextAddr != null ? NextAddr.Clone() : null,
                CreateTime = this.CreateTime,
                FinishType = this.FinishType,
                AllocateFailTime = this.AllocateFailTime,
                AllocateTime = this.AllocateTime,
                CurHandlerId = this.CurHandlerId,
                IsReport = this.IsReport,
                Source = this.Source,
                Qty = this.Qty,
                UpperTaskNo=this.UpperTaskNo,
                DeviceTaskNo = this.DeviceTaskNo,
                SourceTaskType=this.SourceTaskType,
                GoodsType = this.GoodsType,
                GoodsCount = this.GoodsCount,
                TrayType=this.TrayType,
            };
        }

        /// <summary>
        /// 详细参数的描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("OrderID:{0};起始地址:{1};目的地址:{2};当前地址:{3};下一步地址:{4};订单状态:{5};订单类型:{6};垛号:{7}；当前执行的设备：{8}。"
                , OrderId, StartAddr, DestAddr, CurrAddr, NextAddr, Status.GetDescription(), OrderType.GetDescription(), PileNo, CurHandlerId);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (obj is ExOrder)
            {
                ExOrder exOrder = obj as ExOrder;
                if (exOrder.OrderId.Equals(this.OrderId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool _isDisposed = false;

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    //清理托管对象
                    //NextAddr = null;
                    //StartAddr = null;
                    //CurrAddr = null;
                }
                //清理非托管对象
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

    /// <summary>
    /// 指令完成的方式
    /// </summary>
    public enum FinishType
    {
        /// <summary>
        /// 未完成
        /// </summary>
        [Description("未完成")]
        UnFinished = 0,
        /// <summary>
        /// 自动完成
        /// </summary>
        [Description("自动完成")]
        AutoFinish = 1,
        /// <summary>
        /// 单步完成
        /// </summary>
        [Description("单步完成")]
        StepFinish = 2,
        /// <summary>
        /// 强制完成
        /// </summary>
        [Description("强制完成")]
        ForceFinish = 3,
        /// <summary>
        /// 异常完成
        /// </summary>
        [Description("异常完成")]
        ExpFinish = 4,
        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancle = 5,
        /// <summary>
        /// 通知设备失败
        /// </summary>
        [Description("通知失败")]
        NotifyFail = 6,
        /// <summary>
        /// 丢弃
        /// </summary>
        [Description("丢弃")]
        Discard = 7

    }

    /// <summary>
    /// 指令当前状态枚举
    /// </summary>
    public enum StatusEnum
    {
        /// <summary>
        /// 等待执行
        /// </summary>
        [Description("等待执行")]
        Waiting = 0,
        /// <summary>
        /// 处理中
        /// </summary>
        [Description("等待通知设备")]
        Processing = 1,
        /// <summary>
        /// 通知设备处理
        /// </summary>
        [Description("已通知设备")]
        NotifyOPC = 2,
        /// <summary>
        /// 指令搬运完成
        /// </summary>
        [Description("设备搬运完成")]
        TransportCompleted = 3,
        /// <summary>
        /// 命令已经通知上层系统
        /// </summary>
        [Description("已通知上层系统")]
        CmdSent = 4,
        /// <summary>
        /// 通知设备失败
        /// </summary>
        [Description("通知设备失败")]
        NotifyFail = 5,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Exception = 6,
        /// <summary>
        /// 取消
        /// </summary>
        [Description("取消")]
        Cancle = 7,
        /// <summary>
        /// 丢弃
        /// </summary>
        [Description("丢弃")]
        Discard = 8

    }
}
