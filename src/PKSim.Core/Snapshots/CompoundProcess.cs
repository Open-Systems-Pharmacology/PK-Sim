namespace PKSim.Core.Snapshots
{
   public class CompoundProcess : ParameterContainerSnapshotBase
   {
      public string InternalName { get; set; }
      public string DataSource { get; set; }
      public string Species { get; set; }

      //Partial processes only
      public string Molecule { get; set; }

      //Enzymatic processes only
      public string Metabolite { get; set; }
   }
}