using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using CLDC.CLWS.CLWCS.Framework;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness;
using Infrastructrue.Ioc.DependencyFactory;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.Common;
using CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.EqLedScreen;
using CLDC.Infrastructrue.Xml;

namespace CLDC.CLWS.CLWCS.WareHouse.Device
{
    public class EqLedScreenDeviceControl : DisplayDeviceControlAbstract
    {

        //==========================1、节目操作函数==================================================//
        //添加节目
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddProgram(int CardNum, Boolean bWaitToEnd, int iPlayTime);
        //删除所有节目
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_DelAllProgram(int CardNum);
        //删除单个节目
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_DelProgram(int CardNum, int iProgramIndex);


        //添加单行文本区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddSingleText(int CardNum, ref User_SingleText pSingleText, int iProgramIndex);
        //添加文本区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddText(int CardNum, ref User_Text pText, int iProgramIndex);
        //添加时间区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddTime(int CardNum, ref User_DateTime pdateTime, int iProgramIndex);
        //添加图文区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddBmpZone(int CardNum, ref User_Bmp pBmp, int iProgramIndex);
        //指定图像句柄添加图片
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern bool User_AddBmp(int CardNum, int iBmpPartNum, IntPtr hBitmap, ref User_MoveSet pMoveSet, int iProgramIndex);
        //指定图像路径添加图片
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern bool User_AddBmpFile(int CardNum, int iBmpPartNum, string strFileName, ref User_MoveSet pMoveSet, int iProgramIndex);

        //添加RTF区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddRTF(int CardNum, ref User_RTF pRTF, int iProgramIndex);
        //添加计时区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddTimeCount(int CardNum, ref User_Timer pTimeCount, int iProgramIndex);
        //添加温度区
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern int User_AddTemperature(int CardNum, ref User_Temperature pTemperature, int iProgramIndex);

        //发送数据
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_SendToScreen(int CardNum);
        //====================================================================//       

        //=======================2、实时发送数据（高频率发送）=================//
        //实时建立连接
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_RealtimeConnect(int CardNum);
        //实时发送图片数据
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_RealtimeSendData(int CardNum, int x, int y, int iWidth, int iHeight, IntPtr hBitmap);
        //实时发送图片文件
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_RealtimeSendBmpData(int CardNum, int x, int y, int iWidth, int iHeight, string strFileName);
        //实时发送文本
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_RealtimeSendText(int CardNum, int x, int y, int iWidth, int iHeight, string strText, ref User_FontSet pFontInfo);
        //实时关闭连接
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_RealtimeDisConnect(int CardNum);
        //实时发送清屏
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_RealtimeScreenClear(int CardNum);
        //====================================================================//

        //==========================3、显示屏控制函数组=======================//
        //校正时间
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_AdjustTime(int CardNum);
        //开屏
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_OpenScreen(int CardNum);
        //关屏
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_CloseScreen(int CardNum);
        //亮度调节
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern Boolean User_SetScreenLight(int CardNum, int iLightDegreen);
        //Reload参数文件
        [DllImport("EQ2008_Dll.dll", CharSet = CharSet.Ansi)]
        public static extern void User_ReloadIniFile(string strEQ2008_Dll_Set_Path);
        //====================================================================//

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);


        private LedDevice ledDevice;



        private OperateResult CopyFileToRoute()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                string strAppPath = System.Environment.CurrentDirectory;//+ "\\Resources\\"
                if (System.IO.File.Exists(strAppPath + "\\Resources\\EQ2008_Dll.dll") &&
                    System.IO.File.Exists(strAppPath + "\\Resources\\EQ2008_Dll_Set.ini"))
                {
                    System.IO.File.Copy(strAppPath + "\\Resources\\EQ2008_Dll.dll", strAppPath + "\\EQ2008_Dll.dll", true);
                    System.IO.File.Copy(strAppPath + "\\Resources\\EQ2008_Dll_Set.ini", strAppPath + "\\EQ2008_Dll_Set.ini", true);
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private OperateResult<EqLedStyle> InitScreenStyle()
        {
            EqLedStyle style = new EqLedStyle();
            OperateResult<EqLedStyle> result = OperateResult.CreateFailedResult(style, "无数据");
            try
            {
                string strFileName = System.Environment.CurrentDirectory + "\\Config\\LEDConfig.xml";

                EqLedConfig eqLedConfig = (EqLedConfig)XmlSerializerHelper.LoadFromXml(strFileName, typeof(EqLedConfig));

                if (eqLedConfig == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("加载文件：{0} 失败", strFileName);
                    return result;
                }

                ledDevice = eqLedConfig.LedDevice.FirstOrDefault(d => d.DeviceId.Equals(DeviceId));
                if (ledDevice == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("查找不到Eq设备编号：{0} 的样式配置信息,请检查配置文件：{1}", DeviceId, strFileName);
                    return result;
                }

                OperateResult createResult = EqLedHelper.CreateIniFile(ledDevice);
                if (!createResult.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = createResult.Message;
                    return result;
                }
                EqLedHelper.CopyFileToRoute();
                result.IsSuccess = true;
                result.Message = "初始化成功";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
 
            }
            return result;
        }


        public  override OperateResult ParticularInitConfig()
        {
            OperateResult<EqLedStyle> initStyle = InitScreenStyle();
            if (!initStyle.IsSuccess)
            {
                return OperateResult.CreateFailedResult(initStyle.Message, 1);
            }
            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult ParticularInitlize()
        {


            OperateResult copyFile = CopyFileToRoute();
            if (!copyFile.IsSuccess)
            {
                return copyFile;
            }


            return OperateResult.CreateSuccessResult();
        }

        public override OperateResult Start()
        {
            return OperateResult.CreateSuccessResult();
        }

        public override UserControl GetPropertyView()
        {
            return null;
        }

        private readonly object sendLock = new object();

        internal override OperateResult SendTitle(string title)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            Style titleStyle = ledDevice.Styles.FirstOrDefault(s => s.Type.Equals("Title"));

            if (titleStyle == null)
            {
                result.IsSuccess = false;
                result.Message = "找不到对应的标题样式";
                return result;
            }
            User_FontSet fontStyle = EqLedHelper.GetFontSet(titleStyle);
            lock (sendLock)
            {
                try
                {
                    bool isSussece = User_RealtimeSendText(ledDevice.CardNum,
                        titleStyle.FontArea.Ix,
                        titleStyle.FontArea.Iy,
                       titleStyle.FontArea.Width,
                        titleStyle.FontArea.High,
                        title,
                        ref fontStyle);
                    if (isSussece)
                    {
                        result.Message = string.Format("发送标题：{0} 成功", title);
                    }
                    else
                    {
                        result.Message = string.Format("发送标题：{0} 失败", title);
                    }
                    result.IsSuccess = isSussece;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
                return result;
            }
        }




        /// <summary>
        /// 发送内容 和标题一起发送 只需要发送一次即可
        /// </summary>
        /// <param name="content">要发送的内容</param>
        /// <returns></returns>
        internal override OperateResult SendContent(string content)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            string titleText = TitleContent;
            if (string.IsNullOrEmpty(content))
            {
                result.Message = "发送的内容为空";
                result.IsSuccess = false;
                return result;
            }

            Style contentStyle = ledDevice.Styles.FirstOrDefault(s => s.Type.Equals("Content"));
            if (contentStyle == null)
            {
                result.IsSuccess = false;
                result.Message = "找不到对应的内容样式";
                return result;
            }

            Style titleStyle = ledDevice.Styles.FirstOrDefault(s => s.Type.Equals("Title"));
            if (titleStyle == null)
            {
                result.IsSuccess = false;
                result.Message = "找不到对应的标题样式";
                return result;
            }
            lock (sendLock)
            {
                try
                {
                    User_DelAllProgram(ledDevice.CardNum);//删除CardNum所有的节目
                    int g_iProgramIndex = User_AddProgram(ledDevice.CardNum, false, 20);//添加节目 返回g_iProgramIndex 节目索引

                    User_SingleText pTitle_SingleText = new User_SingleText
                    {
                        PartInfo = EqLedHelper.GetPartInfo(titleStyle),
                        FontInfo = EqLedHelper.GetFontSet(titleStyle),
                        MoveSet = EqLedHelper.GetMoveSet(titleStyle),
                        BkColor = titleStyle.FontArea.FrameColor,
                        chContent = titleText //传递标题内容
                    };

                    bool isJson = ValidJson.IsJson(content);
                    if (!isJson) //表示普通信息则用原来的方式处理
                    {
                        pTitle_SingleText.PartInfo.iX = titleStyle.FontArea.Ix;
                        pTitle_SingleText.PartInfo.iY = titleStyle.FontArea.Iy;
                        pTitle_SingleText.PartInfo.iWidth = titleStyle.FontArea.Width;
                        pTitle_SingleText.PartInfo.iHeight = titleStyle.FontArea.High;
                        User_FontSet fontStyle = EqLedHelper.GetFontSet(titleStyle);

                        pTitle_SingleText.MoveSet.iActionType = 1;//立即显示
                        pTitle_SingleText.FontInfo = fontStyle;

                        //向节目中 添加单行标题
                        int pAddSingIndex = User_AddSingleText(ledDevice.CardNum, ref pTitle_SingleText, g_iProgramIndex);
                        if (pAddSingIndex == -1)
                        {
                            result.IsSuccess = false;
                            result.Message = "添加单行标题失败";
                            return result;
                        }

                        //添加 内容
                        User_Text pContentText = new User_Text
                        {
                            PartInfo = EqLedHelper.GetPartInfo(contentStyle),
                            MoveSet = EqLedHelper.GetMoveSet(contentStyle),
                            FontInfo = EqLedHelper.GetFontSet(contentStyle),
                            chContent = content
                        };
                        //向节目中 添加文本区 内容 
                        User_AddText(ledDevice.CardNum, ref pContentText, g_iProgramIndex);
                    }
                    else
                    {
                        //json wcs  用新样式处理  64*256 最多4行
                        //向节目中 添加单行标题
                        pTitle_SingleText.PartInfo.iHeight = 16;//16*4=64 最好是高度16
                        pTitle_SingleText.PartInfo.iX = 0;
                        pTitle_SingleText.PartInfo.iY = 0;
                        pTitle_SingleText.FontInfo.iFontSize = 10;//建议 太大不容易显示
                        pTitle_SingleText.MoveSet.iActionType = 1;
                        int pAddSingIndex = User_AddSingleText(ledDevice.CardNum, ref pTitle_SingleText, g_iProgramIndex);
                        if (pAddSingIndex == -1)
                        {
                            result.IsSuccess = false;
                            result.Message = "添加单行标题失败";
                            return result;
                        }
                        //wms协商的协议格式 string
                        Newtonsoft.Json.Linq.JObject jonObj = Newtonsoft.Json.Linq.JObject.Parse(content);
                        if (jonObj["1"] != null && !string.IsNullOrEmpty(jonObj["1"].ToString()))
                        {
                            //添加 单行文本 内容
                            User_SingleText pContentSingleText = new User_SingleText
                            {
                                PartInfo = EqLedHelper.GetPartInfo(contentStyle),
                                MoveSet = EqLedHelper.GetMoveSet(contentStyle),
                                FontInfo = EqLedHelper.GetFontSet(contentStyle),
                                chContent = jonObj["1"].ToString()
                            };
                            pContentSingleText.FontInfo.iFontSize = 9;
                            pContentSingleText.PartInfo.iHeight = 16;
                            pContentSingleText.MoveSet.iActionType = 1;//立即显示
                            pContentSingleText.PartInfo.iY = 16 * 1;//Y的高度 不一样
                            //向节目中 添加文本区 内容 
                            User_AddSingleText(ledDevice.CardNum, ref pContentSingleText, g_iProgramIndex);
                        }
                        if (jonObj["2"] != null && !string.IsNullOrEmpty(jonObj["2"].ToString()))
                        {
                            //添加 单行文本 内容
                            User_SingleText pContentSingleText = new User_SingleText
                            {
                                PartInfo = EqLedHelper.GetPartInfo(contentStyle),
                                MoveSet = EqLedHelper.GetMoveSet(contentStyle),
                                FontInfo = EqLedHelper.GetFontSet(contentStyle),
                                chContent = jonObj["2"].ToString()
                            };
                            pContentSingleText.FontInfo.iFontSize = 9;
                            pContentSingleText.PartInfo.iHeight = 16;
                            pContentSingleText.MoveSet.iActionType = 1;//立即显示
                            pContentSingleText.PartInfo.iY = 16 * 2;//Y的高度 不一样
                            //向节目中 添加文本区 内容 
                            User_AddSingleText(ledDevice.CardNum, ref pContentSingleText, g_iProgramIndex);
                        }
                        if (jonObj["3"] != null && !string.IsNullOrEmpty(jonObj["3"].ToString()))
                        {
                            //添加 单行文本 内容
                            User_SingleText pContentSingleText = new User_SingleText
                            {
                                PartInfo = EqLedHelper.GetPartInfo(contentStyle),
                                MoveSet = EqLedHelper.GetMoveSet(contentStyle),
                                FontInfo = EqLedHelper.GetFontSet(contentStyle),
                                chContent = jonObj["3"].ToString()
                            };
                            pContentSingleText.FontInfo.iFontSize = 9;
                            pContentSingleText.PartInfo.iHeight = 16;
                         
                            pContentSingleText.PartInfo.iY = 16 * 3;//Y的高度 不一样
                            pContentSingleText.MoveSet.iActionType = 20;//火凤凰系列参数为2，蓝精灵参数20
                            pContentSingleText.MoveSet.iActionSpeed = 7;//火凤凰系列固定为1，蓝精灵参数1（最快）-20（最慢）
                            pContentSingleText.MoveSet.iActionType = 3;//往左滚动
                            //向节目中 添加文本区 内容 
                            User_AddSingleText(ledDevice.CardNum, ref pContentSingleText, g_iProgramIndex);
                        }
                        //pContentSingleText.MoveSet.iClearSpeed = 20;//未测试 暂时不用
                        //pContentSingleText.MoveSet.iHoldTime = 18;//未测试 暂时不用
                    }

                    bool isSussece = User_SendToScreen(ledDevice.CardNum);
                    if (!isSussece)
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format("LED控制卡【{0}】;发送消息：{1} 结果为：{2}", ledDevice.CardNum, content,
                            "失败");
                        return result;
                    }

                    if (User_RealtimeConnect(ledDevice.CardNum))
                    {
                        User_RealtimeDisConnect(ledDevice.CardNum);
                        result.Message = string.Format("Eq发送显示标题：{0} 显示内容：{1} 成功", titleText, content);
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "与Eq显示屏建立实时连接失败";
           
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    result.Message = OperateResult.ConvertException(ex);
                    result.IsSuccess = false;
                }
            }
            return result;
        }


        //internal override OperateResult SendContent(string content)
        //{
        //    OperateResult result = OperateResult.CreateFailedResult();
        //    if (string.IsNullOrEmpty(content))
        //    {
        //        result.Message = "发送的内容为空";
        //        result.IsSuccess = false;
        //        return result;
        //    }
        //    Style contentStyle = ledDevice.Styles.FirstOrDefault(s => s.Type.Equals("Content"));

        //    if (contentStyle == null)
        //    {
        //        result.IsSuccess = false;
        //        result.Message = "找不到对应的标题样式";
        //        return result;
        //    }
        //    User_FontSet fontStyle = EqLedHelper.GetFontSet(contentStyle);
        //    User_PartInfo partInfo = EqLedHelper.GetPartInfo(contentStyle);
        //    User_MoveSet moveSet = EqLedHelper.GetMoveSet(contentStyle);
        //    lock (sendLock)
        //    {
        //        try
        //        {
        //            User_Text pText = new User_Text();
        //            pText.PartInfo = partInfo;
        //            pText.MoveSet = moveSet;
        //            pText.FontInfo = fontStyle;
        //            pText.chContent = content;

        //            User_DelAllProgram(ledDevice.CardNum);

        //            int g_iProgramIndex = User_AddProgram(ledDevice.CardNum, false, 10);
        //            User_AddText(ledDevice.CardNum, ref pText, g_iProgramIndex);
        //            string titleText = TitleContent;
        //            bool isSussece = User_SendToScreen(ledDevice.CardNum);
        //            if (!isSussece)
        //            {
        //                result.IsSuccess = false;
        //                result.Message = string.Format("LED控制卡【{0}】;发送消息：{1} 结果为：{2}", ledDevice.CardNum, content,
        //                    "失败");
        //                return result;
        //            }

        //            if (User_RealtimeConnect(ledDevice.CardNum))
        //            {
        //                if (!string.IsNullOrEmpty(titleText))
        //                {
        //                    OperateResult sendTitle = SendTitle(titleText);
        //                    if (!sendTitle.IsSuccess)
        //                    {
        //                        result.Message = string.Format("LED控制卡【{0}】;显示标题：{1}，结果为：{2}", ledDevice.CardNum,
        //                            titleText, "失败");
        //                        result.IsSuccess = false;
        //                        return result;
        //                    }
        //                }
        //                User_RealtimeDisConnect(ledDevice.CardNum);
        //                result.Message = string.Format("Eq发送显示标题：{0} 显示内容：{1} 成功", titleText, content);
        //                result.IsSuccess = true;
        //            }
        //            else
        //            {
        //                result.Message = "与Eq显示屏建立实时连接失败";
        //                result.IsSuccess = false;
        //                return result;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Message = OperateResult.ConvertException(ex);
        //            result.IsSuccess = false;
        //        }
        //    }
        //    return result;
        //}

        internal override OperateResult ClearScreen()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                lock (sendLock)
                {
                    if (User_RealtimeConnect(ledDevice.CardNum))
                    {
                        bool clearResult = User_RealtimeScreenClear(ledDevice.CardNum);
                        User_RealtimeDisConnect(ledDevice.CardNum);
                        result.IsSuccess = clearResult;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "打开Eq的实时连接失败";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }
    }
}
