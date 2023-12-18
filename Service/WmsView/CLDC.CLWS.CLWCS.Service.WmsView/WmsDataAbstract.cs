using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.Authorize.DataMode;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.Service.WmsView
{
    public abstract class WmsDataAbstract : DatabaseBusinessAbstract<WmsDataModel>
    {
        protected WmsDataAbstract(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        /// <summary>
        /// 创建入库单
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult CreateNewInOrder(Order inOrder);

        /// <summary>
        /// 创建入库单明细
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult CreateNewOrderDetail(List<OrderDetail> orderDetail);

        /// <summary>
        /// 创建库存
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult CreateNewInventory(List<Inventory> inventorys);

        /// <summary>
        /// 获取所有入库单信息
        /// </summary>
        /// <returns></returns>
        public abstract List<Order> GetAllInOrderList();

        public abstract OperateResult<List<Order>> GetInOrderPageList(Expression<Func<Order, bool>> whereLambda = null);

        public abstract OperateResult<List<Order>> GetOrderAndMaterList(Expression<Func<Order, bool>> whereLambda = null);

        public abstract OperateResult<List<Inventory>> GetInventoryPageList(string where);

        public abstract double GetInvQtyByStatus(InvStatusEnum status);

        public abstract Inventory GetInventory(string Barcode);

        public abstract OperateResult UpdateInventory(int invStatus, List<string> barcodes);

        /// <summary>
        /// 根据名称获取物料列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract List<Material> GetMaterialList(string name);

        /// <summary>
        /// 获取物资信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract Material GetMaterial(string materCode);

        public abstract OrderDetail GetOrderDetail(string orderSn);

        /// <summary>
        /// 获取存放区域
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract List<Area> GetAreaList(string name);

        /// <summary>
        /// 获取存放货架
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public abstract List<Shelf> GetShelfList(string area);

        public abstract Shelf GetShelf(string barcode);

        /// <summary>
        /// 删除入库单
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult DeleteInOrder(Order inOrder);

        public abstract OperateResult EditInOrder(Order inOrder);

        /// <summary>
		/// 取指令表中最大的指令ID
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="tableName"></param>
		/// <param name="where"></param>
		/// <returns></returns>
		public abstract long GetMaxID(string fieldName, string tableName, string where);

    }
}
