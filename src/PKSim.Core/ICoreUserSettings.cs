using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   /// <summary>
   ///    User specific settings influencing the model creation in any ways
   /// </summary>
   public interface ICoreUserSettings : OSPSuite.Core.ICoreUserSettings
   {
      /// <summary>
      ///    Name of default species that should be used throughout the program for default initialization
      /// </summary>
      string DefaultSpecies { get; set; }

      /// <summary>
      ///    Name of default population in the default species
      /// </summary>
      string DefaultPopulation { get; set; }

      /// <summary>
      ///    Absolute tolerance used by defaut in  a simulation
      /// </summary>
      double AbsTol { get; set; }

      /// <summary>
      ///    Absolute tolerance used by defaut in  a simulation
      /// </summary>
      double RelTol { get; set; }

      /// <summary>
      ///    Default name use for lipophilicity alternatives
      /// </summary>
      string DefaultLipophilicityName { get; set; }

      /// <summary>
      ///    Default name used for fraction unbound alternatives
      /// </summary>
      string DefaultFractionUnboundName { get; set; }

      /// <summary>
      ///    Default name used for solubility alternatives
      /// </summary>
      string DefaultSolubilityName { get; set; }

      /// <summary>
      ///    Returns the default settings or null if not found
      /// </summary>
      OutputSelections OutputSelections { get; set; }
      
      /// <summary>
      ///    Default population analysis type used when starting a population
      /// </summary>
      PopulationAnalysisType DefaultPopulationAnalysis { get; set; }

      /// <summary>
      ///    Path of the template database containing the template building block
      /// </summary>
      string TemplateDatabasePath { get; set; }
   }
}