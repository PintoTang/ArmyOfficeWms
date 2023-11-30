namespace CLWCS.WareHouse.WebserviceForHeFei.CmdMode
{
   public class  SendUnstackCmd
   {
       public int CMD_NO { get; set; }

       public UnstackData DATA { get; set; }
   }

    public class UnstackData
    {
        public int ID { get; set; }
        public string PALLETIZER_NAME { get; set; }
        public int CONTAINER_TYPE { get; set; }
        public string ADDR { get; set; }
        public int QTY { get; set; }
    }
}
