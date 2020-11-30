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
      private Individual _individual;

      public SetTransportTypeCommand(IndividualTransporter individualTransporter, TransportType transportType, IExecutionContext context)
      {
         var individual = context.BuildingBlockContaining(individualTransporter).DowncastTo<Individual>();
         _individualTransporter = individualTransporter;
         _transportType = transportType;
         BuildingBlockId = individual.Id;
         _individual = individual;
         _transporterId = _individualTransporter.Id;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, individual);
      }

      protected override void ClearReferences()
      {
         _individualTransporter = null;
         _individual = null;
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
         transportContainerUpdater.SetDefaultSettingsForTransporter(_individual, _individualTransporter, _transportType);
         Description = PKSimConstants.Command.SetTransportTypeCommandDescription(_individualTransporter.Name, _oldTransportType.ToString(), _transportType.ToString());
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _individualTransporter = context.Get<IndividualTransporter>(_transporterId);
         _individual = context.Get<Individual>(BuildingBlockId);
      }
   }
}