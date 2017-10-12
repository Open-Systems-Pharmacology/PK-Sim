using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface ITransportContainerUpdater
   {
      /// <summary>
      /// Sets the default propertiess for a concrete transporter location <paramref name="transporterContainer"/> (i.e. Liver, Kidney) based on the default settings available in the database 
      /// for the <paramref name="membraneLocation"/> and the <paramref name="transportType"/>
      /// </summary>
      void UpdateTransporterFromTemplate(TransporterExpressionContainer transporterContainer, string species, MembraneLocation membraneLocation, TransportType transportType);
     
      
      /// <summary>
      /// Update the default transporter settings using the template defined in the database based on the <paramref name="transporterName"/>  and <paramref name="species"/>
      /// </summary>
      void SetDefaultSettingsForTransporter(IndividualTransporter transporter, string species, string transporterName);

      /// <summary>
      /// Retrieves the membrane location that should be use when updating the transport type to <paramref name="newTransportType"/>
      /// </summary>
      /// <param name="transporterContainer">Concrete transporter location (i.e. Liver, Kidney) that will be updated</param>
      /// <param name="newTransportType">New transport type that was set in transporter</param>
      MembraneLocation MembraneLocationToUse(TransporterExpressionContainer transporterContainer, TransportType newTransportType);
   }

   public class TransportContainerUpdater : ITransportContainerUpdater
   {
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private readonly IEventPublisher _eventPublisher;

      public TransportContainerUpdater(ITransporterContainerTemplateRepository transporterContainerTemplateRepository,IEventPublisher eventPublisher)
      {
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
         _eventPublisher = eventPublisher;
      }

      public void UpdateTransporterFromTemplate(TransporterExpressionContainer transporterContainer, string species, MembraneLocation membraneLocation, TransportType transportType)
      {
         //we need to retrieve the process name for the given MembraneTupe/Process Type combo
         var templateToUse = _transporterContainerTemplateRepository.TransportersFor(species, transporterContainer.Name)
            .Where(x => x.MembraneLocation == membraneLocation)
            .FirstOrDefault(x => x.TransportType == transportType); 

         //That should never happen, otherwise we would have a transporter container with an unknown process
         if (templateToUse == null)
            throw new PKSimException(PKSimConstants.Error.CouldNotFindTransporterFor(transporterContainer.Name, membraneLocation.ToString(), transportType.ToString()));

         transporterContainer.UpdatePropertiesFrom(templateToUse);
      }

      public void SetDefaultSettingsForTransporter(IndividualTransporter transporter, string species, string transporterName)
      {
         transporter.TransportType = _transporterContainerTemplateRepository.TransportTypeFor(species, transporterName);

         foreach (var transporterContainer in transporter.AllExpressionsContainers())
         {
            //there is a db template
            var tranporterTemplate = _transporterContainerTemplateRepository.TransportersFor(species, transporterContainer.Name, transporterName).FirstOrDefault();
            if (tranporterTemplate != null)
               transporterContainer.UpdatePropertiesFrom(tranporterTemplate);
            else
               UpdateTransporterFromTemplate(transporterContainer, species, MembraneLocationToUse(transporterContainer, transporter.TransportType), transporter.TransportType);
         }

         if (_transporterContainerTemplateRepository.HasTransporterTemplateFor(species, transporterName))
            return;

         //No template was found for the given name. Raise event warning
         _eventPublisher.PublishEvent(new NoTranporterTemplateAvailableEvent(transporter));

      }

      public MembraneLocation MembraneLocationToUse(TransporterExpressionContainer transporterContainer, TransportType newTransportType)
      {
         if (!transporterContainer.HasPolarizedMembrane)
            return transporterContainer.MembraneLocation;

         //Brain always plasma
         if (transporterContainer.IsNamed(CoreConstants.Organ.Brain))
            return MembraneLocation.BloodBrainBarrier;

         //intestine always apical
         if (transporterContainer.GroupName == CoreConstants.Groups.GI_MUCOSA)
            return MembraneLocation.Apical;

         //pgp and efflux always apical
         if (newTransportType == TransportType.PgpLike || newTransportType == TransportType.Efflux)
            return MembraneLocation.Apical;

         //not brain and intestine=>Influx is always basolateral
         if (newTransportType == TransportType.Influx)
            return MembraneLocation.Basolateral;

         return transporterContainer.MembraneLocation;
      }
   }
}