using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateTimeProfileAnalysisPresenter : ContextSpecification<ICreateTimeProfileAnalysisPresenter>
   {
      protected ICreatePopulationAnalysisView _view;
      protected ISubPresenterItemManager<IPopulationAnalysisItemPresenter> _subPresenterManager;
      protected IDialogCreator _dialogCreator;
      protected IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      protected IPopulationAnalysisChartFactory _populationAnalysisChartFactory;
      protected ILazyLoadTask _lazyLoadTask;
      protected IPopulationAnalysisParameterSelectionPresenter _parameterSelectionPresenter;
      protected IPopulationAnalysisPKParameterSelectionPresenter _pkParameterSpecificationPresenter;
      protected ICommandCollector _commandCollector;
      private IPopulationAnalysisTask _populationAnalysisTask;

      protected override void Context()
      {
         _view = A.Fake<ICreatePopulationAnalysisView>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _populationAnalysisTemplateTask = A.Fake<IPopulationAnalysisTemplateTask>();
         _populationAnalysisChartFactory = A.Fake<IPopulationAnalysisChartFactory>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _populationAnalysisTask = A.Fake<IPopulationAnalysisTask>();

         _subPresenterManager = SubPresenterHelper.Create<IPopulationAnalysisItemPresenter>();
         _parameterSelectionPresenter = _subPresenterManager.CreateFake(TimeProfileItems.ParameterSelection);
         _pkParameterSpecificationPresenter = _subPresenterManager.CreateFake(TimeProfileItems.PKParameterSpecification);

         sut = new CreateTimeProfileAnalysisPresenter(_view, _subPresenterManager, _dialogCreator, _populationAnalysisTemplateTask, _populationAnalysisChartFactory, _lazyLoadTask, _populationAnalysisTask);
      }
   }


   public class When_starting_the_create_time_profile_analysis_presenter : concern_for_CreateTimeProfileAnalysisPresenter
   {

      [Observation]
      public void should_hide_the_scaling_columns_for_pk_parameters_and_population_parameters ()
      {
         _parameterSelectionPresenter.ScalingVisible.ShouldBeFalse();
         _pkParameterSpecificationPresenter.ScalingVisible.ShouldBeFalse();
      }
   }
}