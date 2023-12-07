using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    public class WmsDataService
    {
        private readonly WmsDataAbstract _wmsDataAccess;
        public WmsDataService(WmsDataAbstract wmsDataAccess)
        {
            _wmsDataAccess = wmsDataAccess;
        }

        public List<Material> GetMaterialList(string name)
        {
            return _wmsDataAccess.GetMaterialList(name);
        }

        public List<Material> GetUnitList(string name)
        {
            return _wmsDataAccess.GetUnitList(name);
        }

        public List<Area> GetAreaList(string name)
        {
            return _wmsDataAccess.GetAreaList(name);
        }

        public List<Shelf> GetShelfList(string area)
        {
            return _wmsDataAccess.GetShelfList(area);
        }

        public OperateResult CreateNewInOrder(InOrder inOrder)
        {
            return _wmsDataAccess.CreateNewInOrder(inOrder);
        }

        public OperateResult CreateNewOrderDetail(InOrderDetail orderDetail)
        {
            return _wmsDataAccess.CreateNewOrderDetail(orderDetail);
        }

        public OperateResult CreateNewInventory(Inventory inventory)
        {
            return _wmsDataAccess.CreateNewInventory(inventory);
        }

        public OperateResult<List<InOrder>> GetInOrderPageList(Expression<Func<InOrder, bool>> whereLambda = null)
        {
            return _wmsDataAccess.GetInOrderPageList(whereLambda);
        }

        public OperateResult<List<Inventory>> GetInventoryPageList(Expression<Func<Inventory, bool>> whereLambda = null)
        {
            return _wmsDataAccess.GetInventoryPageList(whereLambda);
        }

        public double GetInvQtyByStatus(InvStatusEnum status)
        {
            return _wmsDataAccess.GetInvQtyByStatus(status);
        }

    }
}
