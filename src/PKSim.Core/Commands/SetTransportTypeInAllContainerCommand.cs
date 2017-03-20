using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetTransportTypeInAllContainerCommand : PKSimMacroCommand
   {
      private IndividualTransporter _individualTransporter;
      public TransportType NewTransportType { get; private set; }
      public TransportType OldTransportType { get; private set; }
      private Individual _individual;

      public SetTransportTypeInAllContainerCommand(IndividualTransporter individualTransporter, TransportType transportType, IExecutionContext context)
      {
         _individualTransporter = individualTransporter;
         _individual = context.BuildingBlockContaining(individualTransporter).DowncastTo<Individual>();

         NewTransportType = transportType;
         OldTransportType = _individualTransporter.TransportType;
         ObjectType = PKSimConstants.ObjectTypes.Transporter;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SetTransportTypeCommandDescription(_individualTransporter.Name, OldTransportType.ToString(), NewTransportType.ToString());
         context.UpdateBuildinBlockProperties(this, _individual);
      }

      public override void Execute(IExecutionContext context)
      {
         var transportContainerUpdater = context.Resolve<ITransportContainerUpdater>();

         Add(new SetTransportTypeInTransporterCommand(_individualTransporter, NewTransportType, context));

         //Check if a template is available for the given container. If yes, we're good to go and can create a change transporter type command
         //we need to retrieve the process name for the given MembraneTupe/Process Type combo
         foreach (var transporterContainer in _individualTransporter.AllExpressionsContainers())
         {
            var membraneLocationToUse = transportContainerUpdater.MembraneLocationToUse(transporterContainer, NewTransportType);

            //change transport type 
            Add(new SetTransportTypeCommand(transporterContainer, OldTransportType, NewTransportType, membraneLocationToUse, context));
         }

         //now execute all commands
         base.Execute(context);

         //clear references
         _individualTransporter = null;
         _individual = null;
      }
   }
}