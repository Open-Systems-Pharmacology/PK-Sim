using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Collections;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockParametersToSimulationUpdater
   {
      /// <summary>
      ///    Updates the parameter values into the building block given as parameter in the simulation
      /// </summary>
      /// <param name="templateBuildingBlock">Template building block containing the original values</param>
      /// <param name="simulation">Simulation whose parameter will be updated</param>
      ICommand UpdateParametersFromBuildingBlockInSimulation(IPKSimBuildingBlock templateBuildingBlock, Simulation simulation);
   }

   public class BuildingBlockParametersToSimulationUpdater : IBuildingBlockParametersToSimulationUpdater
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainerTask _containerTask;
      private readonly IParameterSetUpdater _parameterSetUpdater;

      public BuildingBlockParametersToSimulationUpdater(IExecutionContext executionContext, IContainerTask containerTask, IParameterSetUpdater parameterSetUpdater)
      {
         _executionContext = executionContext;
         _containerTask = containerTask;
         _parameterSetUpdater = parameterSetUpdater;
      }

      public ICommand UpdateParametersFromBuildingBlockInSimulation(IPKSimBuildingBlock templateBuildingBlock, Simulation simulation)
      {
         //Update the building block in the simulation based on the same template
         var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(templateBuildingBlock.Id);
         //Template was not used in the simulation...return
         if (usedBuildingBlock == null) return null;

         string buildingBlockType = _executionContext.TypeFor(templateBuildingBlock);
         var templateParameters = parametersToUpdateFrom(templateBuildingBlock);
         var usedBuildingBlockParameters = parametersToUpdateFrom(usedBuildingBlock.BuildingBlock);
         var updateCommands = new PKSimMacroCommand();

         //First Update the parameters in the used building block (internal command, should be visible = false)
         var updateUsedBuildingBlockParameterCommand = updateParameterValues(templateParameters, usedBuildingBlockParameters);
         updateUsedBuildingBlockParameterCommand.Visible = false;
         updateUsedBuildingBlockParameterCommand.Description = PKSimConstants.Command.UpdateUsedBuildingBlockParameterCommandDescription(templateBuildingBlock.Name, buildingBlockType, simulation.Name);
         updateCommands.Add(updateUsedBuildingBlockParameterCommand);

         //then update the values in the simulation from the used building block
         foreach (var command in updateParameterValues(usedBuildingBlockParameters, simulation, usedBuildingBlock.BuildingBlockType))
         {
            updateCommands.Add(command);
         }

         //now make sure that the used building block is updated with the template building block info
         updateCommands.Add(new UpdateUsedBuildingBlockInfoCommand(simulation, usedBuildingBlock, templateBuildingBlock, _executionContext).Run(_executionContext));

         updateCommands.ObjectType = PKSimConstants.ObjectTypes.Simulation;
         updateCommands.CommandType = PKSimConstants.Command.CommandTypeUpdate;
         updateCommands.Description = PKSimConstants.Command.UpdateBuildingBlockCommandDescription(buildingBlockType, templateBuildingBlock.Name, simulation.Name);
         _executionContext.UpdateBuildinBlockPropertiesInCommand(updateCommands, simulation);
         return updateCommands;
      }

      /// <summary>
      ///    Update parameter values from the used building block into the simulationm
      /// </summary>
      /// <param name="usedBuildingBlockParameters">All used building blocks parameters</param>
      /// <param name="simulation">simulation for which parameters should be updated</param>
      /// <param name="buildingBlockType">Type of parameters to update</param>
      private IEnumerable<ICommand> updateParameterValues(IEnumerable<IParameter> usedBuildingBlockParameters, Simulation simulation, PKSimBuildingBlockType buildingBlockType)
      {
         var simulationParameterCache = new Cache<string, List<IParameter>>(onMissingKey: x => new List<IParameter>());

         foreach (var parameter in simulation.ParametersOfType(buildingBlockType))
         {
            var originParameterId = parameter.Origin.ParameterId;
            if (!simulationParameterCache.Contains(originParameterId))
               simulationParameterCache[originParameterId] = new List<IParameter>();

            simulationParameterCache[originParameterId].Add(parameter);
         }

         return from parameter in usedBuildingBlockParameters.OrderBy(x => x.IsDistributed())
                let simParams = simulationParameterCache[parameter.Id]
                  from simParam in simParams
                     select _parameterSetUpdater.UpdateValue(parameter, simParam);
      }

      private ICommand updateParameterValues(PathCache<IParameter> sourceParameters, PathCache<IParameter> targetParameters)
      {
         return _parameterSetUpdater.UpdateValues(sourceParameters, targetParameters);
      }

      private PathCache<IParameter> parametersToUpdateFrom(IPKSimBuildingBlock buildingBlock) => _containerTask.CacheAllChildren<IParameter>(buildingBlock);

   }
}