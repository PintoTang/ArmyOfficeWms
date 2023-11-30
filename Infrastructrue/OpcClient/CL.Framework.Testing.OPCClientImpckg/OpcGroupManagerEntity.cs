using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.Testing.OPCClientImpPckg
{
    [SugarTable("OPC_GROUPMANAGER", "")]
    public class OpcGroupManagerEntity
    {
        [SugarColumn(ColumnName = "GROUPID", IsNullable = false, IsPrimaryKey = true, IsIdentity = true, ColumnDescription = "ID主键")]
        public decimal GROUPID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnName = "GROUPNAME", Length = 100, IsNullable = false, ColumnDescription = "")]
        public string GROUPNAME { get; set; }
    }
}
