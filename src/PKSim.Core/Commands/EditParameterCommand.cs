using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public abstract class EditParameterCommand : BuildingBlockChangeCommand
   {
      protected IParameter _parameter;
      public string ParameterId { get; private set; }
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
         context.UpdateBuildinBlockProperties(this, parentBuildingBlock);
      }

      protected abstract void UpdateParameter(IParameter parameter, IExecutionContext context);

      protected IParameter OriginParameterFor(IParameter parameter, IExecutionContext context)
      {
         if (!IsBuildingBlockParameterInSimulation(parameter))
            return null;

         return context.Get<IParameter>(parameter.Origin.ParameterId);
      }

      protected bool IsBuildingBlockParameterInSimulation(IParameter parameter)
      {
         //a parameter defined in a simulation? Not a building block parameter
         if (parameter.BuildingBlockType == PKSimBuildingBlockType.Simulation) return false;

         //Not a building simulation parameter but origin id is not set. This is most likely an imported simulation
         if (string.IsNullOrEmpty(parameter.Origin.ParameterId))
            return false;

         //simulation Id was not set=>it is a template parameter. otherwise a sim parameter building block
         return !string.IsNullOrEmpty(SimulationId);
      }

      private void setBuildingBlockAlteredFlag(IExecutionContext context)
      {
         //altered flag is only relevant for parameter changed in simulation, that are not simulation parameters
         if (!IsBuildingBlockParameterInSimulation(_parameter)) return;

         var simulation = context.Get<Simulation>(SimulationId);

         //retrieve the former flag 
         bool altered = simulation.GetAltered(_parameter.Origin.BuilingBlockId);

         //this is a direct command
         if (AlteredOn == false)
         {
            //this is the command where altered was switch from false to true => alteredOn = true
            if (altered == false)
               AlteredOn = true;

            simulation.SetAltered(_parameter.Origin.BuilingBlockId, true);
         }
         //this is an inverse command
         else
         {
            simulation.SetAltered(_parameter.Origin.BuilingBlockId, false);
            AlteredOn = false;
         }
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

      public override void UpdateInternalFrom(IBuildingBlockChangeCommand buildingBlockChangeCommand)
      {
         base.UpdateInternalFrom(buildingBlockChangeCommand);
         var editChangeCommand = buildingBlockChangeCommand.DowncastTo<EditParameterCommand>();
         AlteredOn = editChangeCommand.AlteredOn;
      }
   }
}