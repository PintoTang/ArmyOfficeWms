using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// 仓位信息
    /// </summary>
   public class CellData
	{
	   /// <summary>
	   /// 仓位编号，遵循协议中地址命名规范。如：Cell:1_1_1_H
	   /// </summary>
	   public string CELLNO { get; set; }
	   /// <summary>
	   /// 库区代码
	   /// </summary>
	   public string AREA_CODE { get; set; }
	   /// <summary>
	   /// 仓库代码
	   /// </summary>
	   public string WH_CODE { get; set; }
	   /// <summary>
	   /// 仓位状态代码
	   /// </summary>
	   public int CS_CODE { get; set; }
	   /// <summary>
	   /// 仓位所在排编号
	   /// </summary>
	   public int LINENO { get; set; }
	   /// <summary>
	   /// 仓位所在层编号
	   /// </summary>
	   public int LAYERNO { get; set; }
	   /// <summary>
	   /// 仓位所在列编号
	   /// </summary>
	   public int COLUMNNO { get; set; }
	   /// <summary>
	   /// 仓位深浅表示，0表示浅库位，1表示深库位。单深位用默认值0
	   /// </summary>
	   public int DEPTH { get; set; }
	   /// <summary>
	   /// 备注
	   /// </summary>
	   public string REMARK { get; set; }
	   /// <summary>
	   /// 根据数据库的数据进行格式化成对象
	   /// </summary>
	   /// <param name="row"></param>
	   /// <returns></returns>
	   public static explicit operator CellData(DataRow row)
	   {
		   CellData cellData = new CellData();
		   cellData.CELLNO = row["cellNo"].ToString();
		   cellData.AREA_CODE = row["area_code"].ToString().Trim();
		   cellData.WH_CODE = row["wh_code"].ToString().Trim();
		   cellData.CS_CODE = Convert.ToInt32(row["cs_code"].ToString().Trim());
		   cellData.LINENO = Convert.ToInt32(row["lintNo"].ToString().Trim());
		   cellData.LAYERNO = Convert.ToInt32(row["layerNo"].ToString().Trim());
		   cellData.COLUMNNO = Convert.ToInt32(row["columnNo"].ToString().Trim());
		   cellData.DEPTH = Convert.ToInt32(row["depth"].ToString().Trim());
		   cellData.REMARK = row["remark"].ToString().Trim();

		   return cellData;
	   }

	}
}
