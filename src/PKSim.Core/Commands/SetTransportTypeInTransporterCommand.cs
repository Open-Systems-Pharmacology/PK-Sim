using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetTransportTypeInTransporterCommand : BuildingBlockStructureChangeCommand
   {
      private IndividualTransporter _individualTransporter;
      private readonly TransportType _transportType;
      private TransportType _oldTransportType;
      private readonly string _transporterId;

      public SetTransportTypeInTransporterCommand(IndividualTransporter individualTransporter, TransportType transportType, IExecutionContext context)
      {
         var individual = context.BuildingBlockContaining(individualTransporter).DowncastTo<Individual>();
         _individualTransporter = individualTransporter;
         _transportType = transportType;
         BuildingBlockId = individual.Id;
         _transporterId = _individualTransporter.Id;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildingBlockPropertiesInCommand(this, individual);
      }

      protected override void ClearReferences()
      {
         _individualTransporter = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetTransportTypeInTransporterCommand(_individualTransporter, _oldTransportType, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldTransportType = _individualTransporter.TransportType;
         _individualTransporter.TransportType = _transportType;
         Description = PKSimConstants.Command.SetTransportTypeCommandDescription(_individualTransporter.Name, _oldTransportType.ToString(), _transportType.ToString());
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _individualTransporter = context.Get<IndividualTransporter>(_transporterId);
      }
   }
}