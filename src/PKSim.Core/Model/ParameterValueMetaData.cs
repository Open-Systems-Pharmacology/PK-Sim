namespace PKSim.Core.Model
{
   public class ParameterValueMetaData : ParameterMetaData
   {
      public double DefaultValue { get; set; }
      public string ParameterValueVersion { get; set; }
      public string Species { get; set; }

      public ParameterValueMetaData()
      {
         MinValue = 0;
         MinIsAllowed = true;
      }
   }
}