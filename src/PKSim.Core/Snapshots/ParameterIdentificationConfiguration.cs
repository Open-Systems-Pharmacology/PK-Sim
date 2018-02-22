namespace PKSim.Core.Snapshots
{
   public class ParameterIdentificationConfiguration : SnapshotBase
   {
      public string LLOQMode { get; set; }
      public string RemoveLLOQMode { get; set; }
      public bool CalculateJacobian { get; set; }
      public ExtendedProperty[] Properties { get; set; }
   }
}