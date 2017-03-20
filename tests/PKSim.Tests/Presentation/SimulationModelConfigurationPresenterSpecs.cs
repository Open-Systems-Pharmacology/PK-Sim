using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationModelConfigurationPresenter : ContextSpecification<ISimulationModelConfigurationPresenter>
   {
      private ISimulationFactory _simulationFactory;
      private ISimulationModelConfigurationView _view;
      protected ISubPresenterItemManager<ISimulationModelConfigurationItemPresenter> _subPresenterManager;
      protected ISimulationModelSelectionPresenter _simulationModelSelectionPresenter;
      protected ISimulationSubjectConfigurationPresenter _simulationSubjectConfigurationPresenter;

      protected override void Context()
      {
         _simulationFactory= A.Fake<ISimulationFactory>();
         _view= A.Fake<ISimulationModelConfigurationView>();
         _subPresenterManager = SubPresenterHelper.Create<ISimulationModelConfigurationItemPresenter>();
         _simulationSubjectConfigurationPresenter = _subPresenterManager.CreateFake(SimulationModelConfigurationItems.Subject);
         _simulationModelSelectionPresenter= _subPresenterManager.CreateFake(SimulationModelConfigurationItems.ModelSelection);

         sut = new SimulationModelConfigurationPresenter(_view, _subPresenterManager, _simulationFactory);
      }
   }

   public class When_starting_the_simulation_configuration_presenter : concern_for_SimulationModelConfigurationPresenter
   {
      private ICommandCollector _commandCollector;
      private ISimulationSubject _selectedSubject;

      protected override void Context()
      {
         base.Context();
         _selectedSubject= A.Fake<ISimulationSubject>();
         _commandCollector= A.Fake<ICommandCollector>();
         A.CallTo(() => _simulationSubjectConfigurationPresenter.SelectedSubject).Returns(_selectedSubject);
      }

      protected override void Because()
      {
         sut.InitializeWith(_commandCollector);
      }

      [Observation]
      public void should_initialize_the_sub_presenters()
      {
         A.CallTo(() => _subPresenterManager.InitializeWith(sut, SimulationModelConfigurationItems.All)).MustHaveHappened();   
      }

      [Observation]
      public void should_trigger_an_initialization_of_the_model_selection_presenter_based_on_the_current_selection()
      {
         A.CallTo(() => _simulationModelSelectionPresenter.EditModelConfiguration(_selectedSubject)).MustHaveHappened();
      }
   }
}	