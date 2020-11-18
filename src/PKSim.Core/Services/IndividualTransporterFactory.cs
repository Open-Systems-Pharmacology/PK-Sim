using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Services;
using static PKSim.Core.CoreConstants.Parameters;
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
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      public IndividualTransporterTask(IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver,
         ITransporterContainerTemplateRepository transporterContainerTemplateRepository,
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory,
         objectPathFactory, entityPathResolver)
      {
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
         _individualPathWithRootExpander = individualPathWithRootExpander;
      }

      public IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType)
      {
         var transporter = CreateMolecule(moleculeName);
         //default transporter type
         transporter.TransportType = transporterType;

         AddGlobalExpression(transporter, RelExpParam(REL_EXP_BLOOD_CELLS));
         //TODO
         transporter.TransportDirectionBloodCells = TransportDirection.Influx;

         AddGlobalExpression(transporter, RelExpParam(REL_EXP_VASCULAR_ENDOTHELIUM));
         //TODO
         transporter.TransportDirectionVascularEndothelium = TransportDirection.PlasmaToInterstitial;

         addTissueOrgansExpression(simulationSubject, transporter);
         addMucosaExpression(simulationSubject, transporter);

         simulationSubject.AddMolecule(transporter);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);

         return transporter;
      }

      public override IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName) =>
         CreateFor(simulationSubject, moleculeName, TransportType.Efflux);

      protected override ApplicationIcon Icon => ApplicationIcons.Transporter;

      public IndividualTransporter UndefinedLiverTransporterFor(Individual individual)
      {
         var transporter = CreateMolecule(CoreConstants.Molecule.UndefinedLiverTransporter);
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

         addTissueParameters(zone, individual, transporter);

         var transporterContainer = zone.EntityAt<TransporterExpressionContainer>(CoreConstants.Compartment.Intracellular, transporter.Name);
         transporterContainer.UpdatePropertiesFrom(transportToBile);

         transporterContainer.ClearProcessNames();
         transporterContainer.AddProcessName(CoreConstants.Process.BILIARY_CLEARANCE_TO_GALL_BLADDER);
         transporterContainer.AddProcessName(CoreConstants.Process.BILIARY_CLEARANCE_TO_DUODENUM);
         transporterContainer.RelativeExpression = 1;
      }

      private void addTissueOrgansExpression(ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         var organism = simulationSubject.Organism;
         organism.NonGITissueContainers.Each(x =>
         {
            if (x.IsOrganWithLumen())
               addOrganWithLumenParameters(x, simulationSubject, transporter);
            else
               addTissueParameters(x, simulationSubject, transporter);
         });

         organism.GITissueContainers.Each(x => addTissueParameters(x, simulationSubject, transporter));
      }

      private void addMucosaExpression(ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Compartment(CoreConstants.Compartment.Mucosa);
            organMucosa.GetChildren<Compartment>().Each(x => addOrganWithLumenParameters(x, simulationSubject, transporter));
         }
      }

      private void addOrganWithLumenParameters(IContainer organ, ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         addTissuePlasmaAndBloodCellsInitialConcentrations(organ, simulationSubject, transporter);

         addContainerExpression(organ.Container(CoreConstants.Compartment.Intracellular), simulationSubject, transporter, TransportDirection.Influx,
            RelExpParam(REL_EXP),
            FractionParam(FRACTION_EXPRESSED_APICAL, CoreConstants.Rate.ZERO_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR_TRANSPORTER)
         );

         var (transportDirection, editable) = organ.IsInMucosa() ? (TransportDirection.Influx, true) : (TransportDirection.Elimination, false);
         addContainerExpression(organ.Container(CoreConstants.Compartment.Interstitial), simulationSubject, transporter, transportDirection,
            FractionParam(FRACTION_EXPRESSED_BASOLATERAL, CoreConstants.Rate.PARAM_F_EXP_BASOLATERAL, editable),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER)
         );
      }

      private void addTissueParameters(IContainer organ, ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         addTissuePlasmaAndBloodCellsInitialConcentrations(organ, simulationSubject, transporter);

         addContainerExpression(organ.Container(CoreConstants.Compartment.Intracellular), simulationSubject, transporter,
            TransportDirection.Influx,
            RelExpParam(REL_EXP)
         );

         addContainerExpression(organ.Container(CoreConstants.Compartment.Interstitial), simulationSubject, transporter,
            TransportDirection.None,
            //We had the basolateral parameter to ensure that we can use the same formula. But this parameter is required only from a technical point of view
            FractionParam(FRACTION_EXPRESSED_BASOLATERAL, CoreConstants.Rate.ONE_RATE, editable: false, visible: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER)
         );
      }

      private void addTissuePlasmaAndBloodCellsInitialConcentrations(IContainer organ, ISimulationSubject simulationSubject,
         IndividualTransporter transporter)
      {
         addContainerExpression(organ.Container(CoreConstants.Compartment.Plasma), simulationSubject, transporter, TransportDirection.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA_TRANSPORTER)
         );

         addContainerExpression(organ.Container(CoreConstants.Compartment.BloodCells), simulationSubject, transporter, TransportDirection.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER)
         );
      }

      private TransporterExpressionContainer addContainerExpression(IContainer parentContainer, ISimulationSubject simulationSubject,
         IndividualTransporter transporter, TransportDirection transportDirection, params ParameterMetaData[] parameters)
      {
         var expressionContainer = AddContainerExpression(parentContainer, transporter.Name, parameters);

         expressionContainer.TransportDirection = transportDirection;

         var availableTemplates = _transporterContainerTemplateRepository.TransportersFor(simulationSubject.Species.Name, parentContainer.Name)
            .Where(x => x.TransportType == transporter.TransportType).ToList();

         if (!availableTemplates.Any())
            return null;

         expressionContainer.UpdatePropertiesFrom(availableTemplates.ElementAt(0));
         return expressionContainer;
      }
   }
}