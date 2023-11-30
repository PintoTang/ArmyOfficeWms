using System.Collections.Generic;
using System.Xml.Serialization;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.Display.EqLedScreen
{
    [XmlRoot("EQSet", Namespace = "", IsNullable = false)]
    public class EqLedConfig
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("LedDevice")]
        public List<LedDevice> LedDevice { get; set; }

    }


    public class LedDevice
    {
        [XmlAttribute("DeviceId")]
        public int DeviceId { get; set; }
        [XmlAttribute("IP")]
        public string Ip { get; set; }
        [XmlAttribute("NetPort")]
        public int NetPort { get; set; }
        [XmlAttribute("CardNum")]
        public int CardNum { get; set; }
        [XmlAttribute("CardType")]
        public int CardType { get; set; }
        [XmlAttribute("CardAddress")]
        public int CardAddress { get; set; }
        [XmlAttribute("CommunicationMode")]
        public int CommunicationMode { get; set; }
        [XmlAttribute("ScreemHeight")]
        public int ScreemHeight { get; set; }
        [XmlAttribute("ScreemWidth")]
        public int ScreemWidth { get; set; }
        [XmlAttribute("SerialBaud")]
        public int SerialBaud { get; set; }
        [XmlAttribute("SerialNum")]
        public int SerialNum { get; set; }
        [XmlAttribute("ColorStyle")]
        public int ColorStyle { get; set; }

        [XmlElement("Style")]
        public List<Style> Styles { get; set; }


    }

    public class Style
    {
        [XmlAttribute("Id")]
        public int Id { get; set; }
        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlElement("FontArea")]
        public FontArea FontArea { get; set; }

        [XmlElement("FontStyle")]
        public FontStyle FontStyle { get; set; }

        [XmlElement("MoveSet")]
        public MoveSet MoveSet { get; set; }

    }

    public class FontArea
    {
        [XmlAttribute("Ix")]
        public int Ix { get; set; }
        [XmlAttribute("Iy")]
        public int Iy { get; set; }
        [XmlAttribute("High")]
        public int High { get; set; }
        [XmlAttribute("Width")]
        public int Width { get; set; }
        [XmlAttribute("FrameMode")]
        public int FrameMode { get; set; }
        [XmlAttribute("FrameColor")]
        public int FrameColor { get; set; }

    }

    public class FontStyle
    {
        [XmlAttribute("FontName")]
        public string FontName { get; set; }
        [XmlAttribute("Size")]
        public int Size { get; set; }
        [XmlAttribute("IsBold")]
        public bool IsBold { get; set; }
        [XmlAttribute("IsItaic")]
        public bool IsItaic { get; set; }
        [XmlAttribute("IsUnderline")]
        public bool IsUnderline { get; set; }
        [XmlAttribute("FontColor")]
        public string FontColor { get; set; }
        [XmlAttribute("RowSpace")]
        public int RowSpace { get; set; }
        [XmlAttribute("AlignStyle")]
        public int AlignStyle { get; set; }
        [XmlAttribute("VAlignerStyle")]
        public int VAlignerStyle { get; set; }



    }

    public class MoveSet
    {
        [XmlAttribute("ActionType")]
        public int ActionType { get; set; }
        [XmlAttribute("ActionSpeed")]
        public int ActionSpeed { get; set; }
        [XmlAttribute("IsBackgroundClear")]
        public bool IsBackgroundClear { get; set; }
        [XmlAttribute("HoldTime")]
        public int HoldTime { get; set; }
        [XmlAttribute("ClearSpeed")]
        public int ClearSpeed { get; set; }
        [XmlAttribute("ClearActionType")]
        public int ClearActionType { get; set; }
        [XmlAttribute("FrameTime")]
        public int FrameTime { get; set; }

    }

}
