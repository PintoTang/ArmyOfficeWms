using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.WebService.Service.Common;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.CLWS.CLWCS.WcsService.WcsServiceConfig;
using CLDC.Infrastructrue.Xml;
using MaterialDesignThemes.Wpf;

namespace CLDC.CLWS.CLWCS.WcsService
{
    public class WcsServiceManage
    {
        public WcsServiceConfig.WcsServiceConfig WebseviceConfig { get; set; }

        private static WcsServiceManage _manage;

        private static readonly object ObjLock = new object();
        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static WcsServiceManage Instance
        {
            get
            {
                lock (ObjLock)
                {
                    if (_manage == null)
                        _manage = new WcsServiceManage();
                    return _manage;
                }
            }
        }

        private readonly DataManageablePool<ServiceAbstract> _managedDataPool = new DataManageablePool<ServiceAbstract>();
        public DataManageablePool<ServiceAbstract> ManagedDataPool { get { return _managedDataPool; } }

        public bool IsEmpty
        {
            get { return _managedDataPool.Lenght <= 0; }
        }

        public OperateResult StartWcsService()
        {
            OperateResult initilizeResult = OperateResult.CreateFailedResult();
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\WcsServiceConfig.xml";
                if (!File.Exists(strFileName))
                {
                    initilizeResult.IsSuccess = false;
                    initilizeResult.Message = "不存在WebserviceConfig.xml配置文件，不进行启动Webservice";
                    return initilizeResult;
                }
                WebseviceConfig = (WcsServiceConfig.WcsServiceConfig)XmlSerializerHelper.LoadFromXml(strFileName, typeof(WcsServiceConfig.WcsServiceConfig));
                OperateResult startServiceResult = StartWebservice();

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

        private OperateResult StartWebservice()
        {
            OperateResult startServiceResult = OperateResult.CreateFailedResult();
            try
            {
                if (WebseviceConfig == null)
                {
                    startServiceResult.IsSuccess = false;
                    startServiceResult.Message = "Webservice配置文件读取失败，确认配置文件是否存在或配置文件格式";
                    return startServiceResult;
                }

                foreach (WcsServiceProperty webserviceProperty in WebseviceConfig.WebserviceList)
                {
                    ServiceAbstract service = (ServiceAbstract)Assembly.Load(webserviceProperty.NameSpace)
                  .CreateInstance(webserviceProperty.NameSpace + "." + webserviceProperty.ClassName);
                    if (service == null)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = string.Format("Webservice:{2} 命名空间：{0} 类名：{1} 反射出错", webserviceProperty.NameSpace, webserviceProperty.ClassName, webserviceProperty.Name);
                        return startServiceResult;
                    }
                    service.WebserviceId = webserviceProperty.Id;
                    service.Name = webserviceProperty.Name;
                    service.NameSpace = webserviceProperty.NameSpace;
                    service.ClassName = webserviceProperty.ClassName;
                    service.ServiceType = webserviceProperty.ServiceType;
                    service.Id = webserviceProperty.Id;
                    service.IsShowUi = webserviceProperty.IsShowUi;
                    service.IconKind = webserviceProperty.IconKind;

                    OperateResult initilizeResult = service.Initilize();
                    if (!initilizeResult.IsSuccess)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = initilizeResult.Message;
                        return startServiceResult;
                    }
                    OperateResult addResult = Add(service);
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
        public OperateResult Add(ServiceAbstract data)
        {
            return _managedDataPool.AddPool(data);
        }

        public OperateResult Delete(int id)
        {
            return _managedDataPool.RemovePool(id);
        }

        public ServiceAbstract Find(int id)
        {
            OperateResult<ServiceAbstract> findResult = _managedDataPool.FindData(id);
            return findResult.Content;
        }

        public ServiceAbstract Find(string name)
        {
            OperateResult<ServiceAbstract> findResult = _managedDataPool.FindData(name);
            return findResult.Content;
        }


        public List<ServiceAbstract> GetAllData()
        {
            return ManagedDataPool.DataPool;
        }

        public OperateResult Update(ServiceAbstract data)
        {
            return ManagedDataPool.UpdatePool(data);
        }

    }
}
