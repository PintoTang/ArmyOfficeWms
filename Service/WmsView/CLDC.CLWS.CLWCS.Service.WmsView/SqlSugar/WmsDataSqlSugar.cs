using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CLDC.CLWS.CLWCS.Service.WmsView.SqlSugar
{
    public sealed class WmsDataSqlSugar : WmsDataAbstract
    {
        public WmsDataSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult CreateNewInOrder(InOrder inOrder)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(inOrder) > 0;
                if (!result)
                {
                    createResult.Message = "操作数据库错误";
                }
                createResult.IsSuccess = result;
                return createResult;
            }
            catch (Exception ex)
            {
                createResult.IsSuccess = false;
                createResult.Message = OperateResult.ConvertException(ex);
            }
            return createResult;
        }

        public override OperateResult CreateNewOrderDetail(InOrderDetail inOrder)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(inOrder) > 0;
                if (!result)
                {
                    createResult.Message = "操作数据库错误";
                }
                createResult.IsSuccess = result;
                return createResult;
            }
            catch (Exception ex)
            {
                createResult.IsSuccess = false;
                createResult.Message = OperateResult.ConvertException(ex);
            }
            return createResult;
        }

        public override OperateResult CreateNewInventory(Inventory inventory)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(inventory) > 0;
                if (!result)
                {
                    createResult.Message = "操作数据库错误";
                }
                createResult.IsSuccess = result;
                return createResult;
            }
            catch (Exception ex)
            {
                createResult.IsSuccess = false;
                createResult.Message = OperateResult.ConvertException(ex);
            }
            return createResult;
        }


        public override OperateResult DeleteInOrder(InOrder inOrder)
        {
            throw new NotImplementedException();
        }

        public override OperateResult EditInOrder(InOrder inOrder)
        {
            throw new NotImplementedException();
        }

        public override List<InOrder> GetAllInOrderList()
        {
            List<InOrder> inOrderList = null;
            try
            {
                inOrderList = DbHelper.QueryList<InOrder>();
            }
            catch
            { }
            return inOrderList;
        }

        public override OperateResult<List<InOrder>> GetInOrderPageList(Expression<Func<InOrder, bool>> whereLambda = null)
        {

            OperateResult<List<InOrder>> result = OperateResult.CreateFailedResult<List<InOrder>>("无数据");
            try
            {
                List<InOrder> list = DbHelper.QueryList(whereLambda);
                result.IsSuccess = true;
                result.Content = list;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }

        public override OperateResult<List<Inventory>> GetInventoryPageList(Expression<Func<Inventory, bool>> whereLambda = null)
        {

            OperateResult<List<Inventory>> result = OperateResult.CreateFailedResult<List<Inventory>>("无数据");
            try
            {
                List<Inventory> list = DbHelper.QueryList(whereLambda);
                result.IsSuccess = true;
                result.Content = list;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);

            }
            return result;
        }


        public override List<Material> GetMaterialList(string name)
        {
            List<Material> materialList = null;
            try
            {
                materialList = DbHelper.QueryList<Material>(t => t.MaterialDesc.Contains(name));
            }
            catch
            { }
            return materialList;
        }

        public override List<Material> GetUnitList(string name)
        {
            List<Material> materialList = null;
            try
            {
                materialList = DbHelper.QueryList<Material>().Select(x=>new Material { UnitId=x.UnitId,UnitName=x.UnitName}).Distinct().ToList();
            }
            catch
            { }
            return materialList;
        }

        public override List<Area> GetAreaList(string name)
        {
            List<Area> areaList = null;
            try
            {
                areaList = DbHelper.QueryList<Area>();
            }
            catch
            { }
            return areaList;
        }

        public override List<Shelf> GetShelfList(string area)
        {
            List<Shelf> shelfList = null;
            try
            {
                shelfList = DbHelper.QueryList<Shelf>(x=>x.AreaCode.Contains(area));
            }
            catch
            { }
            return shelfList;
        }


        public override OperateResult Insert(WmsDataModel data)
        {
            throw new NotImplementedException();
        }

        public override OperateResult IsExist(WmsDataModel data)
        {
            throw new NotImplementedException();
        }

        public override OperateResult Update(WmsDataModel data)
        {
            throw new NotImplementedException();
        }

        public override long GetMaxID(string fieldName, string tableName, string where)
        {
            string strsql = "select ISNULL(max(" + fieldName + "),0) from " + tableName + " " + where;
            object obj = DbHelper.QuerySqlScalar(strsql);
            return Convert.ToInt64(obj);
        }

    }
}
