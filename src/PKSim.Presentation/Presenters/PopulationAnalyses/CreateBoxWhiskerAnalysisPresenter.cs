using PKSim.Assets;
using OSPSuite.Core.Services;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ICreateBoxWhiskerAnalysisPresenter : ICreatePopulationAnalysisPresenter
   {
   }

   public class CreateBoxWhiskerAnalysisPresenter : CreatePopulationAnalysisPresenter<PopulationBoxWhiskerAnalysis, BoxWhiskerAnalysisChart>, ICreateBoxWhiskerAnalysisPresenter
   {
      public CreateBoxWhiskerAnalysisPresenter(ICreatePopulationAnalysisView view, ISubPresenterItemManager<IPopulationAnalysisItemPresenter> subPresenterItemManager, IDialogCreator dialogCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IPopulationAnalysisChartFactory populationAnalysisChartFactory, IPopulationAnalysisTask populationAnalysisTask, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory)
         : base(view, subPresenterItemManager, BoxWhiskerItems.All, dialogCreator, populationAnalysisTemplateTask, populationAnalysisChartFactory, populationAnalysisTask, populationAnalysisFieldFactory)
      {
         View.Image = ApplicationIcons.BoxWhiskerAnalysis;
      }

      protected override string AnalysisType
      {
         get { return PKSimConstants.UI.BoxWhisker; }
      }

      protected override ISubPresenterItem<IPopulationAnalysisResultsPresenter> ResultsPresenterItem
      {
         get { return BoxWhiskerItems.Chart; }
      }
   }
}