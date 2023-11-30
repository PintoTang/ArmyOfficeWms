using System;
using System.Collections.Generic;
using System.Linq;
using CL.Framework.OPCClientAbsPckg;
using CL.WCS.DataModelPckg;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;

namespace CLDC.CLWS.CLWCS.WareHouse.Device.DataModel
{
    /// <summary>
    /// Opc协议的操作元素
    /// </summary>
    public sealed class OpcElement
    {
        /// <summary>
        /// 通讯地址集合
        /// </summary>
        List<Datablock> datablocks = new List<Datablock>();
        public List<Datablock> Datablocks
        {
            get { return datablocks; }
        }



        /// <summary>
        /// 移除设备通讯地址集合
        /// </summary>
        /// <param name="datablock"></param>
        /// <returns></returns>
        public OperateResult RemoveDatablock(Datablock datablock)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (datablock != null)
            {
                try
                {
                    lock (datablocks)
                    {
                        if (datablocks.Exists(d => d.DataBlockName.Equals(datablock.DataBlockName)))
                        {
                            Datablock db = datablocks.Find(d => d.DataBlockName.Equals(datablock.DataBlockName));
                            result.IsSuccess = datablocks.Remove(db);
                        }
                        else
                        {
                            result.IsSuccess = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            else

            {
                result.IsSuccess = false;
                result.Message = "传入参数datablock为空";
            }
            return result;
        }

        /// <summary>
        /// 添加设备通讯地址集合
        /// </summary>
        /// <param name="datablock"></param>
        /// <returns></returns>
        public OperateResult AddDatablock(Datablock datablock)
        {
            OperateResult result = OperateResult.CreateFailedResult();
            if (datablock != null)
            {
                try
                {
                    lock (datablocks)
                    {
                        if (datablocks.Exists(d => d.DataBlockName.Equals(datablock.DataBlockName)))
                        {
                            Datablock db = datablocks.First(d => d.DataBlockName.Equals(datablock.DataBlockName));
                            db.RealDataBlockAddr = datablock.RealDataBlockAddr;
                            db.DataType = datablock.DataType;
                            db.DatablockEnum = datablock.DatablockEnum;
                            db.Name = datablock.Name;
                        }
                        else
                        {
                            datablocks.Add(datablock);
                        }
                        result.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = OperateResult.ConvertException(ex);
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "出入参数datablock为空";
            }
            return result;
        }

        public OperateResult<Datablock> CheckDatablockName(string datablockName)
        {
            OperateResult<Datablock> result = new OperateResult<Datablock>();
            try
            {
                DataBlockNameEnum datablockEnum = (DataBlockNameEnum)Enum.Parse(typeof(DataBlockNameEnum), datablockName);
                result = CheckDatablockEnum(datablockEnum);
            }
            catch (Exception ex)
            {
                result.Message = OperateResult.ConvertException(ex);
                result.IsSuccess = false;
            }
            return result;
        }

        public OperateResult<Datablock> CheckDatablockEnum(DataBlockNameEnum nameEnum)
        {
            OperateResult<Datablock> result = new OperateResult<Datablock>();
            lock (datablocks)
            {
                if (datablocks == null)
                {
                    result.IsSuccess = false;
                    return result;
                }
                if (!datablocks.Exists(d => d.DatablockEnum.Equals(nameEnum)))
                {
                    result.IsSuccess = false;
                    return result;
                }
                Datablock db = datablocks.First(d => d.DatablockEnum.Equals(nameEnum));
                if (string.IsNullOrEmpty(db.RealDataBlockAddr))
                {
                    result.IsSuccess = false;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = db;
                return result;
            }
        }
    }
}
