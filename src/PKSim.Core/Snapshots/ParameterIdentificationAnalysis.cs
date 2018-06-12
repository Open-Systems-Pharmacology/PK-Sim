using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class ParameterIdentificationAnalysis : SnapshotBase
   {
      [Required]
      public string Type { get; set; }

      /// <summary>
      /// Set only of chart based analysis. Null otherwise
      /// </summary>
      public CurveChart Chart { get; set; }

      /// <summary>
      /// Local data repositories required for some specific chart types
      /// </summary>
      public DataRepository[] DataRepositories { get; set; }
   }
}