namespace CLDC.Infrastructrue.UserCtrl.Domain
{
    /// <summary>
    /// 主菜单显示界面接口
    /// </summary>
    public interface IMainUseCtrl
    {
        /// <summary>
        /// 控件Id标识
        /// </summary>
        string UseCtrlId { get; set; }
        /// <summary>
        /// 显示
        /// </summary>
        void Show();
        /// <summary>
        /// 隐藏
        /// </summary>
        void Hide();
    }
}
