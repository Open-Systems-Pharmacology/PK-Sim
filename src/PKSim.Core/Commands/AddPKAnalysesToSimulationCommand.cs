using System.Collections.Generic;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    this is not a reversible command!
   /// </summary>
   public class AddPKAnalysesToSimulationCommand : PKSimCommand
   {
      private PopulationSimulation _populationSimulation;
      private IEnumerable<QuantityPKParameter> _pkParameters;

      public AddPKAnalysesToSimulationCommand(PopulationSimulation populationSimulation, IEnumerable<QuantityPKParameter> pkParameters, string fileName)
      {
         _populationSimulation = populationSimulation;
         _pkParameters = pkParameters;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.AddPKAnalysesToSimulationCommandDescription(populationSimulation.Name, fileName);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         foreach (var pkParameter in _pkParameters)
         {
            _populationSimulation.PKAnalyses.AddPKAnalysis(pkParameter);
         }

         context.UpdateBuildingBlockPropertiesInCommand(this, _populationSimulation);
         context.PublishEvent(new SimulationResultsUpdatedEvent(_populationSimulation));
      }

      protected override void ClearReferences()
      {
         _populationSimulation = null;
         _pkParameters = null;
      }
   }
}