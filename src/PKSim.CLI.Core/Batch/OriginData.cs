using OSPSuite.Utility.Collections;

namespace PKSim.Core.Batch
{
   /// <summary>
   ///    Should only be used from matlab or batch component to create individual based on user data.
   ///    This structure will then be converted into a Core OriginData
   /// </summary>
   internal class OriginData
   {
      private readonly ICache<string, string> _cmCache;

      /// <summary>
      ///    Id of species (as defined in the database)
      /// </summary>
      public string Species { get; set; }

      /// <summary>
      ///    Id of population (as defined in the database)
      /// </summary>
      public string Population { get; set; }

      /// <summary>
      ///    Id of gender
      /// </summary>
      public string Gender { get; set; }

      /// <summary>
      ///    Age of individual to create in [year]. This is a mandatory input for age dependent species. If not defined, set to NaN
      /// </summary>
      public double Age { get; set; }

      /// <summary>
      ///    Gestational age of individual to create in [weeks]. If not defined, set to 40
      /// </summary>
      public double GestationalAge { get; set; }
      
      /// <summary>
      ///    Weight of individual to create in [kg]. This is a mandatory input for all species
      /// </summary>
      public double Weight { get; set; }

      /// <summary>
      ///    Height of individual to create in [dm]. This is a mandatory input for height dependent species. If not defined, set to NaN
      /// </summary>
      public double Height { get; set; }

      /// <summary>
      ///    add a calculation method for the given category
      /// </summary>
      /// <param name="category">Name of category</param>
      /// <param name="calculationMethod">Calculation method mapped to the category</param>
      public void AddCalculationMethod(string category, string calculationMethod)
      {
         _cmCache[category] = calculationMethod;
      }

      /// <summary>
      ///    returns the defined calculation method for the given category. An empty string is returned if information was not provided
      /// </summary>
      public string CalculationMethodFor(string category)
      {
         return _cmCache[category];
      }

      public OriginData()
      {
         Species = string.Empty;
         Population = string.Empty;
         Gender = string.Empty;
         Age = double.NaN;
         GestationalAge = double.NaN;
         Weight = double.NaN;
         Height = double.NaN;
         _cmCache = new Cache<string, string> {OnMissingKey = x => string.Empty};
      }
   }
}