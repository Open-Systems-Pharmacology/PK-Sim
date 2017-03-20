using OSPSuite.BDDHelper;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Services;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation
{
   public abstract class concern_for_ShowSimulationResultsCommand : ContextSpecification<ShowSimulationResultsCommand>
   {
      protected ISimulationAnalysisCreator _simulationAnalysisCreator;
      protected IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _simulationAnalysisCreator = A.Fake<ISimulationAnalysisCreator>();
         _activeSubjectRetriever= A.Fake<IActiveSubjectRetriever>();
         sut = new ShowSimulationResultsCommand(_simulationAnalysisCreator,_activeSubjectRetriever);
      }
   }

   
   public class When_the_show_simulation_result_command_is_being_executed : concern_for_ShowSimulationResultsCommand
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation= A.Fake<Simulation>();
         A.CallTo(() => _activeSubjectRetriever.Active<Simulation>()).Returns(_simulation);
      }
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_simulation_plot_creator_command_to_create_a_plot_for_the_active_simulation()
      {
         A.CallTo(() => _simulationAnalysisCreator.CreateAnalysisFor(_simulation)).MustHaveHappened();
      }
   }
}