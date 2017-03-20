using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public static class BoxWhiskerItems
   {
      public static readonly PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter> ParameterSelection = new PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter> PKParameterSpecification = new PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IBoxWhiskerAnalysisResultsPresenter> Chart = new PopulationAnalysisItem<IBoxWhiskerAnalysisResultsPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {ParameterSelection, PKParameterSpecification, Chart};
   }
}