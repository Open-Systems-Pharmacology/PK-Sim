using OSPSuite.Core.Commands;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class UpdateParameterValueOriginCommand : PKSimMacroCommand
   {
      private IParameter _parameter;
      private ValueOrigin _valueOrigin;

      public UpdateParameterValueOriginCommand(IParameter parameter, ValueOrigin valueOrigin)
      {
         _parameter = parameter;
         _valueOrigin = valueOrigin;
      }

      public override void Execute(IExecutionContext context)
      {
         var updateValueOriginCommand = new UpdateValueOriginCommand(_valueOrigin, _parameter, context);
         Add(updateValueOriginCommand);

         var originParameter = context.Get<IParameter>(_parameter.Origin.ParameterId);
         if (originParameter != null)
            Add(new UpdateValueOriginCommand(_valueOrigin, originParameter, context) {Visible = false});

         var parentBuildingBlock = context.BuildingBlockContaining(_parameter) ?? context.Get<IPKSimBuildingBlock>(_parameter.Origin.SimulationId);
         context.UpdateBuildinBlockPropertiesInCommand(this, parentBuildingBlock);

         //update building block properties in sub commands
         All().Each(x => x.UpdatePropertiesFrom(this));

         //now execute all commands
         base.Execute(context);

         //Update properties after execute so that all properties are set 
         ObjectType = updateValueOriginCommand.ObjectType;
         CommandType = updateValueOriginCommand.CommandType;
         Description = updateValueOriginCommand.Description;

         //clear references
         _parameter = null;
         _valueOrigin = null;
      }
   }
}