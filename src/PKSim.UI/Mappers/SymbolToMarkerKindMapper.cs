using OSPSuite.Utility;
using DevExpress.XtraCharts;
using OSPSuite.Core.Chart;

namespace PKSim.UI.Mappers
{
   public interface ISymbolToMarkerKindMapper : IMapper<Symbols, MarkerKind>
   {
   }

   public class SymbolToMarkerKindMapper : ISymbolToMarkerKindMapper
   {
      public MarkerKind MapFrom(Symbols symbol)
      {
         switch (symbol)
         {
            case Symbols.None:
               return MarkerKind.Cross;
            default:
               return EnumHelper.ParseValue<MarkerKind>(symbol.ToString());
         }
      }
   }
}