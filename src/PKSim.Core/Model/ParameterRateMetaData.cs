using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class ParameterRateMetaData : ParameterMetaData
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public string RHSRate { get; set; }

      //TODO
      public ParameterRateMetaData DoClone()
      {
         return MemberwiseClone().DowncastTo<ParameterRateMetaData>();
      }
   }
}