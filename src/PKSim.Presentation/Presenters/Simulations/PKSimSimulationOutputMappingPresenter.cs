using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPKSimSimulationOutputMappingPresenter : IEditIndividualSimulationItemPresenter, IEditPopulationSimulationItemPresenter
   {
   }

   public class PKSimSimulationOutputMappingPresenter : SimulationOutputMappingPresenter, IPKSimSimulationOutputMappingPresenter
   {
      public PKSimSimulationOutputMappingPresenter(ISimulationOutputMappingView view, IEntitiesInSimulationRetriever entitiesInSimulationRetriever,
         IObservedDataRepository observedDataRepository, ISimulationOutputMappingToOutputMappingDTOMapper outputMappingDTOMapper,
         IQuantityToSimulationQuantitySelectionDTOMapper simulationQuantitySelectionDTOMapper) : base(view, entitiesInSimulationRetriever,
         observedDataRepository,
         outputMappingDTOMapper, simulationQuantitySelectionDTOMapper)
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