namespace PKSim.Core.Snapshots
{
   public class ParameterRange 
   {
      //Name of parameter range. Only set for dynamic parameters
      public string Name { get; set; }
      public double? Min { get; set; }
      public double? Max { get; set; }
      public string Unit { get; set; }
   }
}  