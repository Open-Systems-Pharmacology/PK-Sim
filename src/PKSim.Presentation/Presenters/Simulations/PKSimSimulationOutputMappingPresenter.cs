using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Presentation.Mappers;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPKSimSimulationOutputMappingPresenter :  IEditIndividualSimulationItemPresenter//, IEditPopulationSimulationItemPresenter
   {
   }

   public class PKSimSimulationOutputMappingPresenter : SimulationOutputMappingPresenter, IPKSimSimulationOutputMappingPresenter
   {
      public PKSimSimulationOutputMappingPresenter(ISimulationOutputMappingView view, IEntitiesInSimulationRetriever entitiesInSimulationRetriever, IObservedDataRepository observedDataRepository, ISimulationOutputMappingToOutputMappingDTOMapper outputMappingDTOMapper, IQuantityToSimulationQuantitySelectionDTOMapper simulationQuantitySelectionDTOMapper, IParameterIdentificationTask parameterIdentificationTask) : base(view, entitiesInSimulationRetriever, observedDataRepository, outputMappingDTOMapper, simulationQuantitySelectionDTOMapper, parameterIdentificationTask)
      {
      }

      public void EditSimulation(IndividualSimulation simulation)
      {
         SetSimulation(simulation);
      }
   }
}