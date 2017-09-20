using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationSettingsTask : ContextSpecification<ISimulationSettingsTask>
   {
      protected IOutputIntervalFactory _outputIntervalFactory;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _outputIntervalFactory = A.Fake<IOutputIntervalFactory>();
         _executionContext = A.Fake<IExecutionContext>();
         sut = new SimulationSettingsTask(_executionContext, _outputIntervalFactory);
      }
   }

   public class When_the_simulation_setting_task_is_adding_a_simulation_interval_to_a_simulation_output : concern_for_SimulationSettingsTask
   {
      private OutputSchema _simulationSche;
      private OutputInterval _defaultInterval;
      private ICommand _result;

      protected override void Context()
      {
         base.Context();
         _simulationSche = A.Fake<OutputSchema>();
         _defaultInterval = A.Fake<OutputInterval>().WithName("oldName");
         A.CallTo(() => _outputIntervalFactory.CreateDefaultFor(_simulationSche)).Returns(_defaultInterval);
      }

      protected override void Because()
      {
         _result = sut.AddSimulationIntervalTo(_simulationSche);
      }

      [Observation]
      public void should_add_the_default_simulation_interval_to_the_simulation_output()
      {
         A.CallTo(() => _simulationSche.Add(_defaultInterval)).MustHaveHappened();
      }

      [Observation]
      public void the_resulting_command_should_be_of_type_add_simulation_interval_to_simulation_output_command()
      {
         _result.ShouldBeAnInstanceOf<AddSimulationIntervalToSimulationOutputCommand>();
      }

   }

   public class When_the_simulation_setting_task_is_removing_a_simulation_interval_from_a_simulation_output_with_only_one_interval : concern_for_SimulationSettingsTask
   {
      private OutputSchema _simulationOutput;
      private OutputInterval _simulatinIntervalToRemove;

      protected override void Context()
      {
         base.Context();
         _simulationOutput = A.Fake<OutputSchema>();
         _simulatinIntervalToRemove = A.Fake<OutputInterval>();
         A.CallTo(() => _simulationOutput.Intervals).Returns(new[] {_simulatinIntervalToRemove});
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.RemoveSimulationIntervalFrom(_simulatinIntervalToRemove, _simulationOutput)).ShouldThrowAn<CannotDeleteSimulationIntervalException>();
      }
   }
}