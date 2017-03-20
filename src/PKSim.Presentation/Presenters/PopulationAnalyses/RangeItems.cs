using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{

   public static class RangeItems
   {
      public static readonly PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter> ParameterSelection = new PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter> PKParameterSpecification = new PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IRangeAnalysisResultsPresenter> Results = new PopulationAnalysisItem<IRangeAnalysisResultsPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { ParameterSelection, PKParameterSpecification, Results };
   }

}