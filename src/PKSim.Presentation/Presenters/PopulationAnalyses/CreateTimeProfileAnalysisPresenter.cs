using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ICreateTimeProfileAnalysisPresenter : ICreatePopulationAnalysisPresenter
   {
   }

   public class CreateTimeProfileAnalysisPresenter : CreatePopulationAnalysisPresenter<PopulationStatisticalAnalysis, TimeProfileAnalysisChart>, ICreateTimeProfileAnalysisPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;

      public CreateTimeProfileAnalysisPresenter(ICreatePopulationAnalysisView view, ISubPresenterItemManager<IPopulationAnalysisItemPresenter> subPresenterItemManager,
         IDialogCreator dialogCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IPopulationAnalysisChartFactory populationAnalysisChartFactory, ILazyLoadTask lazyLoadTask, IPopulationAnalysisTask populationAnalysisTask, IPopulationAnalysisFieldFactory populationAnalysisFieldFactory)
         : base(view, subPresenterItemManager, TimeProfileItems.All, dialogCreator, populationAnalysisTemplateTask, populationAnalysisChartFactory, populationAnalysisTask, populationAnalysisFieldFactory)
      {
         _lazyLoadTask = lazyLoadTask;
         View.ApplicationIcon = ApplicationIcons.TimeProfileAnalysis;
      }

      protected override string AnalysisType => PKSimConstants.UI.TimeProfile;

      protected override bool Edit(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart<PopulationStatisticalAnalysis> populationAnalysisChart)
      {
         _lazyLoadTask.LoadResults(populationDataCollector);
         return base.Edit(populationDataCollector, populationAnalysisChart);
      }

      public override void InitializeWith(ICommandCollector initializer)
      {
         base.InitializeWith(initializer);
         PresenterAt(TimeProfileItems.PKParameterSpecification).ScalingVisible = false;
         PresenterAt(TimeProfileItems.ParameterSelection).ScalingVisible = false;
      }

      protected override ISubPresenterItem<IPopulationAnalysisResultsPresenter> ResultsPresenterItem => TimeProfileItems.Chart;
   }
}