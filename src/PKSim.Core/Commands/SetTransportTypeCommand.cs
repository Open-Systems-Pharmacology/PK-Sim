using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetTransportTypeCommand : BuildingBlockStructureChangeCommand
   {
      private IndividualTransporter _individualTransporter;
      private readonly TransportType _transportType;
      private TransportType _oldTransportType;
      private readonly string _transporterId;
      private ExpressionProfile _expressionProfile;

      public SetTransportTypeCommand(IndividualTransporter individualTransporter, TransportType transportType, IExecutionContext context)
      {
         _expressionProfile = context.BuildingBlockContaining(individualTransporter).DowncastTo<ExpressionProfile>();
         _individualTransporter = individualTransporter;
         _transportType = transportType;
         BuildingBlockId = _expressionProfile.Id;
         _transporterId = _individualTransporter.Id;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, _expressionProfile);
      }

      protected override void ClearReferences()
      {
         _individualTransporter = null;
         _expressionProfile = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetTransportTypeCommand(_individualTransporter, _oldTransportType, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldTransportType = _individualTransporter.TransportType;
         _individualTransporter.TransportType = _transportType;
         var transportContainerUpdater = context.Resolve<ITransportContainerUpdater>();
         transportContainerUpdater.SetDefaultSettingsForTransporter(_expressionProfile.Individual, _individualTransporter, _transportType);
         Description = PKSimConstants.Command.SetTransportTypeCommandDescription(_individualTransporter.Name, _oldTransportType.ToString(), _transportType.ToString());
         var expressionProfileUpdater = context.Resolve<IExpressionProfileUpdater>();
         expressionProfileUpdater.SynchronizeExpressionProfileInAllIndividuals(_expressionProfile);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _individualTransporter = context.Get<IndividualTransporter>(_transporterId);
         _expressionProfile = context.Get<ExpressionProfile>(BuildingBlockId);
      }
   }
}