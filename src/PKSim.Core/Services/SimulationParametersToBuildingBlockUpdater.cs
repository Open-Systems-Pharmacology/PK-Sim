using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationParametersToBuildingBlockUpdater
   {
      /// <summary>
      ///    Updates the parameter values intop the building block template from the one defined in the simultion
      /// </summary>
      /// <param name="simulation">Simulation whose parameter will be used as source</param>
      /// <param name="templateBuildingBlock">Template building block that will be updated</param>
      ICommand UpdateParametersFromSimulationInBuildingBlock(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock);
   }

   public class SimulationParametersToBuildingBlockUpdater : ISimulationParametersToBuildingBlockUpdater
   {
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IExecutionContext _executionContext;
      private readonly IContainerTask _containerTask;

      public SimulationParametersToBuildingBlockUpdater(IContainerTask containerTask, IParameterSetUpdater parameterSetUpdater, IExecutionContext executionContext)
      {
         _containerTask = containerTask;
         _parameterSetUpdater = parameterSetUpdater;
         _executionContext = executionContext;
      }

      public ICommand UpdateParametersFromSimulationInBuildingBlock(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock)
      {
         //Update the building block in the simulation based on the same template
         var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(templateBuildingBlock.Id);
         //Template was not used in the simulation...return
         if (usedBuildingBlock == null) return null;

         var buildingBlockType = _executionContext.TypeFor(templateBuildingBlock);
         var templateParameters = parametersToUpdateFrom(templateBuildingBlock);
         var usedBuildingBlockParameters = parametersToUpdateFrom(usedBuildingBlock.BuildingBlock);
         var updateCommands = new PKSimMacroCommand();

         //First Update the parameters in the template building block (the parameter in the used building block are synchronized with the one used in the simulation)
         var updateTemplateParametersCommand = updateParameterValues(usedBuildingBlockParameters, templateParameters);
         updateTemplateParametersCommand.Description = PKSimConstants.Command.UpdateTemplateParameterCommandDescription(templateBuildingBlock.Name, buildingBlockType, simulation.Name);
         _executionContext.UpdateBuildingBlockPropertiesInCommand(updateTemplateParametersCommand, templateBuildingBlock);

         updateCommands.Add(updateTemplateParametersCommand);

         //now make sure that the used building block is updated with the template building block info
         updateCommands.Add(new UpdateUsedBuildingBlockInfoCommand(simulation, usedBuildingBlock, templateBuildingBlock, _executionContext).Run(_executionContext));

         updateCommands.ObjectType = buildingBlockType;
         updateCommands.BuildingBlockType = buildingBlockType;
         updateCommands.BuildingBlockName = templateBuildingBlock.Name;
         updateCommands.CommandType = PKSimConstants.Command.CommandTypeUpdate;
         updateCommands.Description = PKSimConstants.Command.UpdateTemplateBuildingBlockCommandDescription(buildingBlockType, templateBuildingBlock.Name, simulation.Name);
         _executionContext.UpdateBuildingBlockPropertiesInCommand(updateCommands, templateBuildingBlock);

         _executionContext.PublishEvent(new BuildingBlockUpdatedEvent(templateBuildingBlock));
         return updateCommands;
      }

      private IOSPSuiteCommand updateParameterValues(PathCache<IParameter> simulationParameters, PathCache<IParameter> templateParameters)
      {
         //we do not want to update parameter origin id here since we are updateing building block parameters
         return _parameterSetUpdater.UpdateValues(simulationParameters, templateParameters, updateParameterOriginId: false);
      }

      private PathCache<IParameter> parametersToUpdateFrom(IPKSimBuildingBlock buildingBlock) => _containerTask.CacheAllChildren<IParameter>(buildingBlock);
   }
}