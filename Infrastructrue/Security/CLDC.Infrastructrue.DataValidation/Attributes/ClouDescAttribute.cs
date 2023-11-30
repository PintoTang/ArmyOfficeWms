using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLDC.Infrastructrue.DataValidation
{
    /// <summary>
    /// 针对类的一个功能说明及描述
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ClouDescAttribute : Attribute
    {
        private List<string> _useProjectName=new List<string>();

        public ClouDescAttribute(string author,string overview,params string[] useProjects)
        {
            Author = author;
            Overview = overview;
            if (useProjects!=null)
            {
                _useProjectName.AddRange(useProjects);
            }
        }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 概要描述
        /// </summary>
        public string Overview { get; set; }

        public List<string> UseProjectName
        {
            get { return _useProjectName; }
            set { _useProjectName = value; }
        }
    }
}
