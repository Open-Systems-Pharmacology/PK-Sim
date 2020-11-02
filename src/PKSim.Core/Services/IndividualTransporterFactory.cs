using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Services;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualTransporterTask : IIndividualMoleculeTask
   {
      IndividualTransporter UndefinedLiverTransporterFor(Individual individual);
      IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType);
   }

   public class IndividualTransporterTask : IndividualMoleculeTask<IndividualTransporter, TransporterExpressionContainer>, IIndividualTransporterTask
   {
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;

      public IndividualTransporterTask(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver,
         ITransporterContainerTemplateRepository transporterContainerTemplateRepository) : base(objectBaseFactory, parameterFactory,
         objectPathFactory, entityPathResolver)
      {
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
      }

      public override IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName)
      {
         //TODO
         return null;
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Transporter;

      public IndividualTransporter UndefinedLiverTransporterFor(Individual individual)
      {
         var transporter = CreateMolecule(CoreConstants.Molecule.UndefinedLiverTransporter);
         transporter.TransportType = TransportType.Efflux;

         CoreConstants.Compartment.LiverZones.Each(z => addLiverZoneExpression(individual, transporter, z));

         return transporter;
      }

      private void addTissueOrgansExpression(ISimulationSubject simulationSubject, IndividualTransporter molecule)
      {
         foreach (var container in simulationSubject.Organism.NonGITissueContainers)
         {
            AddContainerExpression(simulationSubject, molecule, container, CoreConstants.Groups.ORGANS_AND_TISSUES);
         }

         foreach (var organ in simulationSubject.Organism.GITissueContainers)
         {
            AddContainerExpression(simulationSubject, molecule, organ, CoreConstants.Groups.GI_NON_MUCOSA_TISSUE);
         }
      }

      public IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType)
      {
         var transporter = CreateMolecule(moleculeName);
         //default transporter type
         transporter.TransportType = transporterType;

         addTissueOrgansExpression(simulationSubject, transporter);
         AddMucosaExpression(simulationSubject, transporter);

         return transporter;
      }

      protected void AddMucosaExpression(ISimulationSubject simulationSubject, IndividualTransporter molecule)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Compartment(CoreConstants.Compartment.Mucosa);
            foreach (var compartment in organMucosa.GetChildren<Compartment>().Where(c => c.Visible))
            {
               AddContainerExpression(simulationSubject, molecule, compartment, CoreConstants.Groups.GI_MUCOSA);
            }
         }
      }

      private void addLiverZoneExpression(Individual individual, IndividualTransporter transporter, string zoneName)
      {
         var liver = individual.Organism.Organ(CoreConstants.Organ.Liver);
         var zone = liver.Compartment(zoneName);

         var transportToBile = _transporterContainerTemplateRepository.TransportersFor(individual.Species.Name, zone.Name)
            .Where(x => x.MembraneLocation == MembraneLocation.Apical)
            .First(x => x.TransportType == TransportType.Efflux);

         var transporterContainer =
            addTransporterExpressionForContainer(transporter, zone, CoreConstants.Groups.ORGANS_AND_TISSUES, transportToBile);

         transporterContainer.ClearProcessNames();

         transporterContainer.AddProcessName(CoreConstants.Process.BILIARY_CLEARANCE_TO_GALL_BLADDER);
         transporterContainer.AddProcessName(CoreConstants.Process.BILIARY_CLEARANCE_TO_DUODENUM);
         transporterContainer.RelativeExpression = 1;
      }

      protected TransporterExpressionContainer AddContainerExpression(ISimulationSubject simulationSubject, IndividualTransporter transporter,
         IContainer container, string groupName)
      {
         var availableTemplates = _transporterContainerTemplateRepository.TransportersFor(simulationSubject.Species.Name, container.Name)
            .Where(x => x.TransportType == transporter.TransportType).ToList();

         if (!availableTemplates.Any())
            return null;

         return addTransporterExpressionForContainer(transporter, container, groupName, availableTemplates.ElementAt(0));
      }

      private TransporterExpressionContainer addTransporterExpressionForContainer(
         IndividualTransporter transporter, IContainer container, string groupName, TransporterContainerTemplate transportTemplate)
      {
         var containerExpression = addContainerExpression(transporter, container, container.Name, groupName);
         containerExpression.UpdatePropertiesFrom(transportTemplate);
         return containerExpression;
      }

      private TransporterExpressionContainer addContainerExpression(IndividualTransporter protein, IContainer container, string name,
         string groupingName)
      {
         var expressionContainer = addContainerExpression(protein, name, groupingName);
         return expressionContainer;
      }

      private TransporterExpressionContainer addContainerExpression(IndividualTransporter protein, string containerName, string groupingName)
      {
         var expressionContainer = createContainerExpressionFor(protein, containerName);
         expressionContainer.GroupName = groupingName;
         CreateMoleculeParameterIn(expressionContainer, CoreConstants.Parameters.REL_EXP, 0, Constants.Dimension.DIMENSIONLESS);
         return expressionContainer;
      }

      private TransporterExpressionContainer createContainerExpressionFor(IndividualTransporter protein, string containerName)
      {
         var container = _objectBaseFactory.Create<TransporterExpressionContainer>().WithName(containerName);
         protein.Add(container);
         return container;
      }
   }
}