using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.Infrastructrue.DbHelper;
using CLDC.CLWS.CLWCS.Service.WmsView.Model;
using System;
using System.Collections.Generic;

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
    }
}
