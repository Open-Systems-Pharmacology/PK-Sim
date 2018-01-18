using System.Diagnostics;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public abstract class EditParameterCommand : BuildingBlockChangeCommand
   {
      protected IParameter _parameter;
      protected ValueOrigin _oldValueOrigin;
      public string ParameterId { get; }
      public bool AlteredOn { get; protected set; }
      public string SimulationId { get; protected set; }

      protected EditParameterCommand(IParameter parameter)
      {
         _parameter = parameter;
         ParameterId = parameter.Id;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         ObjectType = PKSimConstants.ObjectTypes.Parameter;
         SimulationId = parameter.Origin.SimulationId;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //Retrieve building block id in execute only since parameter might have been added in a macro command and building block id was not available in constructor
         BuildingBlockId = context.BuildingBlockIdContaining(_parameter);

         //Model parameter in simulation might not be found as belonging to a building block, In that case, we set
         //the building block id equal to the simulation id, which is also a building block
         if (string.IsNullOrEmpty(BuildingBlockId))
            BuildingBlockId = SimulationId;

         udpateBuildingBlockProperties(context);

         setBuildingBlockAlteredFlag(context);

         //Command dependent implementation
         ExecuteUpdateParameter(context);

         //Once values have been updated, update dependent objects
         UpdateDependenciesOnParameter(context);
      }

      protected abstract void ExecuteUpdateParameter(IExecutionContext context);

      protected void UpdateDependenciesOnParameter(IExecutionContext context)
      {
         context.UpdateDependenciesOn(_parameter);
      }

      private void udpateBuildingBlockProperties(IExecutionContext context)
      {
         var parentBuildingBlock = context.BuildingBlockContaining(_parameter) ?? context.Get<IPKSimBuildingBlock>(BuildingBlockId);
         context.UpdateBuildinBlockPropertiesInCommand(this, parentBuildingBlock);
      }

      protected abstract void UpdateParameter(IParameter parameter, IExecutionContext context);

      protected IParameter OriginParameterFor(IParameter parameter, IExecutionContext context)
      {
         if (!isBuildingBlockParameterInSimulation(parameter))
            return null;

         return context.Get<IParameter>(parameter.Origin.ParameterId);
      }

      private bool isBuildingBlockParameterInSimulation(IParameter parameter)
      {
         //Building block id is not set => This is the case of parameter defined in a building block
         if (string.IsNullOrEmpty(parameter.Origin.BuilingBlockId))
            return false;

         //THIS SHOULD NOT HAPPEN and should be deleted
         if (string.IsNullOrEmpty(parameter.Origin.SimulationId) || parameter.BuildingBlockType == PKSimBuildingBlockType.Simulation)
         {
            Debug.Print($"This should not happen for parameter {parameter.Name}");
            return false;
         }

         return true;
      }

      private void setBuildingBlockAlteredFlag(IExecutionContext context)
      {
         //altered flag is only relevant for parameter changed in simulation, that are not simulation parameters
         if (!isBuildingBlockParameterInSimulation(_parameter))
            return;

         var simulation = context.Get<Simulation>(SimulationId);

         //retrieve the former flag 
         bool altered = simulation.GetAltered(_parameter.Origin.BuilingBlockId);

         //this is a direct command
         if (AlteredOn == false)
         {
            //this is the command where altered was switch from false to true => alteredOn = true
            if (altered == false)
               AlteredOn = true;

            simulation.SetAltered(_parameter.Origin.BuilingBlockId, altered: true);
         }
         //this is an inverse command
         else
         {
            simulation.SetAltered(_parameter.Origin.BuilingBlockId, altered: false);
            AlteredOn = false;
         }
      }

      protected void SaveValueOriginFor(IParameter parameter)
      {
         _oldValueOrigin = parameter.ValueOrigin.Clone();
      }

      protected virtual void UpdateParameter(IExecutionContext context, bool updateValueOrigin = true)
      {
         var bbParameter = OriginParameterFor(_parameter, context);
         UpdateParameter(_parameter, context);
         UpdateParameter(bbParameter, context);

         if (updateValueOrigin)
            UpdateValueOriginInParameters(_parameter, bbParameter);
      }

      protected virtual void UpdateValueOriginInParameters(IParameter parameter, IParameter buildingBlockParameter)
      {
         //This is an undo command
         if (_oldValueOrigin != null)
         {
            //Simply reset the parameter value origin
            var currentValueOrigin = parameter.ValueOrigin.Clone();
            updateValueOriginIn(parameter, _oldValueOrigin);
            updateValueOriginIn(buildingBlockParameter, _oldValueOrigin);
            _oldValueOrigin = currentValueOrigin;
            return;
         }

         //This is a value being set directly: Only save the ValueOrigin for default parameter
         if (!parameter.ValueOrigin.Default)
            return;

         SaveValueOriginFor(parameter);
         markDefaultValueOriginAsChanged(parameter.ValueOrigin);
      }

      private static bool valueOriginShouldBeUpdatedAutomatically(ValueOrigin valueOrigin)
      {
         return valueOrigin.Source == ValueOriginSources.Undefined &&
                valueOrigin.Method == ValueOriginDeterminationMethods.Undefined;
      }

      private void updateValueOriginIn(IParameter parameter, ValueOrigin valueOrigin)
      {
         if (parameter == null || valueOrigin == null)
            return;

         parameter.ValueOrigin.UpdateFrom(valueOrigin);
      }

      private static void markDefaultValueOriginAsChanged(ValueOrigin valueOrigin)
      {
         valueOrigin.Default = false;
         if (!valueOriginShouldBeUpdatedAutomatically(valueOrigin))
            return;

         valueOrigin.Source = ValueOriginSources.Unknown;
         valueOrigin.Method = ValueOriginDeterminationMethods.Undefined;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         context.Get(SimulationId);
         _parameter = context.Get<IParameter>(ParameterId);
      }

      protected override void ClearReferences()
      {
         _parameter = null;
      }

      public override void UpdateInternalFrom(IBuildingBlockChangeCommand originalCommand)
      {
         base.UpdateInternalFrom(originalCommand);
         var editChangeCommand = originalCommand.DowncastTo<EditParameterCommand>();
         AlteredOn = editChangeCommand.AlteredOn;
         _oldValueOrigin = editChangeCommand._oldValueOrigin;
      }
   }
}