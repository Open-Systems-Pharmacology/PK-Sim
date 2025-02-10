using OSPSuite.Core.Commands;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPKSimSimulationOutputMappingPresenter : IEditIndividualSimulationItemPresenter, IEditPopulationSimulationItemPresenter
   {
   }

   public class PKSimSimulationOutputMappingPresenter : SimulationOutputMappingPresenter, IPKSimSimulationOutputMappingPresenter
   {
      public PKSimSimulationOutputMappingPresenter(ISimulationOutputMappingView view,
         IEntitiesInSimulationRetriever entitiesInSimulationRetriever,
         IObservedDataRepository observedDataRepository,
         ISimulationOutputMappingToOutputMappingDTOMapper outputMappingDTOMapper,
         IQuantityToSimulationQuantitySelectionDTOMapper simulationQuantitySelectionDTOMapper,
         IObservedDataTask observedDataTask,
         IEventPublisher eventPublisher,
         IOutputMappingMatchingTask outputMappingMatchingTask,
         IOSPSuiteExecutionContext executionContext) :
         base(view, entitiesInSimulationRetriever, observedDataRepository, outputMappingDTOMapper, simulationQuantitySelectionDTOMapper, observedDataTask, eventPublisher, outputMappingMatchingTask, executionContext)
      {
      }

      public void EditSimulation(IndividualSimulation simulation)
      {
         base.EditSimulation(simulation);
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         base.EditSimulation(simulation);
      }
   }
}