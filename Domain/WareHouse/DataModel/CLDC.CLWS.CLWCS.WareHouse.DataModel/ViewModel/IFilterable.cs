namespace CLDC.CLWS.CLWCS.WareHouse.DataModel.ViewModel
{
   public interface IFilterable
   {
       /// <summary>
       /// 条件过滤
       /// </summary>
       /// <param name="condition"></param>
       /// <returns></returns>
       bool Filter(string condition);
   }
}
