using System.Collections.Generic;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DataPool;

namespace CLDC.CLWS.CLWCS.WareHouse.ViewModel
{
    public class ViewModelManage
    {
        private static ViewModelManage viewModelManage;

        /// <summary>
        /// 设备管理单例
        /// </summary>
        public static ViewModelManage Instance
        {
            get
            {
                if (viewModelManage == null)
                    viewModelManage = new ViewModelManage();
                return viewModelManage;
            }
        }
        private readonly DataManageablePool<WareHouseViewModelBase> viewModelContainer = new DataManageablePool<WareHouseViewModelBase>();
        public DataManageablePool<WareHouseViewModelBase> Container
        {
            get
            {
                return viewModelContainer;
            }
        }

        public OperateResult Add(WareHouseViewModelBase viewModel)
        {
            return viewModelContainer.AddPool(viewModel);
        }

        public OperateResult Delete(int viewModelId)
        {
            return viewModelContainer.RemovePool(viewModelId);
        }

        public OperateResult Update(WareHouseViewModelBase viewModel)
        {
            return viewModelContainer.UpdatePool(viewModel);
        }

        public WareHouseViewModelBase Find(int viewModelId)
        {
            OperateResult<WareHouseViewModelBase> findResult = viewModelContainer.FindData(viewModelId);
            return findResult.Content;
        }

        public WareHouseViewModelBase Find(string viewModelName)
        {
            OperateResult<WareHouseViewModelBase> findResult = viewModelContainer.FindData(viewModelName);
            return findResult.Content;
        }

        public WareHouseViewModelBase FindDeviceViewModel(int deviceId)
        {
            OperateResult<WareHouseViewModelBase> findResult = viewModelContainer.FindData(d => d.ViewModelType.Equals(ViewModelTypeEnum.Device) && d.Id.Equals(deviceId));
            return findResult.Content;
        }

        public OperateResult<WareHouseViewModelBase> FindWorkerViewModel(int workerId)
        {
            OperateResult<WareHouseViewModelBase> findResult = viewModelContainer.FindData(d => d.ViewModelType.Equals(ViewModelTypeEnum.Woker) && d.Id.Equals(workerId));
            return findResult;
        }

        public List<WareHouseViewModelBase> GetAllData()
        {
            return viewModelContainer.DataPool;
        }

        public List<WareHouseViewModelBase> GetAllWorkerViewModel()
        {
            OperateResult<List<WareHouseViewModelBase>> result = viewModelContainer.FindAllData(v => v.ViewModelType.Equals(ViewModelTypeEnum.Woker));
            return result.Content;
        }

        public List<WareHouseViewModelBase> GetAllDeviceViewModel()
        {
            OperateResult<List<WareHouseViewModelBase>> result = viewModelContainer.FindAllData(v => v.ViewModelType.Equals(ViewModelTypeEnum.Device));
            return result.Content;
        }
    }
}
