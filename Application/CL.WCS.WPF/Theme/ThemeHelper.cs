using System.Windows.Controls;
using System.Windows.Media;
using CL.WCS.SystemConfigPckg.Model;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace CL.WCS.WPF.Theme
{
    public class ThemeHelper
    {
        private PaletteHelper _paletteHelper=new PaletteHelper();

        private void SetPrimaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, color);
            theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, color);
            theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }

        private void SetSecondaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, color);
            theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, color);
            theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }

        /// <summary>
        /// 设置系统整体风格
        /// </summary>
        public void SetGlobalSystemStyle(DepartmentEnum department)
        {
            if (department.Equals(DepartmentEnum.JZX))
            {
                Color primaryColor = Color.FromRgb(36, 93, 85);
                Color PrimaryForegroundColor = Color.FromRgb(255, 255, 255);
                Color SecondaryColor = Color.FromRgb(51, 136, 233);
                Color SecondaryForegroundColor = Color.FromRgb(255, 255, 255);
                PaletteHelperExtensions.ChangePrimaryColor(_paletteHelper, primaryColor);
                PaletteHelperExtensions.ChangeSecondaryColor(_paletteHelper, SecondaryColor);
                SetPrimaryForegroundToSingleColor(PrimaryForegroundColor);
                SetSecondaryForegroundToSingleColor(SecondaryForegroundColor);
                return;
            }
            //if (department.Equals(DepartmentEnum.SCG))
            //{
            //    Color primaryColor = Color.FromRgb(0, 54, 122);
            //    Color PrimaryForegroundColor = Color.FromRgb(130, 218, 255);
            //    Color SecondaryColor = Color.FromRgb(51, 136, 233);
            //    Color SecondaryForegroundColor = Color.FromRgb(255, 255, 255);
            //    PaletteHelperExtensions.ChangePrimaryColor(_paletteHelper, primaryColor);
            //    PaletteHelperExtensions.ChangeSecondaryColor(_paletteHelper, SecondaryColor);
            //    SetPrimaryForegroundToSingleColor(PrimaryForegroundColor);
            //    SetSecondaryForegroundToSingleColor(SecondaryForegroundColor);
            //    return;
            //}
            //if (department.Equals(DepartmentEnum.StateGrid))
            //{
            //    Color primaryColor = Color.FromRgb(0, 54, 122);
            //    Color PrimaryForegroundColor = Color.FromRgb(130, 218, 255);
            //    Color SecondaryColor = Color.FromRgb(51, 136, 233);
            //    Color SecondaryForegroundColor = Color.FromRgb(255, 255, 255);
            //    PaletteHelperExtensions.ChangePrimaryColor(_paletteHelper, primaryColor);
            //    PaletteHelperExtensions.ChangeSecondaryColor(_paletteHelper, SecondaryColor);
            //    SetPrimaryForegroundToSingleColor(PrimaryForegroundColor);
            //    SetSecondaryForegroundToSingleColor(SecondaryForegroundColor);
            //    return;
            //}

        }
    }
}
