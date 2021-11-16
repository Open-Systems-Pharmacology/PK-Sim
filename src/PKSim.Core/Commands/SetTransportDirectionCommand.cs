using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetTransportDirectionCommand : BuildingBlockStructureChangeCommand
   {
      private TransporterExpressionContainer _transporterContainer;
      private readonly TransportDirectionId _newTransportDirection;
      private ExpressionProfile _expressionProfile;
      private readonly string _transporterContainerId;
      private readonly TransportDirectionId _oldTransportDirection;

      public SetTransportDirectionCommand(TransporterExpressionContainer transporterContainer, TransportDirectionId newTransportDirection,
         IExecutionContext context)
      {
         _transporterContainer = transporterContainer;
         _newTransportDirection = newTransportDirection;
         _transporterContainerId = _transporterContainer.Id;
         _expressionProfile = context.BuildingBlockContaining(_transporterContainer).DowncastTo<ExpressionProfile>();
         BuildingBlockId = _expressionProfile.Id;
         _oldTransportDirection = _transporterContainer.TransportDirection;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, _expressionProfile);
      }

      protected override void ClearReferences()
      {
         _transporterContainer = null;
         _expressionProfile = null;
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
         var expressionProfileUpdater = context.Resolve<IExpressionProfileUpdater>();
         expressionProfileUpdater.SynchronizeExpressionProfileInAllSimulationSubjects(_expressionProfile);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _expressionProfile = context.Get<ExpressionProfile>(BuildingBlockId);
         _transporterContainer = context.Get<TransporterExpressionContainer>(_transporterContainerId);
      }
   }
}