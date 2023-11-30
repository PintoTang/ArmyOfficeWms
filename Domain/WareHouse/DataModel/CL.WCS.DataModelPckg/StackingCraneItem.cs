using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 堆垛机信息
    /// </summary>
	public class StackingCraneItem : Item, INotifyPropertyChanged
	{
		string _TaskType = string.Empty;
		string _StackingCraneName = string.Empty;
		string _EquimentStatus = string.Empty;
		string _CommunicationStatus = string.Empty;

		/// <summary>
		/// 堆垛机入库异常类型
		/// 1.双重入库 2.入深浅有
		/// </summary>
		public int InboundExceptionType
		{
			set;
			get;
		}

		/// <summary>
		/// 堆垛机名
		/// </summary>

		public string StackingCraneName
		{
			get { return _StackingCraneName; }
			set
			{
				_StackingCraneName = value;
				OnPropertyChanged("StackingCraneName");
			}
		}

		/// <summary>
		/// 设备状态
		/// </summary>
		public string EquimentStatus
		{
			get { return _EquimentStatus; }
			set
			{
				_EquimentStatus = value;
				OnPropertyChanged("EquimentStatus");
			}
		}

		/// <summary>
		/// 通讯状态
		/// </summary>
		public string CommunicationStatus
		{
			get { return _CommunicationStatus; }
			set
			{
				_CommunicationStatus = value;
				OnPropertyChanged("CommunicationStatus");
			}
		}

		/// <summary>
		/// 任务类型
		/// </summary>
		public string TaskType
		{
			get { return _TaskType; }
			set
			{
				_TaskType = value;
				OnPropertyChanged("TaskType");
			}
		}
        /// <summary>
        /// 
        /// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
		protected virtual void OnPropertyChanged(string propName)
		{
			try
			{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
			catch
			{

			}
		}
		void _subObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			OnPropertyChanged(null);
		}
	}
}
