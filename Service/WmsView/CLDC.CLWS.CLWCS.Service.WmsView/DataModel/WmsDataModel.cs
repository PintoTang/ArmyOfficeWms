﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
    public sealed class WmsDataModel
    {
        private Order _Order=new Order();        
        private Material _material=new Material();
        private Area _area =new Area();
        private Shelf _shelf=new Shelf();
        private Warehouse _warehouse=new Warehouse();

        public Order Order
        {
            get { return _Order; }
            set { _Order = value; }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public Area Area
        {
            get { return _area; }
            set { _area = value; }
        }
        public Shelf Shelf
        {
            get { return _shelf; }
            set { _shelf = value; }
        }
        public Warehouse Warehouse
        {
            get { return _warehouse; }
            set { _warehouse = value; }
        }
    }
}
