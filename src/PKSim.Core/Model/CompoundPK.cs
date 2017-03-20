using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   /// <summary>
   /// Reperesents a cache containing specific PK-Values calculated for a given compound
   /// </summary>
   public class CompoundPK
   {
      public string CompoundName { get; set; }
      
      /// <summary>
      /// AUC_inf of plasma curve following a IV infustion of 15 min
      /// </summary>
      public double? AucIV { get; set; }

      /// <summary>
      /// AUC_inf of plasma curve following an application where only <see cref="CompoundName"/> is applied
      /// </summary>
      public double? AucDDI { get; set; }

      /// <summary>
      /// Cmax of plasma curve following an application where only <see cref="CompoundName"/> is applied
      /// </summary>
      public double? CmaxDDI { get; set; }

      public CompoundPK Clone()
      {
         return new CompoundPK
         {
            AucDDI = AucDDI,
            AucIV = AucIV,
            CompoundName = CompoundName
         };
      }
   }
}