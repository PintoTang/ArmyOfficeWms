using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CLDC.CLWCS.WareHouse.Architecture.Manage.Config;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWCS.WareHouse.Architecture.Manage
{
    public class ArchitectureManage
    {
        public ArchitectureConfig ArchitectureConfig { get; set; }

        private static ArchitectureManage _manage;

        private static readonly object ObjLock = new object();
        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static ArchitectureManage Instance
        {
            get
            {
                lock (ObjLock)
                {
                    if (_manage == null)
                        _manage = new ArchitectureManage();
                    return _manage;
                }
            }
        }

        private readonly DataManageablePool<ArchitectureDataAbstract> _managedDataPool = new DataManageablePool<ArchitectureDataAbstract>();
        public DataManageablePool<ArchitectureDataAbstract> ManagedDataPool { get { return _managedDataPool; } }


        public bool IsEmpty
        {
            get { return _managedDataPool.Lenght <= 0; }
        }

        public OperateResult Initilize()
        {
            OperateResult initilizeResult = OperateResult.CreateFailedResult();
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\WcsArchitectureConfig.xml";
                if (!File.Exists(strFileName))
                {
                    initilizeResult.IsSuccess = true;
                    initilizeResult.Message = "不存在WcsArchitectureConfig.xml配置文件，不进行启动WcsArchitectureConfig";
                    return initilizeResult;
                }
                ArchitectureConfig = (ArchitectureConfig)XmlSerializerHelper.LoadFromXml(strFileName, typeof(ArchitectureConfig));
                OperateResult startServiceResult = InitilizeArchitectureItem();

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

        private OperateResult InitilizeArchitectureItem()
        {
            OperateResult startServiceResult = OperateResult.CreateFailedResult();
            try
            {
                if (ArchitectureConfig == null)
                {
                    startServiceResult.IsSuccess = false;
                    startServiceResult.Message = "WcsArchitectureConfig配置文件读取失败，确认配置文件是否存在或配置文件格式";
                    return startServiceResult;
                }

                foreach (ArchitectureProperty architectureProperty in ArchitectureConfig.ArchitectureItemList)
                {
                    ArchitectureDataAbstract architectureItem =
                        (ArchitectureDataAbstract) Assembly.Load(architectureProperty.NameSpace)
                            .CreateInstance(architectureProperty.NameSpace + "." + architectureProperty.ClassName);
                    if (architectureItem == null)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = string.Format("ArchitectureData:{2} 命名空间：{0} 类名：{1} 反射出错",
                            architectureProperty.NameSpace, architectureProperty.ClassName, architectureProperty.Name);
                        return startServiceResult;
                    }
                    architectureItem.Id = architectureProperty.Id;
                    architectureItem.Name = architectureProperty.Name;
                    architectureItem.NameSpace = architectureProperty.NameSpace;
                    architectureItem.ClassName = architectureProperty.ClassName;
                    architectureItem.Id = architectureProperty.Id;
                    architectureItem.IsShowUi = architectureProperty.IsShowUi;
                    architectureItem.IconKind = architectureProperty.IconKind;

                    OperateResult initilizeResult = architectureItem.Initilize();
                    if (!initilizeResult.IsSuccess)
                    {
                        startServiceResult.IsSuccess = false;
                        startServiceResult.Message = initilizeResult.Message;
                        return startServiceResult;
                    }
                    OperateResult addResult = Add(architectureItem);
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

        public OperateResult Add(ArchitectureDataAbstract data)
        {
            return _managedDataPool.AddPool(data);
        }

        public OperateResult Delete(int id)
        {
            return _managedDataPool.RemovePool(id);
        }

        public ArchitectureDataAbstract Find(int id)
        {
            OperateResult<ArchitectureDataAbstract> findResult = _managedDataPool.FindData(id);
            return findResult.Content;
        }

        public ArchitectureDataAbstract Find(string name)
        {
            OperateResult<ArchitectureDataAbstract> findResult = _managedDataPool.FindData(name);
            return findResult.Content;
        }


        public List<ArchitectureDataAbstract> GetAllData()
        {
            return ManagedDataPool.DataPool;
        }

        public OperateResult Update(ArchitectureDataAbstract data)
        {
            return ManagedDataPool.UpdatePool(data);
        }


    }
}
