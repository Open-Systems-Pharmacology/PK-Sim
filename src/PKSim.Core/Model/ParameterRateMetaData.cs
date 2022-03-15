namespace PKSim.Core.Model
{
   public class ParameterRateMetaData : ParameterMetaData
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public string RHSRate { get; set; }

      public override void UpdatePropertiesFrom(ParameterMetaData parameterMetaData)
      {
         base.UpdatePropertiesFrom(parameterMetaData);
         if (!(parameterMetaData is ParameterRateMetaData parameterRateMetaData)) return;
         CalculationMethod = parameterRateMetaData.CalculationMethod;
         Rate = parameterRateMetaData.Rate;
         RHSRate = parameterRateMetaData.RHSRate;
      }
   }
}