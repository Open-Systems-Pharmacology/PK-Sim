using OSPSuite.Utility;
using DevExpress.XtraCharts;
using OSPSuite.Core.Chart;

namespace PKSim.UI.Mappers
{
   public interface ILineStyleToDashStyleMapper : IMapper<LineStyles, DashStyle>
   {
   }

   public class LineStyleToDashStyleMapper : ILineStyleToDashStyleMapper
   {
      public DashStyle MapFrom(LineStyles lineStyle)
      {
         switch (lineStyle)
         {
            case LineStyles.None:
               return DashStyle.Empty;
            default:
               return EnumHelper.ParseValue<DashStyle>(lineStyle.ToString());
         }
      }
   }
}