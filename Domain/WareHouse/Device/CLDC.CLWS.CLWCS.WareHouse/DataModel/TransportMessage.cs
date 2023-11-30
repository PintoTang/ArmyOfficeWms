using System;
using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.Device.Common.Model;
using GalaSoft.MvvmLight;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
	/// <summary>
	/// 搬运信息类
	/// </summary>
    public class TransportMessage : ViewModelBase,IDeviceTaskContent
	{
	    private TransportResultEnum _transportStatus;
	    private FinishType _transportFinishType;
	    private DateTime _updateDateTime;

	    public TransportMessage(string guid)
		{
			this.Guid = guid;
			AddDateTime = DateTime.Now;
            CheckTimeOut = new TimeOutCheck<string>(2 * 60 * 1000);
		    CheckTimeOut.CallBackParameter = guid;
            TransportStatus= TransportResultEnum.Wait;
		}

        /// <summary>
        /// 检查超时
        /// </summary>
        public TimeOutCheck<string> CheckTimeOut { get; set; }


		/// <summary>
		/// 执行搬运的设备
		/// </summary>
        public DeviceBaseAbstract TransportDevice { get; set; }

		/// <summary>
		/// 开始设备
		/// </summary>
        public DeviceBaseAbstract StartDevice { get; set; }

		/// <summary>
		/// 目标设备
		/// </summary>
        public DeviceBaseAbstract DestDevice { get; set; }

		/// <summary>
		/// 搬运的指令
		/// </summary>
        public ExOrder TransportOrder { get; set; }

		/// <summary>
		/// 开始地址
		/// </summary>
		public Addr StartAddr { get; set; }

        public int TransportOrderId { get { return TransportOrder.OrderId; } }

		/// <summary>
		/// 目的地址
		/// </summary>
		public Addr DestAddr { get; set; }

		/// <summary>
		/// 当前地址
		/// </summary>
		public Addr CurAddr { get; set; }
        /// <summary>
        /// 垛号
        /// </summary>
        public string PileNo {
            get { return TransportOrder.PileNo; } }

		/// <summary>
		/// 拥有者的ID
		/// </summary>
		public int OwnerId { get; set; }

	    /// <summary>
	    /// 搬运状态
	    /// </summary>
	    public TransportResultEnum TransportStatus
	    {
	        get { return _transportStatus; }
	        set
	        {
	            _transportStatus = value; 
	            RaisePropertyChanged();
	        }
	    }

	    /// <summary>
	    /// 搬运信息完成的方式
	    /// </summary>
	    public FinishType TransportFinishType
	    {
	        get { return _transportFinishType; }
	        set
	        {
	            _transportFinishType = value;
	            RaisePropertyChanged();
	        }
	    }

	    /// <summary>
	    /// 搬运信息的唯一标识
	    /// </summary>
	    public string Guid { get; set; }
		
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime AddDateTime { get; set; }

	    /// <summary>
	    /// 完成时间
	    /// </summary>
	    public DateTime UpdateDateTime
	    {
	        get { return _updateDateTime; }
	        set
	        {
	            _updateDateTime = value;
	            RaisePropertyChanged();
	        }
	    }
		private int _trayType = 0;

		public int TrayType
        {

			get { return _trayType; }
			set
			{
				_trayType = value;
				RaisePropertyChanged();
			}
		}


		public TransportMsgModel DatabaseMode
		{
			get
			{
				TransportMsgModel mode = new TransportMsgModel()
				{
					CurAddr = this.CurAddr.FullName,
					DestId = this.DestDevice.Id,
					DestAddr = this.DestAddr.FullName,
					ExOrderId = this.TransportOrder.OrderId,
                    OwnerId = this.OwnerId,
					StartAddr = this.StartAddr.FullName,
					StartId = this.StartDevice.Id,
					TransportId = this.TransportDevice.Id,TransportStatus = this.TransportStatus,
					Guid = this.Guid,
					AddDateTime = this.AddDateTime,
					UpdateDateTime = this.UpdateDateTime,
                    PileNo=this.PileNo,
                    TransportFinishType = this.TransportFinishType,
					TrayType=this.TrayType,
				};
				return mode;
			}
		}

        public string UniqueCode
        {
            get { return Guid; }
            set { Guid = value; }
        }

		public object Clone()
		{
			return this.MemberwiseClone();
		}

	    public override string ToString()
	    {
            return string.Format("搬运编号：{0} 指令编号：{1} 开始设备：{2} 搬运设备：{3} 目标设备：{4}", UniqueCode, TransportOrder.OrderId, StartDevice.Name, TransportDevice == null ? "" : TransportDevice.Name, DestDevice == null ? "" : DestDevice.Name);
	    }

	    public override bool Equals(object obj)
	    {
            if (obj is TransportMessage)
            {
                TransportMessage transport = obj as TransportMessage;
                if (transport.Guid.Equals(this.Guid))
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
    }

	


}
