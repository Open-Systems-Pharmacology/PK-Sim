using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Events;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataInSimulationManager : ContextSpecification<IObservedDataInSimulationManager>
   {
      protected IBuildingBlockRepository _simulationRepository;
      protected IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _simulationRepository = A.Fake<IBuildingBlockRepository>();
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new ObservedDataInSimulationManager(_simulationRepository, _eventPublisher);
      }
   }

   public class When_retrieving_the_simulation_using_some_observed_data : concern_for_ObservedDataInSimulationManager
   {
      private Simulation _sim1;
      private Simulation _sim2;
      private Simulation _sim3;
      private DataRepository _obsData;

      protected override void Context()
      {
         base.Context();
         _sim1 = new IndividualSimulation();
         _sim2 = new IndividualSimulation();
         _sim3 = new IndividualSimulation();
         _obsData = new DataRepository();
         _sim1.AddUsedObservedData(_obsData);
         _sim3.AddUsedObservedData(_obsData);
         A.CallTo(() => _simulationRepository.All<Simulation>()).Returns(new[] {_sim1, _sim2, _sim3});
      }

      [Observation]
      public void should_return_the_simulation_defined_in_the_project_referencing_the_observed_data()
      {
         sut.SimulationsUsing(_obsData).ShouldOnlyContain(_sim1, _sim3);
      }
   }

   public class When_asked_to_update_the_simulations_using_a_given_observed_dtaa : concern_for_ObservedDataInSimulationManager
   {
      private Simulation _sim1;
      private Simulation _sim2;
      private Simulation _sim3;
      private DataRepository _obsData;
      private IList<SimulationStatusChangedEvent> _events;

      protected override void Context()
      {
         base.Context();
         base.Context();
         _events = new List<SimulationStatusChangedEvent>();
         _sim1 = A.Fake<Simulation>();
         _sim2 = A.Fake<Simulation>();
         _sim3 = A.Fake<Simulation>();
         _obsData = A.Fake<DataRepository>();
         A.CallTo(() => _sim1.UsesObservedData(_obsData)).Returns(true);
         A.CallTo(() => _sim2.UsesObservedData(_obsData)).Returns(false);
         A.CallTo(() => _sim3.UsesObservedData(_obsData)).Returns(true);
         A.CallTo(() => _simulationRepository.All<Simulation>()).Returns(new[] {_sim1, _sim2, _sim3});
         A.CallTo(() => _eventPublisher.PublishEvent(A<SimulationStatusChangedEvent>.Ignored)).Invokes(
            x => _events.Add(x.GetArgument<SimulationStatusChangedEvent>(0)));
      }

      protected override void Because()
      {
         sut.UpdateObservedDataInSimulationsUsing(_obsData);
      }

      [Observation]
      public void should_notify_a_simulation_status_change_event_for_all_simulations_using_the_observed_data()
      {
         _events.Count.ShouldBeEqualTo(2); //2 simulations
         _events[0].Simulation.ShouldBeEqualTo(_sim1);
         _events[1].Simulation.ShouldBeEqualTo(_sim3);
      }
   }
}