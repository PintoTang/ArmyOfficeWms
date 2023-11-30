using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CLDC.CLWCS.Service.DataService.Config;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWCS.Service.DataService
{
    public class WcsDataServiceManage
    {
        public WcsDataServiceConfig WcsDataServiceConfig { get; set; }

        private static WcsDataServiceManage _manage;

        private static readonly object ObjLock = new object();
        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static WcsDataServiceManage Instance
        {
            get
            {
                lock (ObjLock)
                {
                    if (_manage == null)
                        _manage = new WcsDataServiceManage();
                    return _manage;
                }
            }
        }

        private readonly DataManageablePool<WcsDataServiceAbstract> _managedDataPool = new DataManageablePool<WcsDataServiceAbstract>();
        public DataManageablePool<WcsDataServiceAbstract> ManagedDataPool { get { return _managedDataPool; } }


        public bool IsEmpty
        {
            get { return _managedDataPool.Lenght <= 0; }
        }

        public OperateResult Initilize()
        {
            OperateResult initilizeResult = OperateResult.CreateFailedResult();
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\WcsDataServiceConfig.xml";
                if (!File.Exists(strFileName))
                {
                    initilizeResult.IsSuccess = true;
                    initilizeResult.Message = "不存在WcsDataServiceConfig.xml配置文件，不进行启动WcsDataServiceConfig";
                    return initilizeResult;
                }
                WcsDataServiceConfig = (WcsDataServiceConfig)XmlSerializerHelper.LoadFromXml(strFileName, typeof(WcsDataServiceConfig));
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
                    startServiceResult.Message = "WcsDataServiceConfig配置文件读取失败，确认配置文件是否存在或配置文件格式";
                    return startServiceResult;
                }

                foreach (WcsDataServiceProperty wcsDataServiceProperty in WcsDataServiceConfig.WcsDataServiceItemList)
                {
                    WcsDataServiceAbstract wcsDataServiceItem =
                        (WcsDataServiceAbstract)Assembly.Load(wcsDataServiceProperty.NameSpace)
                            .CreateInstance(wcsDataServiceProperty.NameSpace + "." + wcsDataServiceProperty.ClassName);
                    if (wcsDataServiceItem == null)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = string.Format("DataService:{2} 命名空间：{0} 类名：{1} 反射出错",
                            wcsDataServiceProperty.NameSpace, wcsDataServiceProperty.ClassName, wcsDataServiceProperty.Name);
                        return startServiceResult;
                    }
                    wcsDataServiceItem.Id = wcsDataServiceProperty.Id;
                    wcsDataServiceItem.Name = wcsDataServiceProperty.Name;
                    wcsDataServiceItem.NameSpace = wcsDataServiceProperty.NameSpace;
                    wcsDataServiceItem.ClassName = wcsDataServiceProperty.ClassName;
                    wcsDataServiceItem.Id = wcsDataServiceProperty.Id;
                    wcsDataServiceItem.IsShowUi = wcsDataServiceProperty.IsShowUi;
                    wcsDataServiceItem.IconKind = wcsDataServiceProperty.IconKind;

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

        public OperateResult Add(WcsDataServiceAbstract data)
        {
            return _managedDataPool.AddPool(data);
        }

        public OperateResult Delete(int id)
        {
            return _managedDataPool.RemovePool(id);
        }

        public WcsDataServiceAbstract Find(int id)
        {
            OperateResult<WcsDataServiceAbstract> findResult = _managedDataPool.FindData(id);
            return findResult.Content;
        }

        public WcsDataServiceAbstract Find(string name)
        {
            OperateResult<WcsDataServiceAbstract> findResult = _managedDataPool.FindData(name);
            return findResult.Content;
        }


        public List<WcsDataServiceAbstract> GetAllData()
        {
            return ManagedDataPool.DataPool;
        }

        public OperateResult Update(WcsDataServiceAbstract data)
        {
            return ManagedDataPool.UpdatePool(data);
        }


    }
}
