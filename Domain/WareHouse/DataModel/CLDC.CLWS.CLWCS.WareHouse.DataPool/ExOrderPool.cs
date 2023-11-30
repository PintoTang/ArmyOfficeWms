using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.DataPool
{
    /// <summary>
    /// 指令池
    /// </summary>
    public class ExOrderPool : ICloneable
    {
        private List<ExOrder> _container = new List<ExOrder>();

        /// <summary>
        /// 数据池的总数量
        /// </summary>
        public int Count()
        {
            return _container.Count;
        }


        /// <summary>
        /// 获取指定状态的数量
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public int Count(StatusEnum status)
        {
            lock (_container)
            {
                return Container.Count(o => o.Status.Equals(status));
            }
        }

        /// <summary>
        /// 容器的克隆 用于界面显示
        /// </summary>
        public List<ExOrder> ContainerClone
        {
            get { return (List<ExOrder>)Clone(); }
        }

        /// <summary>
        /// 指令池内容
        /// </summary>
        public List<ExOrder> Container
        {
            get { return _container; }
            set
            {
                _container = value;
            }
        }
        /// <summary>
        /// 添加指令到指令池
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OperateResult AddPool(ExOrder order)
        {
            OperateResult result = new OperateResult();
            lock (_container)
            {
                try
                {
                    if (order == null)
                    {
                        result.IsSuccess = false;
                        result.Message = "参数传入空对象";
                        return result;
                    }
                    if (_container.Exists(o => o.OrderId.Equals(order.OrderId)))
                    {
                        UpdatePool(order);

                    }
                    else
                    {
                        _container.Add(order);
                    }
                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            return result;
        }

        /// <summary>
        /// 移除指定的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public OperateResult RemovePool(ExOrder data)
        {
            if (data == null)
            {
                return OperateResult.CreateFailedResult("删除参数为空", 1);
            }
            lock (_container)
            {
                if (!_container.Exists(d => d.OrderId.Equals(data.OrderId)))
                {
                    OperateResult failResult = OperateResult.CreateFailedResult();
                    failResult.Message = string.Format("不存在：{0}", data.ToString());
                    return failResult;
                }
                ExOrder removeData = _container.FirstOrDefault(d => d.OrderId.Equals(data.OrderId));
                bool result = _container.Remove(removeData);
                if (result)
                {
                    return OperateResult.CreateSuccessResult(string.Format("添加成功：{0}", data.ToString()));
                }
                return OperateResult.CreateFailedResult(string.Format("移除失败：{0}", data.ToString()), 1);
            }
        }

        /// <summary>
        /// 更新指定指令的指令池信息，不存在则添加，存在则修改
        /// </summary>
        /// <param name="updateOrder">需要更新的指令信息</param>
        /// <returns></returns>
        public OperateResult UpdatePool(ExOrder updateOrder)
        {
            if (updateOrder == null)
            {
                return OperateResult.CreateFailedResult("参数对象为空", 1);
            }
            lock (_container)
            {
                OperateResult<ExOrder> getOrderResult = ObtainData(updateOrder.OrderId);
                if (!getOrderResult.IsSuccess)
                {
                    return AddPool(updateOrder);
                }
                ExOrder oldOrder = getOrderResult.Content;
                oldOrder.AllocateFailTime = updateOrder.AllocateFailTime;
                oldOrder.AllocateTime = updateOrder.AllocateTime;
                oldOrder.CurHandlerId = updateOrder.CurHandlerId;
                oldOrder.FinishType = updateOrder.FinishType;
                oldOrder.HelperRGVNo = updateOrder.HelperRGVNo;
                if (updateOrder.NextAddr != null) oldOrder.NextAddr = updateOrder.NextAddr.Clone();
                oldOrder.IsAllocated = updateOrder.IsAllocated;
                if (updateOrder.StartAddr != null) oldOrder.StartAddr = updateOrder.StartAddr.Clone();
                oldOrder.Status = updateOrder.Status;
                oldOrder.BackFlag = updateOrder.BackFlag;
                oldOrder.CreateTime = updateOrder.CreateTime;
                if (updateOrder.CurrAddr != null) oldOrder.CurrAddr = updateOrder.CurrAddr.Clone();
                if (updateOrder.DestAddr != null) oldOrder.DestAddr = updateOrder.DestAddr.Clone();
                oldOrder.DocumentCode = updateOrder.DocumentCode;
                oldOrder.OrderPriority = updateOrder.OrderPriority;
                oldOrder.OrderType = updateOrder.OrderType;
                oldOrder.PileNo = updateOrder.PileNo;
                oldOrder.PriorDeviceList = updateOrder.PriorDeviceList;

                OperateResult updateResult = OperateResult.CreateSuccessResult("根据指令编号：{0} 更新指令成功");
                return updateResult;
            }
        }

        /// <summary>
        /// 根据指令ID 获取指令信息
        /// </summary>
        /// <param name="orderId">指令ID</param>
        /// <returns></returns>
        public OperateResult<ExOrder> ObtainData(int orderId)
        {
            lock (_container)
            {
                if (_container.Exists(d => d.OrderId.Equals(orderId)))
                {
                    ExOrder tempData = _container.FirstOrDefault(d => d.OrderId.Equals(orderId));
                    return OperateResult.CreateSuccessResult(tempData);
                }
                return OperateResult.CreateFailedResult(default(ExOrder), "获取失败");
            }
        }
        public object Clone()
        {
            List<ExOrder> cloneContainer = new List<ExOrder>();
            if (this.Count() <= 0)
            {
                return cloneContainer;
            }
            else
            {

                lock (this._container)
                {
                    foreach (ExOrder exOrder in _container)
                    {
                        cloneContainer.Add(exOrder);
                    }
                }
            }
            return cloneContainer;
        }
    }
}
