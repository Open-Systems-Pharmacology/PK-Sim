using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IParameterSetUpdater
   {
      /// <summary>
      ///    Update all parameters defined in the target container with the value of the same parameter in the source container
      ///    if available. Same parameter is defined as "have the same absolute path".
      /// </summary>
      /// <param name="sourceContainer">container from which the value should be taken</param>
      /// <param name="targetContainer">container for which the parameter values should be updated</param>
      ICommand UpdateValues(IContainer sourceContainer, IContainer targetContainer);

      /// <summary>
      ///    Update all parameters defined in the target enumeration with the value of the same parameter in the source
      ///    enumeration
      ///    <paramref name="sourceParameters" />. Same parameter is defined as "have the same absolute path".
      /// </summary>
      ICommand UpdateValues(IEnumerable<IParameter> sourceParameters, IEnumerable<IParameter> targetParameters);

      /// <summary>
      ///    Update all parameters defined in the target container  with the value of the same parameter in the source cache
      ///    <paramref name="sourceParameters" />. Same parameter is defined as "have the same absolute path".
      /// <param name="sourceParameters"> Parameters from which the value should be taken</param>
      ///    <param name="targetContainer">container for which the parameter values should be updated</param>
      /// </summary>
      ICommand UpdateValues(PathCache<IParameter> sourceParameters, IContainer targetContainer);

      /// <summary>
      ///    Update all parameters defined in the target cache with the value of the same parameter in the source cache
      ///    <paramref name="sourceParameters" />. Same parameter is defined as being registered with the same key in the cache.
      /// </summary>
      IOSPSuiteCommand UpdateValues(PathCache<IParameter> sourceParameters, PathCache<IParameter> targetParameters, bool updateParameterOriginId = true);

      /// <summary>
      ///    Update all parameters defined in the target container with the value of the same parameter in the source container
      ///    if available. Same parameter is defined as "have the same name"
      /// </summary>
      /// <param name="sourceContainer">container from which the value should be taken</param>
      /// <param name="targetContainer">container for which the parameter values should be updated</param>
      /// <returns></returns>
      ICommand UpdateValuesByName(IContainer sourceContainer, IContainer targetContainer);

      /// <summary>
      ///    Update all parameters defined in the target enumeration with the value of the same parameter in the source
      ///    enumeration
      ///    <paramref name="sourceParameters" />. Same parameter is defined as "have the same name".
      /// </summary>
      ICommand UpdateValuesByName(IEnumerable<IParameter> sourceParameters, IEnumerable<IParameter> targetParameters);

      /// <summary>
      ///    Update all prameters defined in the target enumeration with the value of the same parameter in the source container
      ///    if available. Same parameter is defined as "have the same name"
      /// </summary>
      /// <param name="sourceContainer">container from which the value should be taken</param>
      /// <param name="targetParameters">Parameter enumeration for which the parameter values should be updated</param>
      /// <returns></returns>
      ICommand UpdateValuesByName(IContainer sourceContainer, IEnumerable<IParameter> targetParameters);

      /// <summary>
      ///    Update the value from the source parameter in the target parameter
      /// </summary>
      /// <param name="sourceParameter">Source parameter containing the value</param>
      /// <param name="targetParameter">Target parameter that will be updated with the value from the source parameter</param>
      /// <param name="updateParameterOriginId">
      ///    Set to true, the <paramref name="targetParameter" /> origin id will be set to the
      ///    id of <paramref name="sourceParameter" />
      /// </param>
      /// <returns></returns>
      ICommand UpdateValue(IParameter sourceParameter, IParameter targetParameter, bool updateParameterOriginId = true);
   }

   public class ParameterSetUpdater : IParameterSetUpdater
   {
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IParameterUpdater _parameterUpdater;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IParameterTask _parameterTask;

      public ParameterSetUpdater(IEntityPathResolver entityPathResolver, IParameterUpdater parameterUpdater, IParameterIdUpdater parameterIdUpdater, IParameterTask parameterTask)
      {
         _entityPathResolver = entityPathResolver;
         _parameterUpdater = parameterUpdater;
         _parameterIdUpdater = parameterIdUpdater;
         _parameterTask = parameterTask;
      }

      public ICommand UpdateValues(IContainer sourceContainer, IContainer targetContainer)
      {
         return UpdateValues(sourceContainer.GetAllChildren<IParameter>(), targetContainer.GetAllChildren<IParameter>());
      }

      public ICommand UpdateValues(IEnumerable<IParameter> sourceParameters, IEnumerable<IParameter> targetParameters)
      {
         var allTargetParameters = new PathCache<IParameter>(_entityPathResolver).For(targetParameters);
         var allSourceParameters = new PathCache<IParameter>(_entityPathResolver).For(sourceParameters);

         return UpdateValues(allSourceParameters, allTargetParameters);
      }

      public ICommand UpdateValues(PathCache<IParameter> sourceParameters, IContainer targetContainer)
      {
         var allTargetParameters = new PathCache<IParameter>(_entityPathResolver).For(targetContainer.GetAllChildren<IParameter>());
         return UpdateValues(sourceParameters, allTargetParameters);
      }

      public IOSPSuiteCommand UpdateValues(PathCache<IParameter> sourceParameters, PathCache<IParameter> targetParameters, bool updateParameterOriginId = true)
      {
         var updateCommands = new PKSimMacroCommand {CommandType = PKSimConstants.Command.CommandTypeEdit, ObjectType = PKSimConstants.ObjectTypes.Parameter};
         //should update non distributed parameter first and then distributed parameter
         foreach (var sourceParameter in sourceParameters.KeyValues.OrderBy(x => x.Value.IsDistributed()))
         {
            var targetParameter = targetParameters[sourceParameter.Key];
            if (targetParameter == null)
               continue;

            updateCommands.Add(UpdateValue(sourceParameter.Value, targetParameter, updateParameterOriginId));
         }

         return updateCommands;
      }

      public ICommand UpdateValuesByName(IEnumerable<IParameter> sourceParameters, IEnumerable<IParameter> targetParameters)
      {
         var updateCommands = new PKSimMacroCommand {CommandType = PKSimConstants.Command.CommandTypeEdit, ObjectType = PKSimConstants.ObjectTypes.Parameter};
         var targetParameterCache = new Cache<string, IParameter>(p => p.Name, key => null);
         targetParameterCache.AddRange(targetParameters);

         foreach (var sourceParameter in sourceParameters)
         {
            var targetParameter = targetParameterCache[sourceParameter.Name];
            if (targetParameter == null) continue;

            updateCommands.Add(UpdateValue(sourceParameter, targetParameter));
         }

         return updateCommands;
      }

      public ICommand UpdateValuesByName(IContainer sourceContainer, IContainer targetContainer)
      {
         return UpdateValuesByName(sourceContainer, targetContainer.GetChildren<IParameter>());
      }

      public ICommand UpdateValuesByName(IContainer sourceContainer, IEnumerable<IParameter> targetParameters)
      {
         return UpdateValuesByName(sourceContainer.GetChildren<IParameter>(), targetParameters);
      }

      public ICommand UpdateValue(IParameter sourceParameter, IParameter targetParameter, bool updateParameterOriginId = true)
      {
         if (updateParameterOriginId)
            _parameterIdUpdater.UpdateParameterId(sourceParameter, targetParameter);

         if (parameterShouldBeExcludedFromBulkUpdate(targetParameter))
            return new PKSimEmptyCommand();

          return withUpdatedValueOrigin(_parameterUpdater.UpdateValue(sourceParameter, targetParameter), sourceParameter, targetParameter);
      }

      private ICommand withUpdatedValueOrigin(ICommand command, IParameter sourceParameter, IParameter targetParameter)
      {
         if (Equals(sourceParameter.ValueOrigin, targetParameter.ValueOrigin))
            return command;

         var updateValueOriginCommand = _parameterTask.SetParameterValueOrigin(targetParameter, sourceParameter.ValueOrigin);
         if (command == null)
            return updateValueOriginCommand;

         var macroCommand = new PKSimMacroCommand {CommandType = command.CommandType, ObjectType = command.ObjectType, Description = command.Description};
         macroCommand.Add(command);
         macroCommand.Add(updateValueOriginCommand);
         return macroCommand;
      }

      private bool parameterShouldBeExcludedFromBulkUpdate(IParameter parameter) => parameter.IsExpressionNorm();
   }
}