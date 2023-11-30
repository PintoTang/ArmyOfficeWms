using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CL.WCS.DataModelPckg
{
    /// <summary>
    /// LED样式
    /// </summary>
	public class EqLedStyleStruct
	{
		/// <summary>
		/// 显示样式ID
		/// </summary>
		public int StyleId;
		/// <summary>
		/// 字体区域：左上角X坐标
		/// </summary>
		public int FontAreaIx;
		/// <summary>
		/// 字体区域：左上角Y坐标
		/// </summary>
		public int FontAreaIy;
		/// <summary>
		/// 字体区域：高度
		/// </summary>
		public int High;
		/// <summary>
		/// 字体区域：宽度
		/// </summary>
		public int Wide;
		/// <summary>
		/// 字体区域：边框模式
		/// </summary>
		public int FrameMode;
		/// <summary>
		/// 字体区域：边框颜色
		/// </summary>
		public int FrameColor;
		/// <summary>
		/// 字体样式：字体名称
		/// </summary>
		public string FontName;
		/// <summary>
		/// 字体样式：字体大小
		/// </summary>
		public int Size;
		/// <summary>
		/// 字体样式：是否加粗
		/// </summary>
		public bool IsBold;
		/// <summary>
		/// 字体样式：是否斜体
		/// </summary>
		public bool IsItaic;
		/// <summary>
		/// 字体样式：是否加下划线
		/// </summary>
		public bool IsUnderLine;
		/// <summary>
		/// 字体样式：字体颜色
		/// </summary>
		public string FontColor;
		/// <summary>
		/// 字体样式：行距
		/// </summary>
		public int RowSpace;
		/// <summary>
		/// 字体样式：水平样式(0：左对齐，1：居中，2右对齐)
		/// </summary>
		public int AlignStyle;
		/// <summary>
		/// 字体样式：竖直样式(0：顶对齐，1：上下居中，2：底对齐)
		/// </summary>
		public int ValignerStyle;
        /// <summary>
        /// 根据数据库的数据进行格式化成对象
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
		public static explicit operator EqLedStyleStruct(DataRow row)
		{
			EqLedStyleStruct pLEDStyleStruct = new EqLedStyleStruct();

			pLEDStyleStruct.StyleId = string.IsNullOrEmpty(row["SHOWSTYLE_ID"].ToString().Trim())
				? 0 : int.Parse(row["SHOWSTYLE_ID"].ToString().Trim());

			pLEDStyleStruct.FontAreaIx = string.IsNullOrEmpty(row["FONTAREA_IX"].ToString().Trim())
				? 0 : int.Parse(row["FONTAREA_IX"].ToString().Trim());

			pLEDStyleStruct.FontAreaIy = string.IsNullOrEmpty(row["FONTAREA_IY"].ToString().Trim())
				? 0 : int.Parse(row["FONTAREA_IY"].ToString().Trim());

			pLEDStyleStruct.High = string.IsNullOrEmpty(row["FONTAREA_HIGH"].ToString().Trim())
				? 0 : int.Parse(row["FONTAREA_HIGH"].ToString().Trim());

			pLEDStyleStruct.Wide = string.IsNullOrEmpty(row["FONTAREA_WIDE"].ToString().Trim())
				? 0 : int.Parse(row["FONTAREA_WIDE"].ToString().Trim());

			pLEDStyleStruct.FrameMode = string.IsNullOrEmpty(row["FONTAREA_FRAMEMODE"].ToString().Trim())
				? 0 : int.Parse(row["FONTAREA_FRAMEMODE"].ToString().Trim());

			pLEDStyleStruct.FrameColor = string.IsNullOrEmpty(row["FONTAREA_FRAMECOLOR"].ToString().Trim())
				? 0 : int.Parse(row["FONTAREA_FRAMECOLOR"].ToString().Trim());

			pLEDStyleStruct.FontName = string.IsNullOrEmpty(row["FONTSTYLE_FONTNAME"].ToString().Trim())
				? "" : row["FONTSTYLE_FONTNAME"].ToString().Trim();

			pLEDStyleStruct.Size = string.IsNullOrEmpty(row["FONTSTYLE_SIZE"].ToString().Trim())
				? 0 : int.Parse(row["FONTSTYLE_SIZE"].ToString().Trim());

			pLEDStyleStruct.IsBold = row["FONTSTYLE_ISBOLD"].ToString() == "1" ? true : false;

			pLEDStyleStruct.IsItaic = row["FONTSTYLE_ISITAIC"].ToString() == "1" ? true : false;

			pLEDStyleStruct.IsUnderLine = row["FONTSTYLE_ISUNDERLINE"].ToString() == "1" ? true : false;

			pLEDStyleStruct.FontColor = string.IsNullOrEmpty(row["FONTSTYLE_FONTCOLOR"].ToString().Trim())
				? "" : row["FONTSTYLE_FONTCOLOR"].ToString().Trim();

			pLEDStyleStruct.RowSpace = string.IsNullOrEmpty(row["FONTSTYLE_ROWSPACE"].ToString().Trim())
				? 0 : int.Parse(row["FONTSTYLE_ROWSPACE"].ToString().Trim());

			pLEDStyleStruct.AlignStyle = string.IsNullOrEmpty(row["FONTSTYLE_ALIGNSTYLE"].ToString().Trim())
				? 0 : int.Parse(row["FONTSTYLE_ALIGNSTYLE"].ToString().Trim());

			pLEDStyleStruct.ValignerStyle = string.IsNullOrEmpty(row["FONTSTYLE_VALIGNERSTYLE"].ToString().Trim())
				? 0 : int.Parse(row["FONTSTYLE_VALIGNERSTYLE"].ToString().Trim());

			return pLEDStyleStruct;
		}
	}
}
