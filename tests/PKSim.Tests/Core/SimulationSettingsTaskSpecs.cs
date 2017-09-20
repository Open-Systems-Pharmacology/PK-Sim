using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
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
      private OutputInterval _defaultInterval;
      private ICommand _result;
      private string _newName;
      private OutputSchema _outputSchema;

      protected override void Context()
      {
         base.Context();
         _newName = "Tralala";
         _outputSchema = new OutputSchema();
         _defaultInterval = new OutputInterval().WithName("oldName");
         A.CallTo(() => _outputIntervalFactory.CreateDefault()).Returns(_defaultInterval);
         A.CallTo(() => _containerTask.CreateUniqueName(_outputSchema, _defaultInterval.Name, false)).Returns(_newName);
      }

      protected override void Because()
      {
         _result = sut.AddSimulationIntervalTo(_outputSchema);
      }

      [Observation]
      public void should_add_the_default_simulation_interval_to_the_simulation_output()
      {
         _outputSchema.Intervals.ShouldContain(_defaultInterval);
      }

      [Observation]
      public void the_resulting_command_should_be_of_type_add_simulation_interval_to_simulation_output_command()
      {
         _result.ShouldBeAnInstanceOf<AddSimulationIntervalToSimulationOutputCommand>();
      }

      [Observation]
      public void should_have_set_the_name_of_the_interval_to_the_default_interval_name()
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