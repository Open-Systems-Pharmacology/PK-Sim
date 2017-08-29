using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    this is not a reversible command!
   /// </summary>
   public class SetPopulationSimulationResultsCommand : PKSimCommand
   {
      private PopulationSimulation _populationSimulation;
      private SimulationResults _simulationResults;

      public SetPopulationSimulationResultsCommand(PopulationSimulation populationSimulation, SimulationResults simulationResults)
      {
         _populationSimulation = populationSimulation;
         _simulationResults = simulationResults;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SetPopulationSimulationResultsCommandDescription(populationSimulation.Name);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var quantitiesRetriever = context.Resolve<IEntitiesInContainerRetriever>();
         var quantityCache = quantitiesRetriever.QuantitiesFrom(_populationSimulation);

         //create settings based on import
         _populationSimulation.OutputSelections.Clear();
         foreach (var quantityPath in _simulationResults.AllQuantityPaths())
         {
            var quantity = quantityCache[quantityPath];
            if (quantity != null)
               _populationSimulation.OutputSelections.AddOutput(new QuantitySelection(quantityPath, quantity.QuantityType));
         }

         //set results
         _populationSimulation.Results = _simulationResults;

         //last but not least, create pk analyses results.
         var populationPKAnalysesTask = context.Resolve<IPKAnalysesTask>();
         _populationSimulation.PKAnalyses = populationPKAnalysesTask.CalculateFor(_populationSimulation);

         context.UpdateBuildinBlockProperties(this, _populationSimulation);
         context.PublishEvent(new SimulationResultsUpdatedEvent(_populationSimulation));
      }

      protected override void ClearReferences()
      {
         _populationSimulation = null;
         _simulationResults = null;
      }
   }
}