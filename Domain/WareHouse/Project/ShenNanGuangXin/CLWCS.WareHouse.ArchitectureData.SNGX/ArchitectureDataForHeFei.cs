using System;
using System.Collections.Generic;
using System.Linq;
using CLDC.CLWS.CLWCS.Infrastructrue.DataModel;
using CLDC.CLWCS.WareHouse.DataMapper;
using CLDC.CLWS.CLWCS.WareHouse.DataModel;
using CLDC.CLWS.CLWCS.WareHouse.DbBusiness.Common;

namespace CLWCS.WareHouse.ArchitectureData.HeFei
{
    public class ArchitectureDataForHeFei : IWmsWcsArchitecture
    {
        public ArchitectureDataForHeFei(WhAddressDataAbstract dataContext)
        {
            _dataContext = dataContext;
            InitilizeWcsWmsData();
        }

        private readonly WhAddressDataAbstract _dataContext;
        public List<string> GetAllShowName()
        {
            return _wcsWmsAddressTable.Select(addressModel => addressModel.ShowName).ToList();
        }

        public OperateResult Refresh()
        {
            InitilizeWcsWmsData();
            return OperateResult.CreateSuccessResult();
        }
        private void InitilizeWcsWmsData()
        {
            OperateResult<List<WhAddressModel>> getAllDataResult = _dataContext.GetAllData();
            if (!getAllDataResult.IsSuccess)
            {
                return;
            }
            List<WhAddressModel> autoCreateAddr = CreateWcsWmsData(2,4,20,1, "A01");
            getAllDataResult.Content.AddRange(autoCreateAddr);
            _wcsWmsAddressTable = getAllDataResult.Content;
        }

        private List<WhAddressModel> CreateWcsWmsData(int row, int layer, int column, int depth, string area)
        {
            List<WhAddressModel> wcsWmsAddr = new List<WhAddressModel>();
            for (int i = 1; i < row + 1; i++)
            {
                for (int j = 1; j < layer + 1; j++)
                {
                    for (int k = 1; k < column + 1; k++)
                    {
                        for (int l = 0; l < depth; l++)
                        {
                            WhAddressModel addr = new WhAddressModel();
                            addr.ShowName = string.Format("{4}库区{0}排{1}层{2}列{3}仓位", i, j, k, l == 0 ? "浅" : "深", area);
                            addr.IsSelected = false;
                            addr.LowerAddr = string.Format("Cell:{0}_{1}_{2}_{3}_{4}", i, j, k, l, area);
                            addr.UpperAddr = string.Format("Cell:{0}_{1}_{2}_{3}_{4}", i, j, k, l, area);
                            addr.WcsAddr = string.Format("Cell:{0}_{1}_{2}_{3}_{4}", i, j, k, l, area);
                            wcsWmsAddr.Add(addr);
                        }
                    }
                }
            }
            return wcsWmsAddr;
        }

        private List<WhAddressModel> _wcsWmsAddressTable = new List<WhAddressModel>();
        public OperateResult<string> WcsToWmsAddr(string destAddr)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                //1.检查是否存在配置专属的配置信息
                if (_wcsWmsAddressTable.Exists(d => d.WcsAddr.Equals(destAddr)))
                {
                    WhAddressModel firstOrDefault = _wcsWmsAddressTable.FirstOrDefault(d => d.WcsAddr.Equals(destAddr));
                    if (firstOrDefault != null)
                        result.Content = firstOrDefault.UpperAddr;
                    result.IsSuccess = true;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = destAddr;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<string> WmsToWcsAddr(string destAddr)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                //1.检查是否存在配置专属的配置信息

                if (_wcsWmsAddressTable.Exists(d => d.UpperAddr.Equals(destAddr)))
                {
                    WhAddressModel firstOrDefault = _wcsWmsAddressTable.FirstOrDefault(d => d.UpperAddr.Equals(destAddr));
                    if (firstOrDefault != null)
                    {
                        string wcsAddressFullName = firstOrDefault.WcsAddr;
                        result.Content = wcsAddressFullName;
                    }
                    result.IsSuccess = true;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = destAddr;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<string> WcsToLowerAddr(string wcsAddr)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                //1.检查是否存在配置专属的配置信息
                if (_wcsWmsAddressTable.Exists(d => d.WcsAddr.Equals(wcsAddr)))
                {
                    WhAddressModel firstOrDefault = _wcsWmsAddressTable.FirstOrDefault(d => d.WcsAddr.Equals(wcsAddr));
                    if (firstOrDefault != null)
                        result.Content = firstOrDefault.LowerAddr;
                    result.IsSuccess = true;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = wcsAddr;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<string> WcsToShowName(string wcsAddr)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                //1.检查是否存在配置专属的配置信息
                if (_wcsWmsAddressTable.Exists(d => d.WcsAddr.Equals(wcsAddr)))
                {
                    WhAddressModel firstOrDefault = _wcsWmsAddressTable.FirstOrDefault(d => d.WcsAddr.Equals(wcsAddr));
                    if (firstOrDefault != null)
                        result.Content = firstOrDefault.ShowName;
                    result.IsSuccess = true;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = wcsAddr;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<string> UpperAddrToShowName(string upperAddr)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                //1.检查是否存在配置专属的配置信息
                if (_wcsWmsAddressTable.Exists(d => d.WcsAddr.Equals(upperAddr)))
                {
                    WhAddressModel firstOrDefault = _wcsWmsAddressTable.FirstOrDefault(d => d.WcsAddr.Equals(upperAddr));
                    if (firstOrDefault != null)
                        result.Content = firstOrDefault.ShowName;
                    result.IsSuccess = true;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = upperAddr;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = OperateResult.ConvertException(ex);
            }
            return result;
        }

        public OperateResult<string> ShowNameToWcs(string name)
        {
            OperateResult<string> result = new OperateResult<string>();
            try
            {
                //1.检查是否存在配置专属的配置信息

                if (_wcsWmsAddressTable.Exists(d => d.ShowName.Equals(name)))
                {
                    WhAddressModel firstOrDefault = _wcsWmsAddressTable.FirstOrDefault(d => d.ShowName.Equals(name));
                    if (firstOrDefault != null)
                    {
                        string wcsAddressFullName = firstOrDefault.WcsAddr;
                        result.Content = wcsAddressFullName;
                    }
                    result.IsSuccess = true;
                    return result;
                }
                result.IsSuccess = true;
                result.Content = name;
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
