using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.UpperService.HandleBusiness.Config;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.UpperService.HandleBusiness
{
    public class UpperServiceBusinessManage
    {
        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static UpperServiceBusinessManage Instance
        {
            get
            {
                if (_workerManage == null)
                    _workerManage = new UpperServiceBusinessManage();
                return _workerManage;
            }
        }
        public UpperServiceConfig UpperServiceConfig { get; set; }

        private static UpperServiceBusinessManage _workerManage;

        private readonly DataManageablePool<UpperServiceBusinessAbstract> _managedDataPool = new DataManageablePool<UpperServiceBusinessAbstract>();
        public DataManageablePool<UpperServiceBusinessAbstract> ManagedDataPool { get { return _managedDataPool; } }

        public OperateResult InitilizeAllUpperService()
        {
            OperateResult initilizeResult = OperateResult.CreateFailedResult();
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\UpperServiceConfig.xml";
                if (!File.Exists(strFileName))
                {
                    initilizeResult.IsSuccess = false;
                    initilizeResult.Message = "不存在UpperServiceConfig.xml配置文件，不进行注册上层服务接口";
                    return initilizeResult;
                }
                UpperServiceConfig = (UpperServiceConfig)XmlSerializerHelper.LoadFromXml(strFileName, typeof(UpperServiceConfig));

                OperateResult startServiceResult = RegisterUpperService();

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


        private OperateResult RegisterUpperService()
        {
            //IServiceHandle
            //UpperServiceManange

            OperateResult registerServiceResult = OperateResult.CreateFailedResult();
            try
            {
                if (UpperServiceConfig == null)
                {
                    registerServiceResult.IsSuccess = false;
                    registerServiceResult.Message = "UpperServiceConfig.Xml配置文件读取失败，确认配置文件是否存在或配置文件格式";
                    return registerServiceResult;
                }

                foreach (UpperServiceProperty serviceProperty in UpperServiceConfig.UpperServiceList)
                {
                    UpperServiceBusinessAbstract upperBusinessAbstract = (UpperServiceBusinessAbstract)Assembly.Load(serviceProperty.NameSpace)
                  .CreateInstance(serviceProperty.NameSpace + "." + serviceProperty.ClassName);
                    if (upperBusinessAbstract == null)
                    {
                        registerServiceResult.IsSuccess = false;
                        registerServiceResult.Message = string.Format("UpperServiceBusinessAbstract:{2} 命名空间：{0} 类名：{1} 反射出错", serviceProperty.NameSpace, serviceProperty.ClassName, serviceProperty.Name);
                        return registerServiceResult;
                    }
                    upperBusinessAbstract.Id = serviceProperty.Id;
                    upperBusinessAbstract.Name = serviceProperty.Name;
                    upperBusinessAbstract.NameSpace = serviceProperty.NameSpace;
                    upperBusinessAbstract.ClassName = serviceProperty.ClassName;
                    upperBusinessAbstract.SystemName = serviceProperty.SystemName;

                    OperateResult initilizeResult = upperBusinessAbstract.Initilize();
                    if (!initilizeResult.IsSuccess)
                    {
                        registerServiceResult.IsSuccess = false;
                        registerServiceResult.Message = initilizeResult.Message;
                        return registerServiceResult;
                    }
                    OperateResult addResult = Add(upperBusinessAbstract);
                    if (!addResult.IsSuccess)
                    {
                        registerServiceResult.IsSuccess = false;
                        registerServiceResult.Message = addResult.Message;
                        return registerServiceResult;
                    }
                }
                registerServiceResult.IsSuccess = true;
                return registerServiceResult;
            }
            catch (Exception ex)
            {
                registerServiceResult.IsSuccess = false;
                registerServiceResult.Message = OperateResult.ConvertException(ex);

            }
            return registerServiceResult;
        }


        public OperateResult Add(UpperServiceBusinessAbstract data)
        {
            return _managedDataPool.AddPool(data);
        }

        public OperateResult Delete(int id)
        {
            return _managedDataPool.RemovePool(id);
        }

        public UpperServiceBusinessAbstract Find(int id)
        {
            OperateResult<UpperServiceBusinessAbstract> findResult = _managedDataPool.FindData(id);
            return findResult.Content;
        }

        public UpperServiceBusinessAbstract Find(string name)
        {
            OperateResult<UpperServiceBusinessAbstract> findResult = _managedDataPool.FindData(name);
            return findResult.Content;
        }

        public UpperServiceBusinessAbstract Find(UpperSystemEnum system)
        {
            OperateResult<UpperServiceBusinessAbstract> findResult =
                _managedDataPool.FindData(u => u.SystemName.Equals(system));
            return findResult.Content;
        }

        public List<UpperServiceBusinessAbstract> GetAllData()
        {
            return ManagedDataPool.DataPool;
        }

        public OperateResult Update(UpperServiceBusinessAbstract data)
        {
            return ManagedDataPool.UpdatePool(data);
        }

    }
}
