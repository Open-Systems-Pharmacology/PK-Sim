using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public static class ScatterItems
   {
      public static readonly PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter> ParameterSelection = new PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter> PKParameterSpecification = new PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IScatterAnalysisResultsPresenter> Results = new PopulationAnalysisItem<IScatterAnalysisResultsPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { ParameterSelection, PKParameterSpecification, Results };
   }



  
}