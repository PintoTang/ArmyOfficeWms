using System;
using System.Reflection;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.Infrastructrue.Xml;

namespace CLDC.Service.Project
{
    public class ProjectStart
    {
        private ProjectConfig _projectConfig;
        readonly string _projectConfigPath = System.Environment.CurrentDirectory + "\\Config\\ProjectConfig.xml";
        public OperateResult StartProject()
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                OperateResult initilizeConfigResult = InitilizeConfig(_projectConfigPath);
                if (!initilizeConfigResult.IsSuccess)
                {
                    return initilizeConfigResult;
                }
                ProjectAbstract project = (ProjectAbstract)Assembly.Load(_projectConfig.Project.NameSpace).CreateInstance(_projectConfig.Project.NameSpace + "." + _projectConfig.Project.ClassName);
                if (project == null)
                {
                    result.IsSuccess = false;
                    result.Message = string.Format("初始化项目失败，项目命名空间：{0} 项目类名：{1}", _projectConfig.Project.NameSpace,
                        _projectConfig.Project.ClassName);
                    return result;
                }
                OperateResult initilizeProjectResult = project.InitilizeProject();
                return initilizeProjectResult;
            }
            catch (Exception ex)
            {

                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        private OperateResult InitilizeConfig(string path)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            try
            {
                _projectConfig = (ProjectConfig)XmlSerializerHelper.LoadFromXml(path, typeof(ProjectConfig));
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

    }
}
