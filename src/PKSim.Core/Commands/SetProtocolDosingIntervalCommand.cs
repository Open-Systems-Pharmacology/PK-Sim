using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetProtocolDosingIntervalCommand : BuildingBlockStructureChangeCommand
   {
      private readonly DosingInterval _newDosingInterval;
      private DosingInterval _oldDosingInterval;
      private SimpleProtocol _simpleProtocol;

      public SetProtocolDosingIntervalCommand(SimpleProtocol simpleProtocol, DosingInterval newDosingInterval, IExecutionContext context)
      {
         _simpleProtocol = simpleProtocol;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         _newDosingInterval = newDosingInterval;
         BuildingBlockId = simpleProtocol.Id;
         context.UpdateBuildingBlockPropertiesInCommand(this, simpleProtocol);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldDosingInterval = _simpleProtocol.DosingInterval;
         _simpleProtocol.DosingInterval = _newDosingInterval;
         Description = PKSimConstants.Command.SetSimpleProtocolDosingIntervalDescription(_oldDosingInterval.ToString(), _newDosingInterval.ToString());
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _simpleProtocol = context.Get<SimpleProtocol>(BuildingBlockId);
      }

      protected override void ClearReferences()
      {
         _simpleProtocol = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetProtocolDosingIntervalCommand(_simpleProtocol, _oldDosingInterval, context).AsInverseFor(this);
      }
   }
}