using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IIndividualTransporterFactory : IIndividualMoleculeFactory
   {
      IndividualTransporter UndefinedLiverTransporterFor(Individual individual);
   }

   public class IndividualTransporterFactory : IndividualMoleculeFactory<IndividualTransporter, TransporterExpressionContainer>, IIndividualTransporterFactory
   {
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;

      public IndividualTransporterFactory(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver,
         ITransporterContainerTemplateRepository transporterContainerTemplateRepository) : base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver)
      {
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
      }

      public override IndividualMolecule CreateFor(ISimulationSubject simulationSubject)
      {
         var transporter = CreateEmptyMolecule();
         //default transporter type
         transporter.TransportType = TransportType.Efflux;

         AddTissueOrgansExpression(simulationSubject, transporter);
         AddMucosaExpression(simulationSubject, transporter);

         return transporter;
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Transporter;

      public IndividualTransporter UndefinedLiverTransporterFor(Individual individual)
      {
         var transporter = CreateEmptyMolecule().WithName(CoreConstants.Molecule.UndefinedLiverTransporter);
         transporter.TransportType = TransportType.Efflux;

         CoreConstants.Compartment.LiverZones.Each(z => addLiverZoneExpression(individual, transporter, z));

         return transporter;
      }

      private void addLiverZoneExpression(Individual individual, IndividualTransporter transporter, string zoneName)
      {
         var liver = individual.Organism.Organ(CoreConstants.Organ.Liver);
         var zone = liver.Compartment(zoneName);

         var transportToBile = _transporterContainerTemplateRepository.TransportersFor(individual.Species.Name, zone.Name)
            .Where(x => x.MembraneLocation == MembraneLocation.Apical)
            .First(x => x.TransportType == TransportType.Efflux);

         var transporterContainer = addTransporterExpressionForContainer(individual, transporter, zone, CoreConstants.Groups.ORGANS_AND_TISSUES, transportToBile);

         transporterContainer.ClearProcessNames();

         transporterContainer.AddProcessName(CoreConstants.Process.BILIARY_CLEARANCE_TO_GALL_BLADDER);
         transporterContainer.AddProcessName(CoreConstants.Process.BILIARY_CLEARANCE_TO_DUODENUM);
         transporterContainer.RelativeExpression = 100;
         transporterContainer.RelativeExpressionNorm = 1;
      }

      protected override TransporterExpressionContainer AddContainerExpression(ISimulationSubject simulationSubject, IndividualTransporter transporter, IContainer container, string groupeName)
      {
         var availableTemplates = _transporterContainerTemplateRepository.TransportersFor(simulationSubject.Species.Name, container.Name)
            .Where(x => x.TransportType == transporter.TransportType).ToList();

         if (!availableTemplates.Any())
            return null;

         return addTransporterExpressionForContainer(simulationSubject, transporter, container, groupeName, availableTemplates.ElementAt(0));
      }

      private TransporterExpressionContainer addTransporterExpressionForContainer(ISimulationSubject simulationSubject, IndividualTransporter transporter, IContainer container, string groupeName, TransporterContainerTemplate transportTemplate)
      {
         var containerExpression = base.AddContainerExpression(simulationSubject, transporter, container, groupeName);
         containerExpression.UpdatePropertiesFrom(transportTemplate);
         return containerExpression;
      }
   }
}