namespace PKSim.Core.Snapshots
{
   public class Observer : SnapshotBase
   {
      public string Dimension { get; set; }
      public DescriptorCondition[] ContainerCriteria { get; set; }
      public ExplicitFormula Formula { get; set; }
      public MoleculeList MoleculeList { get; set; }
      public string Type { get; set; }
   }
}