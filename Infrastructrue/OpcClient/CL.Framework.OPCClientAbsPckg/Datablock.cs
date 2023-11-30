using CL.Framework.CmdDataModelPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CL.Framework.OPCClientAbsPckg
{
	public class Datablock:NotifyObject, IUnique
	{
        public int OpcId { get; set; }
		public string Connection
		{
			get { return connection; }
		    set
		    {
		        connection = value;
                RaisePropertyChanged("Connection");
		    }
		}

	    /// <summary>
	    /// DB快的类型
	    /// </summary>
	    public DataBlockNameEnum DatablockEnum
	    {
	        get { return _datablockEnum; }
	        set
	        {
	            _datablockEnum = value;
                RaisePropertyChanged("DatablockEnum");
	        }
	    }

	    /// <summary>
	    /// DB块的名称
	    /// </summary>
	    public string DataBlockName
	    {
	        get { return _dataBlockName; }
	        set
	        {
	            _dataBlockName = value;
                RaisePropertyChanged("DataBlockName");
	        }
	    }

	    private string realDataBlockAddr = string.Empty;

		/// <summary>
		/// DB块的地址
		/// </summary>
		public string RealDataBlockAddr
		{
			get
			{
				if (realDataBlockAddr.Contains("S7:[S7 connection_"))
				{
					return realDataBlockAddr;
				}			
				return	 connection+realDataBlockAddr;
			}
		    set
		    {
		        realDataBlockAddr = value;
                RaisePropertyChanged("RealDataBlockAddr");
            }
		}

        private string realValue = string.Empty;
		private string connection = "S7:[S7 connection_1]";
	    private string _dataBlockName;
	    private DataBlockNameEnum _datablockEnum;
	    private string _dataType;
	    private string _name;

	    /// <summary>
	    /// DB数据的类型
	    /// </summary>
	    public string DataType
	    {
	        get { return _dataType; }
	        set
	        {
	            _dataType = value;
                RaisePropertyChanged("DataType");
	        }
	    }

	    /// <summary>
	    /// DB地址的实时值
	    /// </summary>
	    public string RealValue
		{
            get { return realValue; }
		    set
		    {
		        this.realValue = value;
                RaisePropertyChanged("RealValue");
		    }
		}

        public string UpdateValue { get; set; }

	    /// <summary>
	    /// 描述
	    /// </summary>
	    public string Name
	    {
	        get { return _name; }
	        set
	        {
	            _name = value;
	            RaisePropertyChanged("Name");
	        }
	    }

	    public override string ToString()
		{
			return string.Format("DB协议：DB协议名称： {0} DB的PLC地址：{1} ", this.Name, this.RealDataBlockAddr);
		}

	    public object Clone()
	    {
	       return this.MemberwiseClone();
	    }

	    public string UniqueCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Datablock)
            {
                Datablock db = obj as Datablock;
                if (db.DatablockEnum.Equals(this.DatablockEnum))
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
