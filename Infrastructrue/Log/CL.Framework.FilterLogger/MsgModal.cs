using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Framework.FilterLogger
{
    class MsgModal
    {
        public DateTime HappenTime { get; set; }

        public string Msg { get; set; }

        public long Id { get; set; }

        public int RepeatTime { get; set; }

        public TimeSpan DuarationTime { get; set; }

        public override string ToString()
        {
            return Msg;
        }

        public MsgModal(DateTime happenTime, string msg, long id, int repeatTime)
        {
            HappenTime = happenTime;
            Msg = msg;
            Id = id;
            RepeatTime = repeatTime;
        }
    }
}
