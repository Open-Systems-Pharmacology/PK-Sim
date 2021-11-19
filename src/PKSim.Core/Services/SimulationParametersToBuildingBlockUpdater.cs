using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationParametersToBuildingBlockUpdater
   {
      /// <summary>
      ///    Updates the parameter values into the building block template from the one defined in the simulation
      /// </summary>
      /// <param name="simulation">Simulation whose parameter will be used as source</param>
      /// <param name="templateBuildingBlock">Template building block that will be updated</param>
      ICommand UpdateParametersFromSimulationInBuildingBlock(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock);
   }

   public class SimulationParametersToBuildingBlockUpdater : ISimulationParametersToBuildingBlockUpdater
   {
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IExpressionProfileUpdater _expressionProfileUpdater;
      private readonly IExecutionContext _executionContext;
      private readonly IContainerTask _containerTask;

      public SimulationParametersToBuildingBlockUpdater(
         IContainerTask containerTask,
         IParameterSetUpdater parameterSetUpdater,
         IExpressionProfileUpdater expressionProfileUpdater,
         IExecutionContext executionContext)
      {
         _containerTask = containerTask;
         _parameterSetUpdater = parameterSetUpdater;
         _expressionProfileUpdater = expressionProfileUpdater;
         _executionContext = executionContext;
      }

      public ICommand UpdateParametersFromSimulationInBuildingBlock(Simulation simulation, IPKSimBuildingBlock templateBuildingBlock)
      {
         //Update the building block in the simulation based on the same template
         var usedBuildingBlock = simulation.UsedBuildingBlockByTemplateId(templateBuildingBlock.Id);
         //Template was not used in the simulation...return
         if (usedBuildingBlock == null)
            return null;

         var buildingBlockType = _executionContext.TypeFor(templateBuildingBlock);
         var templateParameters = parametersToUpdateFrom(templateBuildingBlock);
         var usedBuildingBlockParameters = parametersToUpdateFrom(usedBuildingBlock.BuildingBlock);
         var updateCommands = new PKSimMacroCommand
         {
            ObjectType = buildingBlockType,
            BuildingBlockType = buildingBlockType,
            BuildingBlockName = templateBuildingBlock.Name,
            CommandType = PKSimConstants.Command.CommandTypeUpdate,
            Description = PKSimConstants.Command.UpdateTemplateBuildingBlockCommandDescription(buildingBlockType, templateBuildingBlock.Name, simulation.Name)
         };
         _executionContext.UpdateBuildingBlockPropertiesInCommand(updateCommands, templateBuildingBlock);

         //First Update the parameters in the template building block (the parameter in the used building block are synchronized with the one used in the simulation)
         var updateTemplateParametersCommand = updateParameterValues(usedBuildingBlockParameters, templateParameters);
         updateTemplateParametersCommand.Description = PKSimConstants.Command.UpdateTemplateParameterCommandDescription(templateBuildingBlock.Name, buildingBlockType, simulation.Name);
         _executionContext.UpdateBuildingBlockPropertiesInCommand(updateTemplateParametersCommand, templateBuildingBlock);

         updateCommands.Add(updateTemplateParametersCommand);

         //Last, see if we have some special cases to handle 
         var synchronizeCommand = synchronizeBuildingBlocks(templateBuildingBlock, updateTemplateParametersCommand);
         updateCommands.Add(synchronizeCommand);

         //now make sure that the used building block is updated with the template building block info
         updateCommands.Add(new UpdateUsedBuildingBlockInfoCommand(simulation, usedBuildingBlock, templateBuildingBlock, _executionContext).Run(_executionContext));

         _executionContext.PublishEvent(new BuildingBlockUpdatedEvent(templateBuildingBlock));
         return updateCommands;
      }

      private ICommand synchronizeBuildingBlocks(IPKSimBuildingBlock templateBuildingBlock, IPKSimMacroCommand updateTemplateParametersCommand)
      {
         var simulationSubject = templateBuildingBlock as ISimulationSubject;
         //For now, deal with update from Individual or Population into Expression Profile
         if (simulationSubject == null)
            return new PKSimEmptyCommand();

         var allExpressionProfileParameterValueCommand = updateTemplateParametersCommand.All()
            .OfType<SetExpressionProfileValueCommand>()
            .ToList();

         if (!allExpressionProfileParameterValueCommand.Any())
            return new PKSimEmptyCommand();


         var expressionProfilesToUpdate = new HashSet<ExpressionProfile>();
         var macroCommand = new PKSimMacroCommand();
         //We have some commands related to expression profile. We need to update the expression profile
         foreach (var parameterCommand in allExpressionProfileParameterValueCommand)
         {
            var simulationSubjectParameter = _executionContext.Get<IParameter>(parameterCommand.ParameterId);
            if (simulationSubjectParameter == null)
               continue;

            //This should be the id of the parameter in the expression profile
            var expressionProfileParameterId = simulationSubjectParameter.Origin.ParameterId;
            var expressionProfileParameter = _executionContext.Get<IParameter>(expressionProfileParameterId);
            if (expressionProfileParameter == null)
               continue;

            var expressionProfile = _executionContext.BuildingBlockContaining(expressionProfileParameter) as ExpressionProfile;
            if (expressionProfile == null)
               continue;

            expressionProfilesToUpdate.Add(expressionProfile);

            //We do not update the simulation subject. It will be done at the end of the loop
            var command = new SetExpressionProfileValueCommand(expressionProfileParameter, simulationSubjectParameter.Value, updateSimulationSubjects: false);
            macroCommand.Add(command);
         }

         macroCommand.Run(_executionContext);
         _executionContext.UpdateBuildingBlockPropertiesInCommand(macroCommand, templateBuildingBlock);

         //Now that our expression profile are updated, we need to trigger the synchronization in all building blocks
         expressionProfilesToUpdate.Each(x => _expressionProfileUpdater.SynchronizeExpressionProfileInAllSimulationSubjects(x));

         return macroCommand;
      }

      private IPKSimMacroCommand updateParameterValues(PathCache<IParameter> simulationParameters, PathCache<IParameter> templateParameters)
      {
         //we do not want to update parameter origin id here since we are updating building block parameters
         return _parameterSetUpdater.UpdateValues(simulationParameters, templateParameters, updateParameterOriginId: false);
      }

      private PathCache<IParameter> parametersToUpdateFrom(IPKSimBuildingBlock buildingBlock) => _containerTask.CacheAllChildren<IParameter>(buildingBlock);
   }
}