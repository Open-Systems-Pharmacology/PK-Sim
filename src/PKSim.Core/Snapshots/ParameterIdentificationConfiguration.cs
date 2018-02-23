namespace PKSim.Core.Snapshots
{
   public class ParameterIdentificationConfiguration 
   {
      public string LLOQMode { get; set; }
      public string RemoveLLOQMode { get; set; }
      public bool CalculateJacobian { get; set; }
      public OptimizationAlgorithm Algorithm { get; set; }
      public ParameterIdentificationRunMode RunMode { get; set; }
   }
}