using SqlSugar;
using System;

namespace CLDC.CLWS.CLWCS.Service.WmsView.Model
{
    /// <summary>
    /// 声光配置
    /// </summary>
    [SugarTable("t_SoundLight")]
    public class SoundLight
    {
        /// <summary>
        /// 声光配置Id
        /// </summary>
        [SugarColumn]
        public long Id { get; set; }

        /// <summary>
        /// 任务分类
        /// </summary>
        [SugarColumn]
        public string Area { get; set; }

        /// <summary>
        /// 任务分队
        /// </summary>
        [SugarColumn]
        public string Team { get; set; }

        /// <summary>
        /// 光道
        /// </summary>
        [SugarColumn]
        public int Light { get; set; }

        /// <summary>
        /// 声道
        /// </summary>
        [SugarColumn]
        public int Sound { get; set; }

        /// <summary>
        /// 播报内容
        /// </summary>
        [SugarColumn]
        public string SoundContent { get; set; }

        /// <summary>
        /// 灯光指令
        /// </summary>
        [SugarColumn]
        public string LightCode { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn]
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn]
        public DateTime? CreatedTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [SugarColumn]
        public bool? IsDeleted { get; set; }
    }
}

