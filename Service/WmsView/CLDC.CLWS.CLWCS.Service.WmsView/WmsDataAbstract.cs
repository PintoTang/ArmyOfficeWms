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
        public abstract OperateResult CreateNewInOrder(InOrder inOrder);

        /// <summary>
        /// 创建入库单明细
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult CreateNewOrderDetail(InOrderDetail orderDetail);

        /// <summary>
        /// 创建库存
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult CreateNewInventory(Inventory inventory);

        /// <summary>
        /// 获取所有入库单信息
        /// </summary>
        /// <returns></returns>
        public abstract List<InOrder> GetAllInOrderList();

        public abstract OperateResult<List<InOrder>> GetInOrderPageList(Expression<Func<InOrder, bool>> whereLambda = null);

        public abstract OperateResult<List<Inventory>> GetInventoryPageList(Expression<Func<Inventory, bool>> whereLambda = null);

        public abstract double GetInvQtyByStatus(InvStatusEnum status);

        /// <summary>
        /// 根据名称获取物料列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract List<Material> GetMaterialList(string name);

        /// <summary>
        /// 获取单位列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract List<Material> GetUnitList(string name);

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

        /// <summary>
        /// 删除入库单
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult DeleteInOrder(InOrder inOrder);

        public abstract OperateResult EditInOrder(InOrder inOrder);

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
