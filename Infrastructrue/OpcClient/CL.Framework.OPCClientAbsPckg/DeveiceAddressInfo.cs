
namespace CL.Framework.OPCClientAbsPckg
{
    /// <summary>
    /// 设备地址集合
    /// </summary>
    public class DeviceAddressInfo
    {
        public DeviceAddressInfo()
        {
            this.deviceName = string.Empty;
            this.Datablock = new Datablock();
        }

        public string deviceName { get; set; }
        public Datablock Datablock { get; set; }

        public override bool Equals(object obj)
        {
            bool result = false;
            DeviceAddressInfo other = obj as DeviceAddressInfo;

            result = object.ReferenceEquals(this, other)
                || (this.deviceName.Equals(other.deviceName)
                && this.Datablock.RealDataBlockAddr.Equals(other.Datablock.RealDataBlockAddr));

            return result;
        }

        public override int GetHashCode()
        {
            return this.Datablock.RealDataBlockAddr.GetHashCode() | this.deviceName.GetHashCode();
        }
    }
}
