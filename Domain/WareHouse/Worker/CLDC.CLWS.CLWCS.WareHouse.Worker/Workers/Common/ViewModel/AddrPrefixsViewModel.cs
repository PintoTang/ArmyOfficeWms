using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Config;
using CLDC.Infrastructrue.UserCtrl;
using CLDC.Infrastructrue.UserCtrl.Model;

namespace CLDC.CLWS.CLWCS.WareHouse.Worker.Workers.Common.ViewModel
{

    public class AddrPrefixsViewModel
    {
        private AddrPrefixsProperty AddrPrefixsData { get; set; }

        public AddrPrefixsViewModel(AddrPrefixsProperty model)
        {
            AddrPrefixsData = model;
            InitAddressLst();
        }

        public ObservableCollection<PrefixsItem> InPrefixsLst { get; set; }

        public ObservableCollection<PrefixsItem> OutPrefixsLst { get; set; }

        public ObservableCollection<PrefixsItem> MovePrefixsLst { get; set; }


        private void InitAddressLst()
        {
            InPrefixsLst = new ObservableCollection<PrefixsItem>();
            OutPrefixsLst = new ObservableCollection<PrefixsItem>();
            MovePrefixsLst = new ObservableCollection<PrefixsItem>();
            foreach (PrefixsItem prefixs in AddrPrefixsData.AddrPrefixsList)
            {
                if (prefixs.Type.Equals("In"))
                {
                    string[] addrLst = prefixs.Value.Trim().Split('|');
                    for (int i = 0; i < addrLst.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(addrLst[i]))
                        {
                            continue;
                        }
                        PrefixsItem tempItem = new PrefixsItem
                        {
                            Type = prefixs.Type,
                            Value = addrLst[i]
                        };
                        InPrefixsLst.Add(tempItem);
                    }
                }

                else if (prefixs.Type.Equals("Out"))
                {
                    string[] addrLst = prefixs.Value.Trim().Split('|');
                    for (int i = 0; i < addrLst.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(addrLst[i]))
                        {
                            continue;
                        }
                        PrefixsItem tempItem = new PrefixsItem
                        {
                            Type = prefixs.Type,
                            Value = addrLst[i]
                        };
                        OutPrefixsLst.Add(tempItem);
                    }
                }
                else
                {
                    string[] addrLst = prefixs.Value.Trim().Split('|');
                    for (int i = 0; i < addrLst.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(addrLst[i]))
                        {
                            continue;
                        }
                        PrefixsItem tempItem = new PrefixsItem
                        {
                            Type = prefixs.Type,
                            Value = addrLst[i]
                        };
                        MovePrefixsLst.Add(tempItem);
                    }
                }
            }
        }

        private MyCommand _addInPrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddInPrefixsCommand
        {
            get
            {
                if (_addInPrefixsCommand == null)
                    _addInPrefixsCommand = new MyCommand(AddInPrefixs);
                return _addInPrefixsCommand;
            }
        }
        private void AddInPrefixs(object obj)
        {
            PrefixsItem deviceProperty = new PrefixsItem();
            InPrefixsLst.Add(deviceProperty);
        }


        private MyCommand _deleteInPrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteInPrefixsCommand
        {
            get
            {
                if (_deleteInPrefixsCommand == null)
                    _deleteInPrefixsCommand = new MyCommand(DeleteInPrefixs);
                return _deleteInPrefixsCommand;
            }
        }

        private void DeleteInPrefixs(object arg)
        {
            if (!(arg is PrefixsItem))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的地址前缀信息");
                return;
            }
            PrefixsItem deleteItem = arg as PrefixsItem;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该地址前缀信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = InPrefixsLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "地址前缀信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("地址前缀信息删除成功");
            }
        }




        private MyCommand _addOutPrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddOutPrefixsCommand
        {
            get
            {
                if (_addOutPrefixsCommand == null)
                    _addOutPrefixsCommand = new MyCommand(AddOutPrefixs);
                return _addOutPrefixsCommand;
            }
        }
        private void AddOutPrefixs(object obj)
        {
            PrefixsItem deviceProperty = new PrefixsItem();
            OutPrefixsLst.Add(deviceProperty);
        }


        private MyCommand _deleteOutPrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteOutPrefixsCommand
        {
            get
            {
                if (_deleteOutPrefixsCommand == null)
                    _deleteOutPrefixsCommand = new MyCommand(DeleteOutPrefixs);
                return _deleteOutPrefixsCommand;
            }
        }

        private void DeleteOutPrefixs(object arg)
        {
            if (!(arg is PrefixsItem))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的地址前缀信息");
                return;
            }
            PrefixsItem deleteItem = arg as PrefixsItem;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该地址前缀信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = OutPrefixsLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "地址前缀信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("地址前缀信息删除成功");
            }
        }




        private MyCommand _addMovePrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand AddMovePrefixsCommand
        {
            get
            {
                if (_addMovePrefixsCommand == null)
                    _addMovePrefixsCommand = new MyCommand(AddMovePrefixs);
                return _addMovePrefixsCommand;
            }
        }
        private void AddMovePrefixs(object obj)
        {
            PrefixsItem deviceProperty = new PrefixsItem();
            MovePrefixsLst.Add(deviceProperty);
        }


        private MyCommand _deleteMovePrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand DeleteMovePrefixsCommand
        {
            get
            {
                if (_deleteMovePrefixsCommand == null)
                    _deleteMovePrefixsCommand = new MyCommand(DeleteMovePrefixs);
                return _deleteMovePrefixsCommand;
            }
        }

        private void DeleteMovePrefixs(object arg)
        {
            if (!(arg is PrefixsItem))
            {
                SnackbarQueue.MessageQueue.Enqueue("请选择需要删除的地址前缀信息");
                return;
            }
            PrefixsItem deleteItem = arg as PrefixsItem;
            MessageBoxResult result = MessageBoxEx.Show("确定删除该地址前缀信息？", "提示", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            bool removeResult = MovePrefixsLst.Remove(deleteItem);
            if (!removeResult)
            {
                string msg = "地址前缀信息删除失败";
                SnackbarQueue.MessageQueue.Enqueue(msg);
            }
            else
            {
                SnackbarQueue.MessageQueue.Enqueue("地址前缀信息删除成功");
            }
        }





        private MyCommand _savePrefixsCommand;
        /// <summary>
        /// 取消指令
        /// </summary>
        public MyCommand SavePrefixsCommand
        {
            get
            {
                if (_savePrefixsCommand == null)
                    _savePrefixsCommand = new MyCommand(SavePrefixs);
                return _savePrefixsCommand;
            }
        }

        private void SavePrefixs(object arg)
        {
            AddrPrefixsData.AddrPrefixsList.Clear();
            StringBuilder inPrefixs=new StringBuilder();
            StringBuilder outPrefixs=new StringBuilder();
            StringBuilder movePrefixs=new StringBuilder();
            foreach (PrefixsItem deviceProperty in InPrefixsLst)
            {
                inPrefixs.Append(deviceProperty.Value);
                inPrefixs.Append('|');
            }
            foreach (PrefixsItem deviceProperty in OutPrefixsLst)
            {
                outPrefixs.Append(deviceProperty.Value);
                outPrefixs.Append('|');
            }
            foreach (PrefixsItem deviceProperty in MovePrefixsLst)
            {
                movePrefixs.Append(deviceProperty.Value);
                movePrefixs.Append('|');
            }

            PrefixsItem inPrefixsItem = new PrefixsItem
            {
                Type = "In",
                Value = inPrefixs.ToString()
            };
            AddrPrefixsData.AddrPrefixsList.Add(inPrefixsItem);

            PrefixsItem outPrefixsItem = new PrefixsItem
            {
                Type = "Out",
                Value = outPrefixs.ToString()
            };
            AddrPrefixsData.AddrPrefixsList.Add(outPrefixsItem);

            PrefixsItem movePrefixsItem = new PrefixsItem
            {
                Type = "Move",
                Value = movePrefixs.ToString()
            };
            AddrPrefixsData.AddrPrefixsList.Add(movePrefixsItem);


            SnackbarQueue.MessageQueue.Enqueue("地址前缀信息保存成功");
        }


    }
}
