using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetTransportTypeCommand : BuildingBlockStructureChangeCommand
   {
      private TransporterExpressionContainer _transporterContainer;
      private readonly MembraneLocation _membraneLocationToUse;
      private readonly string _transporterContainerId;
      private readonly TransportType _newTransportType;
      private readonly TransportType _oldTransportType;
      private MembraneLocation _oldMembraneLocation;
      private Individual _individual;

      public SetTransportTypeCommand(TransporterExpressionContainer transporterContainer, TransportType oldTransportType, TransportType newTransportType, MembraneLocation membraneLocationToUse, IExecutionContext context)
      {
         _transporterContainer = transporterContainer;
         _membraneLocationToUse = membraneLocationToUse;
         _transporterContainerId = _transporterContainer.Id;
         _individual = context.BuildingBlockContaining(transporterContainer).DowncastTo<Individual>();
         BuildingBlockId = _individual.Id;
         _newTransportType = newTransportType;
         _oldTransportType = oldTransportType;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildinBlockProperties(this, _individual);
         Visible = false;
      }

      protected override void ClearReferences()
      {
         _transporterContainer = null;
         _individual = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetTransportTypeCommand(_transporterContainer, _newTransportType, _oldTransportType, _oldMembraneLocation, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _individual = context.Get<Individual>(BuildingBlockId);
         _transporterContainer = context.Get<TransporterExpressionContainer>(_transporterContainerId);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var transportContainerUpdater = context.Resolve<ITransportContainerUpdater>();
         Description = PKSimConstants.Command.SetTransportTypeCommandDescription(_transporterContainer.Name, _oldTransportType.ToString(), _newTransportType.ToString());
         //Update required membrane location before updating transport type
         _oldMembraneLocation = _transporterContainer.MembraneLocation;
         _transporterContainer.MembraneLocation = _membraneLocationToUse;

         transportContainerUpdater.UpdateTransporterFromTemplate(_transporterContainer, _individual.Species.Name, _transporterContainer.MembraneLocation, _newTransportType);
      }
   }
}