using PKSim.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class PopulationPKAnalysis
   {
      public CurveData<TimeProfileXValue, TimeProfileYValue> CurveData;
      public PKAnalysis PKAnalysis { get; set; }

      public PopulationPKAnalysis(CurveData<TimeProfileXValue, TimeProfileYValue> curveData, PKAnalysis pkAnalysis)
      {
         CurveData = curveData;
         PKAnalysis = pkAnalysis;
      }
   }
}