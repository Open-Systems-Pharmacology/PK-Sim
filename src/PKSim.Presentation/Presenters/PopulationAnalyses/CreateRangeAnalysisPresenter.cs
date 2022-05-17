using PKSim.Assets;
using OSPSuite.Core.Services;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ICreateRangeAnalysisPresenter : ICreatePopulationAnalysisPresenter
   {
   }

   public class CreateRangeAnalysisPresenter : CreatePopulationAnalysisPresenter<PopulationPivotAnalysis, RangeAnalysisChart>, ICreateRangeAnalysisPresenter
   {
      public CreateRangeAnalysisPresenter(ICreatePopulationAnalysisView view, ISubPresenterItemManager<IPopulationAnalysisItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IPopulationAnalysisChartFactory populationAnalysisChartFactory, IPopulationAnalysisTask populationAnalysisTask, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory) :
            base(view, subPresenterItemManager, RangeItems.All, dialogCreator, populationAnalysisTemplateTask, populationAnalysisChartFactory, populationAnalysisTask, populationAnalysisFieldFactory)
      {
         View.ApplicationIcon = ApplicationIcons.RangeAnalysis;
      }

      protected override string AnalysisType => PKSimConstants.UI.Range;

      protected override ISubPresenterItem<IPopulationAnalysisResultsPresenter> ResultsPresenterItem => RangeItems.Results;
   }
}