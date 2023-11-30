using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.WareHouse.DataModel
{
    public class BaseQueryInput<T>
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int Limit { set; get; } = 50;

        /// <summary>
        /// 关键字
        /// </summary>
        public string Key
        {
            get; set;
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression<Func<T, bool>> WhereLambda
        {
            get;set;
        }

        /// <summary>
        /// 排序列
        /// </summary>
        public string SortField { get; set; }


        /// <summary>
        /// 排序类型
        /// </summary>
        public string SortOrder { get; set; }

        private string _fullSortFields;
        /// <summary>
        /// 排序全拼
        /// </summary>
        public string FullSortFields
        {
            get
            {
                if (!string.IsNullOrEmpty(_fullSortFields))
                {
                    return _fullSortFields;
                }
                if (string.IsNullOrWhiteSpace(SortField))
                {
                    return "";
                }
                string[] orderFieldArr = SortField.Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (orderFieldArr == null || orderFieldArr.Length == 0)
                {
                    return "";
                }

                string[] orderWayArr = null;
                if (!string.IsNullOrWhiteSpace(SortOrder))
                {
                    orderWayArr = SortOrder.Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < orderFieldArr.Length; i++)
                {
                    string orderField = orderFieldArr[i];
                    string orderWay = "ASC";
                    if (orderWayArr != null && orderWayArr.Length >= i + 1)
                    {
                        if (orderWayArr[i].ToUpper() == "ASC" || orderWayArr[i].ToUpper() == "DESC")
                        {
                            orderWay = orderWayArr[i];
                        }
                    }
                    sb.Append(orderField).Append(" ").Append(orderWay).Append(",");
                }
                _fullSortFields = sb.ToString().TrimEnd(',');
                return _fullSortFields;
            }
        }
    }
}
