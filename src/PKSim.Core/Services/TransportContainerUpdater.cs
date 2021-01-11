using System.Linq;
using OSPSuite.Core.Domain;
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
      ///    <paramref name="transporterName" />/>
      /// </summary>
      void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, string transporterName) ;

      void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, TransportType transportType) ;
   }

   public class TransportContainerUpdater : ITransportContainerUpdater
   {
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private readonly IEventPublisher _eventPublisher;

      public TransportContainerUpdater(
         ITransporterContainerTemplateRepository transporterContainerTemplateRepository,
         IEventPublisher eventPublisher)
      {
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
         _eventPublisher = eventPublisher;
      }

      public void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, string transporterName)
      {
         var speciesName = simulationSubject.Species.Name;
         transporter.TransportType = _transporterContainerTemplateRepository.TransportTypeFor(speciesName, transporterName);

         foreach (var transporterContainer in simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter))
         {
            //there is a db template
            var transporterTemplate = _transporterContainerTemplateRepository
               .TransportersFor(speciesName, transporterContainer.LogicalContainerName, transporterName)
               .FirstOrDefault();

            updateTransporterContainerFromTemplate(transporterContainer, transporterTemplate, transporter.TransportType);
         }

         if (_transporterContainerTemplateRepository.HasTransporterTemplateFor(speciesName, transporterName))
            return;

         //No template was found for the given name. Raise event warning
         _eventPublisher.PublishEvent(new NoTranporterTemplateAvailableEvent(transporter));
      }

      public void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, TransportType transportType)
      {
         foreach (var transporterContainer in simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter))
         {
            updateTransporterContainerFromTemplate(transporterContainer, null, transportType);
         }
      }

      private void updateTransporterContainerFromTemplate(TransporterExpressionContainer expressionContainer,
         TransporterContainerTemplate transporterContainerTemplate, TransportType defaultTransportType)
      {
         var transportType = transporterContainerTemplate?.TransportType ?? defaultTransportType;
         expressionContainer.TransportDirection = TransportDirections.DefaultDirectionFor(transportType, expressionContainer);
      }
   }
}