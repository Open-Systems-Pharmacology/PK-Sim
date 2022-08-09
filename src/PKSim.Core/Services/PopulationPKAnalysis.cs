using PKSim.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class PopulationPKAnalysis
   {
      public CurveData<TimeProfileXValue, TimeProfileYValue> CurveData;
      public PKAnalysis PKAnalysis { get; set; }
      public string ExtraDescription { get; private set; }

      public PopulationPKAnalysis(CurveData<TimeProfileXValue, TimeProfileYValue> curveData, PKAnalysis pkAnalysis, string extraDescription = "")
      {
         CurveData = curveData;
         PKAnalysis = pkAnalysis;
         ExtraDescription = extraDescription;
      }
   }
}