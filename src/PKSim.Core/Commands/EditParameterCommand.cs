using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public abstract class EditParameterCommand : BuildingBlockChangeCommand
   {
      protected IParameter _parameter;

      public string ParameterId { get; }
      private bool _alteredOn;

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

         updateBuildingBlockProperties(context);

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

      private void updateBuildingBlockProperties(IExecutionContext context)
      {
         var parentBuildingBlock = context.BuildingBlockContaining(_parameter) ?? context.Get<IPKSimBuildingBlock>(BuildingBlockId);
         context.UpdateBuildingBlockPropertiesInCommand(this, parentBuildingBlock);
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

         return true;
      }

      private void setBuildingBlockAlteredFlag(IExecutionContext context)
      {
         //altered flag is only relevant for parameter changed in simulation, that are not simulation parameters
         if (!isBuildingBlockParameterInSimulation(_parameter))
            return;

         var simulation = context.Get<Simulation>(SimulationId);

         //This can happen when updating simulation parameters during ad-hoc simulation creating that are not registered in context (for example for DDI Ratio simulation)
         if (simulation == null)
            return;

         //retrieve the former flag 
         bool altered = simulation.GetAltered(_parameter.Origin.BuilingBlockId);

         //this is a direct command
         if (_alteredOn == false)
         {
            //this is the command where altered was switch from false to true => alteredOn = true
            if (altered == false)
               _alteredOn = true;

            simulation.SetAltered(_parameter.Origin.BuilingBlockId, altered: true);
         }
         //this is an inverse command
         else
         {
            simulation.SetAltered(_parameter.Origin.BuilingBlockId, altered: false);
            _alteredOn = false;
         }
      }

      protected virtual void UpdateParameter(IExecutionContext context)
      {
         var bbParameter = OriginParameterFor(_parameter, context);
         UpdateParameter(_parameter, context);
         UpdateParameter(bbParameter, context);
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
         _alteredOn = editChangeCommand._alteredOn;
      }
   }
}