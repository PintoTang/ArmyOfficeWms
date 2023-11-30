using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL.WCS.SystemConfigPckg.Model;

namespace CL.WCS.WPF.View.MainTitle
{
    public class MainTitleViewModel
    {
        public string IconPath
        {
            get
            {
                DepartmentEnum curDepartment = SystemConfig.Instance.Department;
                string pathImage = string.Format("\\Resources\\{0}\\Image\\Logo.png", curDepartment);
                string path = System.Environment.CurrentDirectory + pathImage;
                return path;
            }
        }
    }
}
