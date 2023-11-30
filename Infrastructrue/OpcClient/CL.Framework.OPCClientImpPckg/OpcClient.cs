using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.OPCClientImpPckg
{
    /// <summary>
    /// Opc通讯客户端装饰类
    /// </summary>
    public class OpcClient : IOpcCommunicationAbstract
    {
        private volatile bool isUsing;
        private HashSet<string> itemNameHashSet;
        private IOpcCommunicationAbstract opcClient;

        public OpcClient(IOpcCommunicationAbstract opcClient)
        {
            this.opcClient = opcClient;
            this.isUsing = false;
            this.itemNameHashSet = new HashSet<string>();
        }

        public bool IsUsing
        {
            get
            {
                return isUsing;
            }
            set
            {
                isUsing = value;
            }
        }

        public void AddGroup(string groupName)
        {
            opcClient.AddGroup(groupName);
        }

        public void AddItem(string groupName, string itemName)
        {
            opcClient.AddItem(groupName, itemName);
        }

        public DataItem Read(string groupName, string itemName)
        {
            TryAddItem(groupName, itemName);

            return opcClient.Read(groupName, itemName);
        }

        public List<DataItem> Read(string groupName, List<string> itemNameList)
        {
            TryAddItem(groupName, itemNameList);

            return opcClient.Read(groupName, itemNameList);
        }

        public void Write(string groupName, string itemName, object value)
        {
            TryAddItem(groupName, itemName);

            opcClient.Write(groupName, itemName, value);
        }

        public void Write(string groupName, Dictionary<string, object> itemValueDic)
        {
            TryAddItem(groupName, itemValueDic.Keys);

            opcClient.Write(groupName, itemValueDic);
        }

        public void Write(string groupName, Dictionary<string, int> itemValueDic)
        {
            TryAddItem(groupName, itemValueDic.Keys);

            opcClient.Write(groupName, itemValueDic);
        }

        public void Close()
        {
            opcClient.Close();
        }

        public void Dispose()
        {
            isUsing = false;
        }

        private void TryAddItem(string groupName, IEnumerable<string> itemNames)
        {
            foreach (string itemName in itemNames)
            {
                TryAddItem(groupName, itemName);
            }
        }

        private void TryAddItem(string groupName, string itemName)
        {
            if (!Contains(itemName))
            {
                AddItem(groupName, itemName);
            }
        }

        private bool Contains(string itemName)
        {
            lock (itemNameHashSet)
            {
                return !itemNameHashSet.Add(itemName);
            }
        }
    }
}