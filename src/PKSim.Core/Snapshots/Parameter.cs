using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Parameter : SnapshotBase
   {
      public double? Value { get; set; }
      public string Unit { get; set; }
      public string ValueDescription { get; set; }
      public TableFormula TableFormula { get; set; }
   }

   public class LocalizedParameter : Parameter
   {
      //Full path of parameter in container hierarchy
      [Required]
      public string Path { get; set; }

      public override string ToString()
      {
         return Path;
      }
   }
}