using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationSettingsTask : ContextSpecification<ISimulationSettingsTask>
   {
      protected IOutputIntervalFactory _outputIntervalFactory;
      protected IExecutionContext _executionContext;
      protected IContainerTask _containerTask;

      protected override void Context()
      {
         _outputIntervalFactory = A.Fake<IOutputIntervalFactory>();
         _executionContext = A.Fake<IExecutionContext>();
         _containerTask = A.Fake<IContainerTask>();
         sut = new SimulationSettingsTask(_executionContext, _outputIntervalFactory, _containerTask);
      }
   }

   public class When_the_simulation_setting_task_is_adding_a_simulation_interval_to_a_simulation_output : concern_for_SimulationSettingsTask
   {
      private OutputSchema _simulationOutput;
      private OutputInterval _defaultInterval;
      private ICommand _result;
      private string _newName;

      protected override void Context()
      {
         base.Context();
         _simulationOutput = A.Fake<OutputSchema>();
         _defaultInterval = A.Fake<OutputInterval>().WithName("oldName");
         _newName = "Tralala";
         A.CallTo(() => _outputIntervalFactory.CreateDefault()).Returns(_defaultInterval);
         A.CallTo(() => _containerTask.CreateUniqueName(_simulationOutput, _defaultInterval.Name,  false)).Returns(_newName);
      }

      protected override void Because()
      {
         _result = sut.AddSimulationIntervalTo(_simulationOutput);
      }

      [Observation]
      public void should_add_the_default_simulation_interval_to_the_simulation_output()
      {
         A.CallTo(() => _simulationOutput.Add(_defaultInterval)).MustHaveHappened();
      }

      [Observation]
      public void the_resulting_command_should_be_of_type_add_simulation_interval_to_simulation_output_command()
      {
         _result.ShouldBeAnInstanceOf<AddSimulationIntervalToSimulationOutputCommand>();
      }

      [Observation]
      public void should_have_updated_the_name_of_the_interval_to_represent_a_unique_name_in_the_simulation_output()
      {
         _defaultInterval.Name.ShouldBeEqualTo(_newName);
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