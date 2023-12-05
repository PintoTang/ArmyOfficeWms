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
        /// 获取所有入库单信息
        /// </summary>
        /// <returns></returns>
        public abstract List<InOrder> GetAllInOrderList();

        /// <summary>
        /// 删除入库单
        /// </summary>
        /// <param name="inOrder"></param>
        /// <returns></returns>
        public abstract OperateResult DeleteInOrder(InOrder inOrder);

        public abstract OperateResult EditInOrder(InOrder inOrder);

    }
}
