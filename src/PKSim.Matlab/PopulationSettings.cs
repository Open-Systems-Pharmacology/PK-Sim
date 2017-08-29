using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;

namespace PKSim.Matlab
{
   /// <summary>
   ///    Should only be used from matlab or batch component to create a population based on user data.
   ///    This structure will then be converted into a Core Population Settings
   /// </summary>
   public class PopulationSettings
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
      ///    Min age of individual to create in [year]. This is a mandatory input for age dependent species. If not defined, set to NaN
      /// </summary>
      public double MinAge { get; set; }

      /// <summary>
      ///    Max age of individual to create in [year]. This is a mandatory input for age dependent species. If not defined, set to NaN
      /// </summary>
      public double MaxAge { get; set; }

      /// <summary>
      /// Min gestational age of individual to create in [weeks]. This is a mandatory input for age dependent species. If not defined, set to PretermRange.Min
      /// </summary>
      public double MinGestationalAge { get; set; }

      /// <summary>
      /// Max gestational age of individual to create in [weeks]. This is a mandatory input for age dependent species. If not defined, set to PretermRange.Min
      /// </summary>
      public double MaxGestationalAge { get; set; }

      /// <summary>
      ///    Min weight of individual to create in [kg]. This is a mandatory input for all species. If unconstrained, set to NaN
      /// </summary>
      public double MinWeight { get; set; }

      /// <summary>
      ///    Max weight of individual to create in [kg]. This is a mandatory input for all species. If unconstrained, set to NaN
      /// </summary>
      public double MaxWeight { get; set; }

      /// <summary>
      ///    Min Height of individual to create in [cm]. This is an input for height dependent species. If unconstrained, set to NaN
      /// </summary>
      public double MinHeight { get; set; }

      /// <summary>
      ///    Max Height of individual to create in [cm]. This is an input for height dependent species. If unconstrained, set to NaN
      /// </summary>
      public double MaxHeight { get; set; }

      /// <summary>
      ///    Min BMI of individual to create in [kg/m²]. This is an input for height dependent species. If unconstrained, set to NaN
      /// </summary>
      public double MinBMI { get; set; }

      /// <summary>
      ///    Min BMI of individual to create in [kg/m²]. This is an input for height dependent species. If unconstrained, set to NaN
      /// </summary>
      public double MaxBMI { get; set; }

      /// <summary>
      /// Number of individuals to create
      /// </summary>
      public int NumberOfIndividuals { get; set; }

      /// <summary>
      /// Proportions of Females between 0 and 100
      /// </summary>
      public int ProportionOfFemales { get; set; }

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

      public ICache<string, string> AllCalculationMethods => _cmCache;

      public PopulationSettings()
      {
         Species = string.Empty;
         Population = string.Empty;
         MinAge = double.NaN;
         MaxAge = double.NaN;
         MinWeight = double.NaN;
         MaxWeight = double.NaN;
         MinHeight = double.NaN;
         MaxHeight = double.NaN;
         MinBMI = double.NaN;
         MaxBMI = double.NaN;
         MinGestationalAge = CoreConstants.PRETERM_RANGE.Min();
         MaxGestationalAge = CoreConstants.PRETERM_RANGE.Max();
         NumberOfIndividuals = CoreConstants.DEFAULT_NUMBER_OF_INDIVIDUALS_IN_POPULATION;
         _cmCache = new Cache<string, string> { OnMissingKey = x => string.Empty };
         ProportionOfFemales = 50;
      }
   }
}