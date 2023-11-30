using CLDC.Framework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHSE.Monitor.Client.Model.DataPool
{
	public class BaseDataPool<TDictKey, TDictValue> where TDictValue : new()
	{
		protected Dictionary<TDictKey, TDictValue> _dataPoolDict = new Dictionary<TDictKey, TDictValue>();

		protected bool DeleteDataPoolRecord(TDictKey key)
		{
			bool isSuccess = false;
			lock (_dataPoolDict)
			{
				isSuccess = _dataPoolDict.Remove(key);
			}

			if (false == isSuccess)
			{
				Log.getDebugFile().Debug("界面层尝试删除一个不存在的数据池记录，说明界面层有严重bug，有可能是重复多次删除。出问题的key为：" + key.ToString());
			}
			return isSuccess;
		}

		protected TDictValue GetDataPoolRecord(TDictKey key)
		{
			lock (_dataPoolDict)
			{
				if (_dataPoolDict.ContainsKey(key))
				{
					return _dataPoolDict[key];
				}
				else
				{
					return default(TDictValue);
				}
			}
		}

		protected void CreateNewValueIfNotInDict(TDictKey key)
		{
			lock (_dataPoolDict)
			{
				if (!_dataPoolDict.ContainsKey(key))
				{
					_dataPoolDict.Add(key, new TDictValue());
				}
			}
		}
	}
}


