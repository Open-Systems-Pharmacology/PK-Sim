namespace PKSim.Core.Snapshots
{
   public class SchemaItem: ParameterContainerSnapshotBase
   {
      public string ApplicationType { get; set; }
      public string FormulationKey { get; set; }
      public string TargetOrgan { get; set; }
      public string TargetCompartment { get; set; }
   }
}