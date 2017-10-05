using OSPSuite.Core.Chart;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class ObservedDataCurveOptions
   {
      public string ColumnId { get; set; }
      public CurveOptions CurveOptions { get; }
      public string Caption { get; set; }

      public ObservedDataCurveOptions()
      {
         CurveOptions = new CurveOptions {LineStyle = LineStyles.None, Symbol = Symbols.Circle, Visible = true};
      }
   }
}