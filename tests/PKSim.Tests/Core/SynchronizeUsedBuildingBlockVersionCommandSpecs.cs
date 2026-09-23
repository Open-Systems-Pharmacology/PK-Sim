using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_SynchronizeUsedBuildingBlockVersionCommand : ContextSpecification<SynchronizeUsedBuildingBlockVersionCommand>
   {
      protected IExecutionContext _executionContext;
      protected IndividualSimulation _simulation;
      protected Compound _templateCompound;
      protected UsedBuildingBlock _usedCompound;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateCompId", Version = 7 };
         var simulationCompound = new Compound { Name = "Aspirin", Id = "SimCompId" };
         _usedCompound = new UsedBuildingBlock("TemplateCompId", PKSimBuildingBlockType.Compound) { BuildingBlock = simulationCompound, Version = 3 };

         _simulation = new IndividualSimulation { Id = "SimId", Name = "Sim" };
         _simulation.AddUsedBuildingBlock(_usedCompound);
         A.CallTo(() => _executionContext.Get<Simulation>(_simulation.Id)).Returns(_simulation);
         A.CallTo(() => _executionContext.Get<IPKSimBuildingBlock>(_templateCompound.Id)).Returns(_templateCompound);

         sut = new SynchronizeUsedBuildingBlockVersionCommand(_simulation, _usedCompound, _templateCompound, _executionContext);
      }
   }

   public class When_synchronizing_the_version_of_a_used_building_block_with_its_template : concern_for_SynchronizeUsedBuildingBlockVersionCommand
   {
      protected override void Because()
      {
         //the template changes before the command executes
         _templateCompound.Version = 8;
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_set_the_version_the_template_has_at_execution_time()
      {
         _usedCompound.Version.ShouldBeEqualTo(8);
      }

      [Observation]
      public void should_publish_a_simulation_status_changed_event()
      {
         A.CallTo(() => _executionContext.PublishEvent(A<SimulationStatusChangedEvent>.That.Matches(x => x.Simulation == _simulation))).MustHaveHappened();
      }

      [Observation]
      public void should_not_be_shown_in_the_history()
      {
         sut.Visible.ShouldBeFalse();
      }
   }

   public class When_undoing_the_synchronization_of_the_version_of_a_used_building_block : concern_for_SynchronizeUsedBuildingBlockVersionCommand
   {
      protected override void Because()
      {
         sut.ExecuteAndInvokeInverse(_executionContext);
      }

      [Observation]
      public void should_restore_the_previous_version()
      {
         _usedCompound.Version.ShouldBeEqualTo(3);
      }
   }
}
