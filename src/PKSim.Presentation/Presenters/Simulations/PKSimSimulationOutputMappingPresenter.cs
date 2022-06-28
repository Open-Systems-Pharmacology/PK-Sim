using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Mappers.ParameterIdentifications;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPKSimSimulationOutputMappingPresenter :  IEditIndividualSimulationItemPresenter, IEditPopulationSimulationItemPresenter
   {
      /// <summary>
      ///    Sets the subject of the mapped Outputs to the simulation
      /// </summary>
      /// <param name="simulation">The simulation containing the mapped outputs being displayed</param>
      void Edit(Simulation simulation);
   }

   public class PKSimSimulationOutputMappingPresenter : SimulationOutputMappingPresenter, IPKSimSimulationOutputMappingPresenter
   {
      public PKSimSimulationOutputMappingPresenter(ISimulationOutputMappingView view, IEntitiesInSimulationRetriever entitiesInSimulationRetriever, IObservedDataRepository observedDataRepository, ISimulationOutputMappingToOutputMappingDTOMapper outputMappingDTOMapper, IQuantityToSimulationQuantitySelectionDTOMapper simulationQuantitySelectionDTOMapper, IParameterIdentificationTask parameterIdentificationTask) : base(view, entitiesInSimulationRetriever, observedDataRepository, outputMappingDTOMapper, simulationQuantitySelectionDTOMapper, parameterIdentificationTask)
      {
      }

      public void EditSimulation(IndividualSimulation simulation)
      {
         Edit(simulation);
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         Edit(simulation);
      }

      public void Edit(Simulation simulation)
      {
         SetSimulation(simulation);
      }

      public void InitializeWith(ICommandCollector initializer)
      {
         //
      }

      public void AddCommand(ICommand command)
      {
         //
      }

      public IEnumerable<ICommand> All()
      {
         return new List<ICommand>();
      }

      public ICommandCollector CommandCollector { get; }
      public ApplicationIcon Icon { get; }
   }
}