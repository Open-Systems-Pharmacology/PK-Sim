using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetTransportDirectionCommand : BuildingBlockStructureChangeCommand
   {
      private TransporterExpressionContainer _transporterContainer;
      private readonly TransportDirection _newTransportDirection;
      private Individual _individual;
      private readonly string _transporterContainerId;
      private readonly TransportDirection _oldTransportDirection;

      public SetTransportDirectionCommand(TransporterExpressionContainer transporterContainer, TransportDirection newTransportDirection,
         IExecutionContext context)
      {
         _transporterContainer = transporterContainer;
         _newTransportDirection = newTransportDirection;
         _transporterContainerId = _transporterContainer.Id;
         _individual = context.BuildingBlockContaining(_transporterContainer).DowncastTo<Individual>();
         BuildingBlockId = _individual.Id;
         _oldTransportDirection = _transporterContainer.TransportDirection;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, _individual);
      }

      protected override void ClearReferences()
      {
         _transporterContainer = null;
         _individual = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetTransportDirectionCommand(_transporterContainer, _oldTransportDirection, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         Description = PKSimConstants.Command.SetTransportDirectionCommandDescription(_transporterContainer.ParentContainer.Name,
            _transporterContainer.Name, _transporterContainer.TransportDirection.ToString(), _newTransportDirection.ToString());


         _transporterContainer.TransportDirection = _newTransportDirection;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _individual = context.Get<Individual>(BuildingBlockId);
         _transporterContainer = context.Get<TransporterExpressionContainer>(_transporterContainerId);
      }
   }
}