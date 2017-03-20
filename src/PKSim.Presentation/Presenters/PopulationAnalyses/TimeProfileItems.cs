using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public class TimeProfileItems
   {
      public static readonly PopulationAnalysisItem<IPopulationAnalysisOutputSelectionPresenter> OutputSelection = new PopulationAnalysisItem<IPopulationAnalysisOutputSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter> ParameterSelection = new PopulationAnalysisItem<IPopulationAnalysisParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter> PKParameterSpecification = new PopulationAnalysisItem<IPopulationAnalysisPKParameterSelectionPresenter>();
      public static readonly PopulationAnalysisItem<ITimeProfileAnalysisResultsPresenter> Chart = new PopulationAnalysisItem<ITimeProfileAnalysisResultsPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {OutputSelection, ParameterSelection, PKParameterSpecification, Chart};
   }
}