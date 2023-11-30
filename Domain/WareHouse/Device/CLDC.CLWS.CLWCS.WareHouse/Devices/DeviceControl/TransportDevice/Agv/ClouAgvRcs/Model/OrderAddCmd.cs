using System;
using System.Collections.Generic;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.Devices.DeviceControl.TransportDevice.Agv.ClouAgvRcs.Model
{
    public class OrderAddCmd
    {
        private List<OrderDetail> _details = new List<OrderDetail>();
        public string orderSource { get; set; }
        public string orderCode { get; set; }
        public DateTime planBeginTime { get; set; }
        public DateTime planEndTime { get; set; }
        public int priority { get; set; }

        public List<OrderDetail> details
        {
            get { return _details; }
            set { _details = value; }
        }
    }

    public class OrderDetail
    {
        public int orderType { get; set; }
        public string productCode { get; set; }
        public string productName { get; set; }
        public int qty { get; set; }
        public string beginStationCode { get; set; }
        public string endStationCode { get; set; }
        public string actionExtra { get; set; }
    }

}
