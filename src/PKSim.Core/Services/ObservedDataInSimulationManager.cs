using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;

namespace PKSim.Core.Services
{
   public interface IObservedDataInSimulationManager
   {
      /// <summary>
      ///    Returns all the simulation using the given observed data
      /// </summary>
      /// <param name="observedData">data repository used to filter the simulation</param>
      IEnumerable<Simulation> SimulationsUsing(DataRepository observedData);

      /// <summary>
      ///    Trigger an update of all simulatuon using the given data repository (for instance after a rename)
      /// </summary>
      void UpdateObservedDataInSimulationsUsing(DataRepository dataRepository);
   }

   public class ObservedDataInSimulationManager : IObservedDataInSimulationManager
   {
      private readonly IBuildingBlockRepository _simulationRepository;
      private readonly IEventPublisher _eventPublisher;

      public ObservedDataInSimulationManager(IBuildingBlockRepository simulationRepository, IEventPublisher eventPublisher)
      {
         _simulationRepository = simulationRepository;
         _eventPublisher = eventPublisher;
      }

      public IEnumerable<Simulation> SimulationsUsing(DataRepository observedData)
      {
         return from sim in _simulationRepository.All<Simulation>()
            where sim.UsesObservedData(observedData)
            select sim;
      }

      public void UpdateObservedDataInSimulationsUsing(DataRepository dataRepository)
      {
         SimulationsUsing(dataRepository).Each(s => _eventPublisher.PublishEvent(new SimulationStatusChangedEvent(s)));
      }
   }
}