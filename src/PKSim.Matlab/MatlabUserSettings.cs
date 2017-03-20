using System.Collections.Generic;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Matlab
{
   public class MatlabUserSettings : Notifier, ICoreUserSettings
   {
      public int MaximumNumberOfCoresToUse { get; set; }
      public Scalings DefaultChartYScaling { get; set; }
      public PopulationAnalysisType DefaultPopulationAnalysis { get; set; }
      public string ActiveSkin { get; set; }
      public int NumberOfBins { get; set; }
      public int NumberOfIndividualsPerBin { get; set; }
      public string MainViewLayout { get; set; }
      public string RibbonLayout { get; set; }
      public string DefaultChartEditorLayout { get; set; }
      public string DefaultSpecies { get; set; }
      public string DefaultPopulation { get; set; }
      public string TemplateDatabasePath { get; set; }
      public int LayoutVersion { get; set; }
      public IList<string> ProjectFiles { get; set; }
      public double AbsTol { get; set; }
      public double RelTol { get; set; }
      public string DefaultLipophilicityName { get; set; }
      public string DefaultFractionUnboundName { get; set; }
      public string DefaultSolubilityName { get; set; }
      public IBusinessRuleSet Rules { get; private set; }
      public OutputSelections OutputSelections { get; set; }
   }
}