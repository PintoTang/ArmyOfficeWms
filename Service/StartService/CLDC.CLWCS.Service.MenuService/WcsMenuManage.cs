using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CLDC.CLWCS.Service.MenuService.Config;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWCS.Service.MenuService
{
    public class WcsMenuManage
    {
        public WcsMenuConfig WcsDataServiceConfig { get; set; }

        private static WcsMenuManage _manage;

        private static readonly object ObjLock = new object();
        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static WcsMenuManage Instance
        {
            get
            {
                lock (ObjLock)
                {
                    if (_manage == null)
                        _manage = new WcsMenuManage();
                    return _manage;
                }
            }
        }

        private readonly DataManageablePool<WcsMenuAbstract> _managedDataPool = new DataManageablePool<WcsMenuAbstract>();
        public DataManageablePool<WcsMenuAbstract> ManagedDataPool { get { return _managedDataPool; } }


        public bool IsEmpty
        {
            get { return _managedDataPool.Lenght <= 0; }
        }

        public OperateResult Initilize()
        {
            OperateResult initilizeResult = OperateResult.CreateFailedResult();
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\WcsMenuConfig.xml";
                if (!File.Exists(strFileName))
                {
                    initilizeResult.IsSuccess = true;
                    initilizeResult.Message = "不存在WcsMenuConfig.xml配置文件，不进行启动WcsMenuConfig";
                    return initilizeResult;
                }
                WcsDataServiceConfig = (WcsMenuConfig)XmlSerializerHelper.LoadFromXml(strFileName, typeof(WcsMenuConfig));
                OperateResult startServiceResult = InitilizeWcsDataServiceItem();

                if (!startServiceResult.IsSuccess)
                {
                    initilizeResult.IsSuccess = false;
                    initilizeResult.Message = startServiceResult.Message;
                    return initilizeResult;
                }

                initilizeResult.IsSuccess = true;
                return initilizeResult;
            }
            catch (Exception ex)
            {
                initilizeResult.IsSuccess = false;
                initilizeResult.Message = OperateResult.ConvertException(ex);

            }
            return initilizeResult;
        }

        private OperateResult InitilizeWcsDataServiceItem()
        {
            OperateResult startServiceResult = OperateResult.CreateFailedResult();
            try
            {
                if (WcsDataServiceConfig == null)
                {
                    startServiceResult.IsSuccess = false;
                    startServiceResult.Message = "WcsMenuConfig配置文件读取失败，确认配置文件是否存在或配置文件格式";
                    return startServiceResult;
                }

                Dictionary<string, Assembly> dic = new Dictionary<string, Assembly>();

                foreach (WcsMenuProperty wcsMenuProperty in WcsDataServiceConfig.WcsMenuItemList)
                {
                    Assembly assembly = null;
                    if (!dic.ContainsKey(wcsMenuProperty.NameSpace))
                    {
                        assembly = Assembly.Load(wcsMenuProperty.NameSpace);
                        dic[wcsMenuProperty.NameSpace] = assembly;
                    }
                    else
                    {
                        assembly = dic[wcsMenuProperty.NameSpace];
                    }
                    WcsMenuAbstract wcsDataServiceItem =(WcsMenuAbstract)assembly.CreateInstance(wcsMenuProperty.NameSpace + "." + wcsMenuProperty.ClassName);
                    if (wcsDataServiceItem == null)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = string.Format("WcsMenu:{2} 命名空间：{0} 类名：{1} 反射出错",
                            wcsMenuProperty.NameSpace, wcsMenuProperty.ClassName, wcsMenuProperty.Name);
                        return startServiceResult;
                    }
                    wcsDataServiceItem.Id = wcsMenuProperty.Id;
                    wcsDataServiceItem.Name = wcsMenuProperty.Name;
                    wcsDataServiceItem.NameSpace = wcsMenuProperty.NameSpace;
                    wcsDataServiceItem.ClassName = wcsMenuProperty.ClassName;
                    wcsDataServiceItem.Id = wcsMenuProperty.Id;
                    wcsDataServiceItem.IsShowUi = wcsMenuProperty.IsShowUi;
                    wcsDataServiceItem.IconKind = wcsMenuProperty.IconKind;

                    OperateResult initilizeResult = wcsDataServiceItem.Initilize();
                    if (!initilizeResult.IsSuccess)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = initilizeResult.Message;
                        return startServiceResult;
                    }
                    OperateResult addResult = Add(wcsDataServiceItem);
                    if (!addResult.IsSuccess)
                    {
                        return addResult;
                    }
                }

                startServiceResult.IsSuccess = true;
                return startServiceResult;
             }
            catch (Exception ex)
            {
                startServiceResult.IsSuccess = false;
                startServiceResult.Message = OperateResult.ConvertException(ex);

            }
            return startServiceResult;
        }

        public OperateResult Add(WcsMenuAbstract data)
        {
            return _managedDataPool.AddPool(data);
        }

        public OperateResult Delete(int id)
        {
            return _managedDataPool.RemovePool(id);
        }

        public WcsMenuAbstract Find(int id)
        {
            OperateResult<WcsMenuAbstract> findResult = _managedDataPool.FindData(id);
            return findResult.Content;
        }

        public WcsMenuAbstract Find(string name)
        {
            OperateResult<WcsMenuAbstract> findResult = _managedDataPool.FindData(name);
            return findResult.Content;
        }


        public List<WcsMenuAbstract> GetAllData()
        {
            return ManagedDataPool.DataPool;
        }

        public OperateResult Update(WcsMenuAbstract data)
        {
            return ManagedDataPool.UpdatePool(data);
        }


    }
}
