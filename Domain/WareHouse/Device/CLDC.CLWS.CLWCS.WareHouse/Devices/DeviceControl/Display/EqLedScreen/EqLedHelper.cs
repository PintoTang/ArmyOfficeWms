using System;
using System.Runtime.InteropServices;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.EqLedScreen
{
    /// <summary>
    /// Eq的Led设备Ini文件创建
    /// </summary>
    public class EqLedHelper
    {
        /// <summary>
        /// 写ini 文件
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        /// <summary>
        /// 写配置文件区部分下的节点值
        /// </summary>
        /// <param name="Section">区名称</param>
        /// <param name="Key">节点关键字</param>
        /// <param name="strValue">保存的值</param>
        /// <returns></returns>
        private static long WriteFileName(string Section, string Key, string strValue)
        {
            return WritePrivateProfileString(Section, Key, strValue, strPath);
        }

        static string strPath = System.Environment.CurrentDirectory + "\\Resources\\EQ2008_Dll_Set.ini";

        /// <summary>
        /// 复制ini输出文件夹到根目录
        /// </summary>
        public static void CopyFileToRoute()
        {
            string strAppPath = System.Environment.CurrentDirectory;
            if (System.IO.File.Exists(strAppPath + "\\Resources\\EQ2008_Dll.dll") &&
                System.IO.File.Exists(strAppPath + "\\Resources\\EQ2008_Dll_Set.ini"))
            {
                System.IO.File.Copy(strAppPath + "\\Resources\\EQ2008_Dll.dll", strAppPath + "\\EQ2008_Dll.dll", true);
                System.IO.File.Copy(strAppPath + "\\Resources\\EQ2008_Dll_Set.ini", strAppPath + "\\EQ2008_Dll_Set.ini", true);
            }
        }

        public static OperateResult CreateIniFile(LedDevice ledDevice)
        {
            int cardNo = ledDevice.CardNum - 1;
            string section = "地址：" + cardNo;
            string[] ipAdress = ledDevice.Ip.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            WriteFileName(section, "CardType", ledDevice.CardType.ToString());
            WriteFileName(section, "CardAddress", cardNo.ToString());
            WriteFileName(section, "CommunicationMode", ledDevice.CommunicationMode.ToString());
            WriteFileName(section, "ScreemHeight", ledDevice.ScreemHeight.ToString());
            WriteFileName(section, "ScreemWidth", ledDevice.ScreemWidth.ToString());
            WriteFileName(section, "SerialBaud", ledDevice.SerialBaud.ToString());
            WriteFileName(section, "SerialNum", ledDevice.SerialNum.ToString());
            WriteFileName(section, "NetPort", ledDevice.NetPort.ToString());
            if (ipAdress.Length == 4)
            {
                WriteFileName(section, "IpAddress0", ipAdress[0]);
                WriteFileName(section, "IpAddress1", ipAdress[1]);
                WriteFileName(section, "IpAddress2", ipAdress[2]);
                WriteFileName(section, "IpAddress3", ipAdress[3]);
            }
            WriteFileName(section, "ColorStyle", ledDevice.ColorStyle.ToString());
            return OperateResult.CreateSuccessResult();
        }

        public static User_FontSet GetFontSet(Style style)
        {
            User_FontSet fontStyle = new User_FontSet
            {
                bFontBold = style.FontStyle.IsBold,
                bFontItaic = style.FontStyle.IsItaic,
                bFontUnderline = style.FontStyle.IsUnderline,
                iAlignStyle = style.FontStyle.AlignStyle,
                iFontSize = style.FontStyle.Size,
                iRowSpace = style.FontStyle.RowSpace,
                iVAlignerStyle = style.FontStyle.VAlignerStyle,
                strFontName = style.FontStyle.FontName
            };
            if (style.FontStyle.FontColor.Trim().Contains("0x"))
            {
                string swapString = style.FontStyle.FontColor.Trim().Replace("0x", "");
                fontStyle.colorFont = int.Parse(swapString, System.Globalization.NumberStyles.HexNumber);
            }
            return fontStyle;
        }

        public static User_PartInfo GetPartInfo(Style style)
        {
            User_PartInfo partInfo = new User_PartInfo
            {
                FrameColor = style.FontArea.FrameColor,
                iFrameMode = style.FontArea.FrameMode,
                iHeight = style.FontArea.High,
                iWidth = style.FontArea.Width,
                iX = style.FontArea.Ix,
                iY = style.FontArea.Iy
            };
            return partInfo;
        }

        public static User_MoveSet GetMoveSet(Style style)
        {
            User_MoveSet moveSet = new User_MoveSet
            {
                bClear = style.MoveSet.IsBackgroundClear,
                iActionSpeed = style.MoveSet.ActionSpeed,
                iActionType = style.MoveSet.ActionType,
                iClearActionType = style.MoveSet.ClearActionType,
                iClearSpeed = style.MoveSet.ClearSpeed,
                iFrameTime = style.MoveSet.FrameTime,
                iHoldTime = style.MoveSet.HoldTime
            };
            return moveSet;
        }

    }
}
