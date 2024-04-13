using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.Authorize;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace CLDC.CLWS.CLWCS.Service.WmsView.SqlSugar
{
    public sealed class WmsDataSqlSugar : WmsDataAbstract
    {
        public WmsDataSqlSugar(IDbHelper dbHelper)
            : base(dbHelper)
        {
        }

        public override OperateResult CreateNewInOrder(Order inOrder)
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

        public override OperateResult CreateNewArea(Area model)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(model) > 0;
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

        public override OperateResult CreateSoundLight(SoundLight model)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(model) > 0;
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

        public override OperateResult UpdateArea(Area model)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Update<Area>(t => new Area
                {
                    AreaCode = model.AreaCode,
                    AreaName = model.AreaName,
                    ROW = model.ROW,
                    COLUMN = model.COLUMN,
                    Status = model.Status,
                }, t => t.Id == model.Id) > 0;
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

        public override OperateResult UpdateSoundLight(SoundLight model)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Update<SoundLight>(t => new SoundLight
                {
                    Area = model.Area,
                    Team = model.Team,
                    Sound = model.Sound,
                    Light = model.Light,
                    SoundContent = model.SoundContent,
                    LightCode = model.LightCode,
                }, t => t.Id == model.Id) > 0;
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


        public override OperateResult DeleteArea(Area model)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                bool done = DbHelper.Delete<Area>(t => t.Id == model.Id) > 0;
                result.IsSuccess = done;
                result.Message = "操作成功";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult DeleteSoundLight(SoundLight model)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                bool done = DbHelper.Delete<SoundLight>(t => t.Id == model.Id) > 0;
                result.IsSuccess = done;
                result.Message = "操作成功";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public override OperateResult CreateNewOrderDetail(List<OrderDetail> inOrder)
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

        public override OperateResult CreateNewInventory(List<Inventory> inventorys)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                bool result = DbHelper.Add(inventorys) > 0;
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


        public override OperateResult DeleteInOrder(Order inOrder)
        {
            throw new NotImplementedException();
        }

        public override OperateResult EditInOrder(Order inOrder)
        {
            throw new NotImplementedException();
        }

        public override List<Order> GetAllInOrderList()
        {
            List<Order> inOrderList = null;
            try
            {
                inOrderList = DbHelper.QueryList<Order>();
            }
            catch
            { }
            return inOrderList;
        }

        public override OperateResult<List<Order>> GetInOrderPageList(Expression<Func<Order, bool>> whereLambda = null)
        {
            OperateResult<List<Order>> result = OperateResult.CreateFailedResult<List<Order>>("无数据");
            try
            {
                //dataPool = DbHelper.QueryPageList((int)filterModel.PageIndex, (int)filterModel.PageSize,
                //    "LOG_RECORD_DATE DESC", out totalCount, where);
                List<Order> list = DbHelper.QueryList(whereLambda).OrderByDescending(x=>x.CreatedTime).ToList();
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

        public override OperateResult<List<Order>> GetOrderAndMaterList(Expression<Func<Order, bool>> whereLambda = null)
        {
            OperateResult<List<Order>> result = OperateResult.CreateFailedResult<List<Order>>("无数据");
            try
            {
                List<Order> list = DbHelper.QuerySqlList<Order>("SELECT top(100) a.MaterialDesc,b.InOutType,b.Reason,b.CreatedTime FROM [t_OrderDetail] as a " +
                    "left join [t_Order] as b on a.ordersn=b.ordersn where a.IsDeleted=0 and b.IsDeleted=0 Order by b.CreatedTime Desc");
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

        public override OperateResult<List<Inventory>> GetInventoryPageList(string where)
        {
            OperateResult<List<Inventory>> result = OperateResult.CreateFailedResult<List<Inventory>>("无数据");
            try
            {
                List<Inventory> list = DbHelper.QuerySqlList<Inventory>("SELECT [AreaName],[AreaTeam],[MaterialDesc],[ShelfName],SUM([Qty]) as Qty,[Status]" +
                    " FROM [ArmyOfficeWms].[dbo].[t_Inventory] Where 1=1 " + where + " GROUP BY [AreaName],[AreaTeam],[MaterialDesc],[ShelfName],[Status] ");
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

        public override OperateResult<List<Area>> GetAreaPageList(Expression<Func<Area, bool>> whereLambda = null)
        {
            OperateResult<List<Area>> result = OperateResult.CreateFailedResult<List<Area>>("无数据");
            try
            {
                List<Area> list = DbHelper.QueryList(whereLambda);
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

        public override OperateResult<List<SoundLight>> GetSoundLightPageList(Expression<Func<SoundLight, bool>> whereLambda = null)
        {
            OperateResult<List<SoundLight>> result = OperateResult.CreateFailedResult<List<SoundLight>>("无数据");
            try
            {
                List<SoundLight> list = DbHelper.QueryList(whereLambda);
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


        public override double GetInvQtyByStatus(InvStatusEnum status)
        {
            double invQty = 0;
            try
            {
                invQty = DbHelper.QueryCount<Inventory>(t => t.Status == status);
            }
            catch
            { }
            return invQty;
        }

        public override Inventory GetInventory(string Barcode)
        {
            Inventory inventory = null;
            try
            {
                inventory = DbHelper.Query<Inventory>(t => t.Barcode == Barcode);
            }
            catch
            { }
            return inventory;
        }

        public override OperateResult UpdateInventory(int invStatus, List<string> barcodes)
        {
            OperateResult createResult = OperateResult.CreateFailedResult();
            try
            {
                StringBuilder idsStr = new StringBuilder();
                for (int i = 0; i < barcodes.Count; i++)
                {
                    if (i > 0)
                    {
                        idsStr.Append(",");
                    }
                    idsStr.Append("'").Append(barcodes[i]).Append("'");
                }

                string sql = "UPDATE [dbo].[t_Inventory] SET [Status]=" + invStatus + " WHERE 1=1 AND [Barcode] in(" + idsStr + ")";
                bool result = DbHelper.ExecuteNonQuery(sql) > 0;
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

        public override List<Material> GetMaterialList(string area)
        {
            List<Material> materialList = null;
            try
            {
                materialList = DbHelper.QueryList<Material>(t => t.AreaCode == area);
            }
            catch
            { }
            return materialList;
        }

        public override Material GetMaterial(string materCode)
        {
            Material material = null;
            try
            {
                material = DbHelper.QueryList<Material>().FirstOrDefault(x => x.MaterialCode == materCode);
            }
            catch
            { }
            return material;
        }

        public override OrderDetail GetOrderDetail(string orderSn)
        {
            OrderDetail model = null;
            try
            {
                model = DbHelper.QueryList<OrderDetail>().FirstOrDefault(x => x.OrderSN == orderSn && x.IsDeleted == false);
            }
            catch
            { }
            return model;
        }

        public override List<Area> GetAreaList(string code)
        {
            List<Area> areaList = null;
            try
            {
                areaList = DbHelper.QueryList<Area>(t => t.AreaCode.Contains(code) && t.Status == 1);
            }
            catch
            { }
            return areaList;
        }

        public override List<SoundLight> GetSoundLightList(string code)
        {
            List<SoundLight> areaList = null;
            try
            {
                areaList = DbHelper.QueryList<SoundLight>(t => t.Area.Contains(code));
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

        public override Shelf GetShelf(string barcode)
        {
            Shelf model = null;
            try
            {
                model = DbHelper.QueryList<Shelf>().FirstOrDefault(x => x.Barcode == barcode);
            }
            catch
            { }
            return model;
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
