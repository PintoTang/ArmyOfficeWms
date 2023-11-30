using Newtonsoft.Json;

namespace CLWCS.UpperServiceForHeFei.Interface.InterfaceDataMode
{
    public class CmdReturnMessageHeFei
    {
        private ReturnData _data = new ReturnData();

        public ReturnData DATA
        {
            get { return _data; }
            set { _data = value; }
        }
        public CmdReturnMessageHeFei(int result, string msg,ReturnData data)
        {
            this.RESULT = result;
            this.MESSAGE = msg;
        }
        /// <summary>
        /// 结果
        /// </summary>
        public int RESULT { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string MESSAGE { get; set; }

        public static explicit operator CmdReturnMessageHeFei(string json)
        {
            return JsonConvert.DeserializeObject<CmdReturnMessageHeFei>(json);
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess
        {
            get { return RESULT == 1; }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ReturnData
    {

    }
}
