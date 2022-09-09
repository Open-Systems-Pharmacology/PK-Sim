using System;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Services;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Services
{
   public interface ITransportContainerUpdater
   {
      /// <summary>
      ///    Update the default transporter settings using the template defined in the database based on the
      ///    <paramref name="transporterName" />/>
      /// </summary>
      void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, string transporterName);

      void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, TransportType transportType);
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
         var transportType = _transporterContainerTemplateRepository.TransportTypeFor(speciesName, transporterName);
         transporter.TransportType = transportType;

         foreach (var transporterContainer in simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter))
         {
            //there is a db template
            var transporterTemplate = _transporterContainerTemplateRepository
               .TransportersFor(speciesName, transporterContainer.LogicalContainerName, transporterName)
               .FirstOrDefault();

            updateTransporterContainerFromTemplate(transporterContainer, transporterTemplate, transportType);
         }

         if (_transporterContainerTemplateRepository.HasTransporterTemplateFor(speciesName, transporterName))
            return;

         //No template was found for the given name. Raise event warning
         _eventPublisher.PublishEvent(new NoTransporterTemplateAvailableEvent(transporter));
      }

      public void SetDefaultSettingsForTransporter(ISimulationSubject simulationSubject, IndividualTransporter transporter, TransportType transportType)
      {
         var allTransporterContainers = simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter);
         allTransporterContainers.Each(transporterContainer => updateTransporterContainerForTransportType(transporterContainer, transportType));
      }

      private void updateFractionExpressedApical(TransporterExpressionContainer transporterContainer, TransportType transportType)
      {
         if (!transportType.IsOneOf(TransportType.Efflux, TransportType.Influx, TransportType.PgpLike))
            return;

         var fractionExpressedApical = transporterContainer.Parameter(FRACTION_EXPRESSED_APICAL);
         if (fractionExpressedApical == null)
            return;

         //Parameter is a hidden parameter. This is used for consistency purpose only and should not be updated
         if (!fractionExpressedApical.Visible)
            return;

         //value was set by the user.
         if (fractionExpressedApical.Value != 0 && fractionExpressedApical.Value != 1)
            return;

         //Set the value according to the new transport type (Efflux + pgp one, Influx 0) except in mucosa where it is always apical
         fractionExpressedApical.Value = transporterContainer.IsInMucosa() ? 1 : transportType == TransportType.Influx ? 0 : 1;
      }

      private void updateTransporterContainerFromTemplate(TransporterExpressionContainer transporterContainer, TransporterContainerTemplate transporterContainerTemplate, TransportType transportType)
      {
         var transporterTypeToUse = transporterContainerTemplate?.TransportType ?? transportType;
         //default update based on transport type
         updateTransporterContainerForTransportType(transporterContainer, transporterTypeToUse);

         //now overwrite if a template container is defined with value specific based on membrane location
         if (transporterContainerTemplate == null)
            return;


         void setFractionValue(string fractionExpressedParameterName, double value)
         {
            var fractionExpressedParameter = transporterContainer.Parameter(fractionExpressedParameterName);
            if (fractionExpressedParameter == null)
               return;
            ;
            fractionExpressedParameter.Value = value;
         }

         switch (transporterContainerTemplate.MembraneLocation)
         {
            case MembraneLocation.Apical:
               setFractionValue(FRACTION_EXPRESSED_APICAL, 1);
               break;
            case MembraneLocation.Basolateral:
               setFractionValue(FRACTION_EXPRESSED_APICAL, 0);
               break;
            case MembraneLocation.BloodBrainBarrier:
               setFractionValue(FRACTION_EXPRESSED_AT_BLOOD_BRAIN_BARRIER, 1);
               break;
            case MembraneLocation.Tissue:
               setFractionValue(FRACTION_EXPRESSED_AT_BLOOD_BRAIN_BARRIER, 0);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      private void updateTransporterContainerForTransportType(TransporterExpressionContainer transporterContainer, TransportType transportType)
      {
         transporterContainer.TransportDirection = TransportDirections.DefaultDirectionFor(transportType, transporterContainer);
         updateFractionExpressedApical(transporterContainer, transportType);
      }
   }
}