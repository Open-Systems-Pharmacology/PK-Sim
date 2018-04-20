using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Parameter : SnapshotBase
   {
      /// <summary>
      /// Value of parameter in <see cref="Unit"/>
      /// </summary>
      public double? Value { get; set; }

      /// <summary>
      /// Unit of parameter. If unit is not defined, it is assume that the <see cref="Value"/> is in default unit for the dimensions
      /// </summary>
      public string Unit { get; set; }

      public ValueOrigin ValueOrigin { get; set; }
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