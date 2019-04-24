using OSPSuite.Core.Domain;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIUserSettings : Notifier, ICoreUserSettings
   {
      public IBusinessRuleSet Rules { get; private set; }
      public int NumberOfBins { get; set; }
      public int NumberOfIndividualsPerBin { get; set; }
      public string DefaultSpecies { get; set; } = CoreConstants.Species.HUMAN;
      public string DefaultPopulation { get; set; } = CoreConstants.Population.ICRP;
      public double AbsTol { get; set; }
      public double RelTol { get; set; }
      public string DefaultLipophilicityName { get; set; }
      public string DefaultFractionUnboundName { get; set; }
      public string DefaultSolubilityName { get; set; }
      public OutputSelections OutputSelections { get; set; }
      public int MaximumNumberOfCoresToUse { get; set; }
      public PopulationAnalysisType DefaultPopulationAnalysis { get; set; }
      public string TemplateDatabasePath { get; set; }

      public void ResetToDefault()
      {
         AbsTol = CoreConstants.DEFAULT_ABS_TOL;
         RelTol = CoreConstants.DEFAULT_REL_TOL;
         NumberOfBins = CoreConstants.DEFAULT_NUMBER_OF_BINS;
         DefaultLipophilicityName = PKSimConstants.UI.DefaultAlternative;
         DefaultFractionUnboundName = PKSimConstants.UI.DefaultAlternative;
         DefaultSolubilityName = PKSimConstants.UI.DefaultAlternative;
      }

      public CLIUserSettings()
      {
         ResetToDefault();
      }
   }
}