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

        public List<Material> GetMaterialList(string area)
        {
            return _wmsDataAccess.GetMaterialList(area);
        }

        public Material GetMaterial(string materCode)
        {
            return _wmsDataAccess.GetMaterial(materCode);
        }
        public OrderDetail GetOrderDetail(string orderSn)
        {
            return _wmsDataAccess.GetOrderDetail(orderSn);
        }

        public List<Area> GetAreaList(string code)
        {
            return _wmsDataAccess.GetAreaList(code);
        }

        public List<SoundLight> GetSoundLightList(string code)
        {
            return _wmsDataAccess.GetSoundLightList(code);
        }

        public List<Shelf> GetShelfList(string area)
        {
            return _wmsDataAccess.GetShelfList(area);
        }

        public Shelf GetShelf(string barcode)
        {
            return _wmsDataAccess.GetShelf(barcode);
        }

        public OperateResult CreateNewInOrder(Order inOrder)
        {
            return _wmsDataAccess.CreateNewInOrder(inOrder);
        }

        public OperateResult CreateNewArea(Area model)
        {
            return _wmsDataAccess.CreateNewArea(model);
        }

        public OperateResult CreateSoundLight(SoundLight model)
        {
            return _wmsDataAccess.CreateSoundLight(model);
        }

        public OperateResult UpdateSoundLight(SoundLight model)
        {
            return _wmsDataAccess.UpdateSoundLight(model);
        }

        public OperateResult DeleteSoundLight(SoundLight model)
        {
            return _wmsDataAccess.DeleteSoundLight(model);
        }

        public OperateResult UpdateArea(Area model)
        {
            return _wmsDataAccess.UpdateArea(model);
        }

        public OperateResult DeleteArea(Area model)
        {
            return _wmsDataAccess.DeleteArea(model);
        }

        public OperateResult CreateNewOrderDetail(List<OrderDetail> orderDetail)
        {
            return _wmsDataAccess.CreateNewOrderDetail(orderDetail);
        }

        public OperateResult CreateNewInventory(List<Inventory> inventorys)
        {
            return _wmsDataAccess.CreateNewInventory(inventorys);
        }

        public OperateResult<List<Order>> GetInOrderPageList(Expression<Func<Order, bool>> whereLambda = null)
        {
            return _wmsDataAccess.GetInOrderPageList(whereLambda);
        }

        public OperateResult<List<Order>> GetOrderAndMaterList(Expression<Func<Order, bool>> whereLambda = null)
        {
            return _wmsDataAccess.GetOrderAndMaterList(whereLambda);
        }

        public OperateResult<List<Inventory>> GetInventoryPageList(string where)
        {
            return _wmsDataAccess.GetInventoryPageList(where);
        }

        public OperateResult<List<Area>> GetAreaPageList(Expression<Func<Area, bool>> whereLambda = null)
        {
            return _wmsDataAccess.GetAreaPageList(whereLambda);
        }

        public OperateResult<List<SoundLight>> GetSoundLightPageList(Expression<Func<SoundLight, bool>> whereLambda = null)
        {
            return _wmsDataAccess.GetSoundLightPageList(whereLambda);
        }

        public double GetInvQtyByStatus(InvStatusEnum status)
        {
            return _wmsDataAccess.GetInvQtyByStatus(status);
        }

        public Inventory GetInventory(string Barcode)
        {
            return _wmsDataAccess.GetInventory(Barcode);
        }

        public OperateResult UpdateInventory(int invStatus, List<string> barcodes)
        {
            return _wmsDataAccess.UpdateInventory(invStatus, barcodes);
        }

    }
}
