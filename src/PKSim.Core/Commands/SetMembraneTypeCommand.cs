using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetMembraneTypeCommand : BuildingBlockStructureChangeCommand
   {
      private TransporterExpressionContainer _transporterContainer;
      private readonly MembraneLocation _newMembraneLocation;
      private readonly TransportType _transportType;
      private readonly MembraneLocation _oldMembraneLocation;
      private Individual _individual;
      private readonly string _transporterContainerId;

      public SetMembraneTypeCommand(TransporterExpressionContainer transporterContainer, TransportType transportType, MembraneLocation newMembraneLocation, IExecutionContext context)
      {
         _transporterContainer = transporterContainer;
         _transportType = transportType;
         _transporterContainerId = _transporterContainer.Id;
         _newMembraneLocation = newMembraneLocation;
         _individual = context.BuildingBlockContaining(_transporterContainer).DowncastTo<Individual>();
         BuildingBlockId = _individual.Id;
         _oldMembraneLocation = _transporterContainer.MembraneLocation;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildinBlockProperties(this, _individual);
      }

      protected override void ClearReferences()
      {
         _transporterContainer = null;
         _individual = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetMembraneTypeCommand(_transporterContainer, _transportType, _oldMembraneLocation, context).AsInverseFor(this);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var transportContainerUpdater = context.Resolve<ITransportContainerUpdater>();

         Description = PKSimConstants.Command.SetMembraneTypeCommandDescription(_transporterContainer.ParentContainer.Name,
                                                                                _transporterContainer.Name, _transporterContainer.MembraneLocation.ToString(), _newMembraneLocation.ToString());


         transportContainerUpdater.UpdateTransporterFromTemplate(_transporterContainer, _individual.Species.Name, _newMembraneLocation, _transportType);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _individual = context.Get<Individual>(BuildingBlockId);
         _transporterContainer = context.Get<TransporterExpressionContainer>(_transporterContainerId);
      }
   }
}