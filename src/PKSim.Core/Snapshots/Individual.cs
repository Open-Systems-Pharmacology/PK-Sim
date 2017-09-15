namespace PKSim.Core.Snapshots
{
   public class Individual : SnapshotBase
   {
      public int Seed { get; set; }

      public string Species { get; set; }
      public string Population { get; set; }
      public string Gender { get; set; }
      public Parameter Age { get; set; }
      public Parameter GestationalAge { get; set; }
      public Parameter Weight { get; set; }
      public Parameter Height { get; set; }

      public LocalizedParameter[] Parameters { get; set; }
      public Molecule[] Molecules { get; set; }
   }
}