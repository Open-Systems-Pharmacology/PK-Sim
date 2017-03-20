using PKSim.Assets;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Services;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ICreateScatterAnalysisPresenter : ICreatePopulationAnalysisPresenter
   {
   }

   public class CreateScatterAnalysisPresenter : CreatePopulationAnalysisPresenter<PopulationPivotAnalysis, ScatterAnalysisChart>, ICreateScatterAnalysisPresenter
   {
      public CreateScatterAnalysisPresenter(ICreatePopulationAnalysisView view, ISubPresenterItemManager<IPopulationAnalysisItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IPopulationAnalysisChartFactory populationAnalysisChartFactory, IPopulationAnalysisTask populationAnalysisTask) :
         base(view, subPresenterItemManager, ScatterItems.All, dialogCreator, populationAnalysisTemplateTask, populationAnalysisChartFactory, populationAnalysisTask)
      {
         View.Image = ApplicationIcons.ScatterAnalysis;
      }
      protected override string AnalysisType => PKSimConstants.UI.Scatter;

      protected override ISubPresenterItem<IPopulationAnalysisResultsPresenter> ResultsPresenterItem => ScatterItems.Results;
   }
}