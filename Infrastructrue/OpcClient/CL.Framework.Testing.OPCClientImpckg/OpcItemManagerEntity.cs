using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.Testing.OPCClientImpPckg
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("OPC_ITEMMANAGER", "")]
    public class OpcItemManagerEntity
    {
        [SugarColumn(ColumnName = "ITEMID", IsNullable = false, IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "ID主键")]
        public decimal ITEMID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "GROUPNAME", Length = 50, IsNullable = true, ColumnDescription = "")]
		public string GROUPNAME { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn(ColumnName = "ITEMNAME", Length = 100, IsNullable = true, ColumnDescription = "")]
		public string ITEMNAME { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[SugarColumn(ColumnName = "VALUE", Length = 100, IsNullable = true, ColumnDescription = "")]
		public string VALUE { get; set; }
	}
}
