namespace PKSim.Core.Snapshots
{
   public class ExplicitFormula : SnapshotBase
   {
      public string Formula { get; set; }
      public FormulaUsablePath[] References { get; set; }
      public string Dimension { get; set; }
   }
}