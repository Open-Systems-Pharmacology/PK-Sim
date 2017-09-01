namespace PKSim.Core.Snapshots
{
   public class Protocol : ParameterContainerSnapshotBase
   {
      public string ApplicationType { get; set; }
      public string DosingInterval { get; set; }
      public string TargetOrgan { get; set; }
      public string TargetCompartment { get; internal set; }
      public bool IsSimple => !string.IsNullOrEmpty(ApplicationType) && !string.IsNullOrEmpty(DosingInterval);
   }
}