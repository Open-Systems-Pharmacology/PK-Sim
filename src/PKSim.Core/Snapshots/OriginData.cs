using System.ComponentModel.DataAnnotations;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Snapshots
{
   public class OriginData
   {
      public CalculationMethodCache CalculationMethods { get; set; }

      /// <summary>
      ///    Id of species (as defined in the database)
      /// </summary>
      [Required]
      public string Species { get; set; }

      /// <summary>
      ///    Id of population (as defined in the database)
      /// </summary>
      public string Population { get; set; }

      /// <summary>
      ///    Id of gender (as defined in the database)
      /// </summary>
      public string Gender { get; set; }

      /// <summary>
      ///    Age of individual to create. This is a mandatory input for age dependent species
      /// </summary>
      public Parameter Age { get; set; }

      /// <summary>
      ///    Gestational age of individual to create in [weeks]. This is a mandatory input for age dependent species
      /// </summary>
      public Parameter GestationalAge { get; set; }

      /// <summary>
      ///    Weight of individual to create. This is a mandatory input for all species
      /// </summary>
      [Required]
      public Parameter Weight { get; set; }

      /// <summary>
      ///    Height of individual to create in. This is a mandatory input for height dependent species. 
      /// </summary>
      public Parameter Height { get; set; }


      public void AddCalculationMethods(params string[] calculationMethods)
      {
         if (CalculationMethods == null)
            CalculationMethods = new CalculationMethodCache();

         calculationMethods?.Each(CalculationMethods.Add);
      }
   }
}