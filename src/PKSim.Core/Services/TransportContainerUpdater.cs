using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface ITransportContainerUpdater
   {
      /// <summary>
      ///    Update the default transporter settings using the template defined in the database based on the
      ///    <paramref name="transporterName" />  and <paramref name="species" />
      /// </summary>
      void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, string species,
         string transporterName);

      /// <summary>
      ///    Retrieves the transport direction that should be use when updating the transport type to
      ///    <paramref name="newTransportType" />
      /// </summary>
      /// <param name="transporterContainer">Concrete transporter location (i.e. Liver, Kidney) that will be updated</param>
      /// <param name="newTransportType">New transport type that was set in transporter</param>
      TransportDirection TransportDirectionToUse(TransporterExpressionContainer transporterContainer, TransportType newTransportType);

      /// <summary>
      ///    Retrieves the transport direction that should be use when updating the transport type to
      ///    <paramref name="transportType" />
      /// </summary>
      TransportDirection MapTransportTypeTransportDirection(TransportType transportType);

   }

   public class TransportContainerUpdater : ITransportContainerUpdater
   {
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private readonly IEventPublisher _eventPublisher;

      public TransportContainerUpdater(ITransporterContainerTemplateRepository transporterContainerTemplateRepository, IEventPublisher eventPublisher)
      {
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
         _eventPublisher = eventPublisher;
      }

      public void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, string species,
         string transporterName)
      {
         transporter.TransportType = _transporterContainerTemplateRepository.TransportTypeFor(species, transporterName);

         foreach (var transporterContainer in simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter))
         {
            //there is a db template
            var transporterTemplate = _transporterContainerTemplateRepository
               .TransportersFor(species, transporterContainer.ContainerName, transporterName)
               .FirstOrDefault();

            updateTransporterContainerFromTemplate(transporterContainer, transporterTemplate, transporter.TransportType);
         }

         if (_transporterContainerTemplateRepository.HasTransporterTemplateFor(species, transporterName))
            return;

         //No template was found for the given name. Raise event warning
         _eventPublisher.PublishEvent(new NoTranporterTemplateAvailableEvent(transporter));
      }

      private void updateTransporterContainerFromTemplate(TransporterExpressionContainer expressionContainer,
         TransporterContainerTemplate transporterContainerTemplate, TransportType defaultTransportType)
      {
         if (transporterContainerTemplate == null)
         {
            expressionContainer.TransportDirection = TransportDirectionToUse(expressionContainer, defaultTransportType);
            return;
         }

         expressionContainer.TransportDirection = TransportDirectionToUse(expressionContainer, transporterContainerTemplate.TransportType);
      }

      public TransportDirection TransportDirectionToUse(TransporterExpressionContainer transporterContainer, TransportType newTransportType)
      {
         if (!transporterContainer.TransportDirection.CanBeSetGlobally)
            return transporterContainer.TransportDirection;

         return MapTransportTypeTransportDirection(newTransportType);
      }

      public TransportDirection MapTransportTypeTransportDirection(TransportType transportType)
      {
         switch (transportType)
         {
            case TransportType.Influx:
               return TransportDirections.Influx;
            case TransportType.Efflux:
               return TransportDirections.Efflux;
            case TransportType.PgpLike:
               return TransportDirections.PgpLike;
         }

         return TransportDirections.None;
      }
   }
}