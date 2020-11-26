using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Parameters;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualTransporterFactory : IIndividualMoleculeFactory
   {
      IndividualTransporter UndefinedLiverTransporterFor(Individual individual);
      IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType);
   }

   public class IndividualTransporterFactory : IndividualMoleculeFactory<IndividualTransporter, TransporterExpressionContainer>,
      IIndividualTransporterFactory
   {
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      public IndividualTransporterFactory(IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver,
         ITransportContainerUpdater transportContainerUpdater,
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory,
         objectPathFactory, entityPathResolver)
      {
         _transportContainerUpdater = transportContainerUpdater;
         _individualPathWithRootExpander = individualPathWithRootExpander;
      }

      public IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType)
      {
         var transporter = CreateMolecule(moleculeName);
         //default transporter type
         transporter.TransportType = transporterType;

         //default transport direction
         var defaultTransportDirection = _transportContainerUpdater.MapTransportTypeTransportDirection(transporterType);

         addGlobalExpression(transporter, BloodCells, defaultTransportDirection, RelExpParam(REL_EXP_BLOOD_CELLS));

         //Special direction for vascular endothelium that is independent from the default direction choice
         addGlobalExpression(transporter, VascularEndothelium, TransportDirections.PlasmaToInterstitial, RelExpParam(REL_EXP_VASCULAR_ENDOTHELIUM));

         addVascularSystemInitialConcentration(simulationSubject, transporter);
         addTissueOrgansExpression(simulationSubject, transporter);
         addMucosaExpression(simulationSubject, transporter);

         simulationSubject.AddMolecule(transporter);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);

         return transporter;
      }

      private void addGlobalExpression(IndividualTransporter transporter, string globalContainerName, TransportDirection transportDirection,
         params ParameterMetaData[] parameters)
      {
         //Create a global container that we only old transport direction settings
         var transportContainer = AddContainerExpression(transporter, globalContainerName);
         transportContainer.TransportDirection = transportDirection;
         AddGlobalExpression(transporter, parameters);
      }

      public override IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName) =>
         CreateFor(simulationSubject, moleculeName, TransportType.Efflux);

      protected override ApplicationIcon Icon => ApplicationIcons.Transporter;

      public IndividualTransporter UndefinedLiverTransporterFor(Individual individual)
      {
         var transporter = CreateMolecule(CoreConstants.Molecule.UndefinedLiverTransporter);
         transporter.TransportType = TransportType.Efflux;
         LiverZones.Each(z => addLiverZoneExpression(individual, transporter, z));

         return transporter;
      }

      private void addLiverZoneExpression(Individual individual, IndividualTransporter transporter, string zoneName)
      {
         var liver = individual.Organism.Organ(CoreConstants.Organ.Liver);
         var zone = liver.Compartment(zoneName);

         addTissueParameters(zone, transporter);

         var transporterContainer = zone.EntityAt<TransporterExpressionContainer>(Intracellular, transporter.Name);
         transporterContainer.TransportDirection = TransportDirections.Excretion;
         transporterContainer.RelativeExpression = 1;
      }

      private void addVascularSystemInitialConcentration(ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         var organism = simulationSubject.Organism;
         organism.OrgansByType(OrganType.VascularSystem).Each(organ =>
         {
            addContainerExpression(organ.Container(BloodCells), transporter, TransportDirections.None,
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER)
            );
         });
      }

      private void addTissueOrgansExpression(ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         var organism = simulationSubject.Organism;
         organism.NonGITissueContainers.Each(x =>
         {
            //Special case for brain with a different structure to account for blood brain barrier
            if (x.IsBrain())
               addBrainParameters(x, transporter);

            else if (x.IsOrganWithLumen())
               addOrganWithLumenParameters(x, transporter);
            else
               addTissueParameters(x, transporter);
         });

         organism.GITissueContainers.Each(x => addTissueParameters(x, transporter));
      }

      private void addMucosaExpression(ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Compartment(Mucosa);
            organMucosa.GetChildren<Compartment>().Each(x => addOrganWithLumenParameters(x, transporter));
         }
      }

      private void addOrganWithLumenParameters(IContainer organ, IndividualTransporter transporter)
      {
         var defaultTransportDirection = _transportContainerUpdater.MapTransportTypeTransportDirection(transporter.TransportType);

         addTissuePlasmaAndBloodCellsInitialConcentrations(organ, transporter);

         var transportDirection = organ.IsInMucosa() ? defaultTransportDirection : TransportDirections.Excretion;

         addContainerExpression(organ.Container(Intracellular), transporter, transportDirection,
            RelExpParam(REL_EXP),
            FractionParam(FRACTION_EXPRESSED_APICAL, CoreConstants.Rate.ZERO_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR_TRANSPORTER)
         );

         addContainerExpression(organ.Container(Interstitial), transporter, defaultTransportDirection,
            FractionParam(FRACTION_EXPRESSED_BASOLATERAL, CoreConstants.Rate.PARAM_F_EXP_BASOLATERAL, editable: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER)
         );
      }

      private void addBrainParameters(IContainer organ, IndividualTransporter transporter)
      {
         var transportDirection = _transportContainerUpdater.MapTransportTypeTransportDirection(transporter.TransportType);

         addContainerExpression(organ.Container(Plasma), transporter, transportDirection,
            FractionParam(FRACTION_EXPRESSED_AT_BLOOD_BRAIN_BARRIER, CoreConstants.Rate.ONE_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BRAIN_PLASMA_TRANSPORTER)
         );

         addContainerExpression(organ.Container(BloodCells), transporter, TransportDirections.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER)
         );

         addContainerExpression(organ.Container(Interstitial), transporter, transportDirection,
            FractionParam(FRACTION_EXPRESSED_BRAIN_TISSUE, CoreConstants.Rate.PARAM_F_EXP_BRN_TISSUE, editable: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BRAIN_INTERSTITIAL_TRANSPORTER)
         );

         addContainerExpression(organ.Container(Intracellular), transporter, TransportDirections.None,
            RelExpParam(REL_EXP)
         );
      }

      private void addTissueParameters(IContainer organ, IndividualTransporter transporter)
      {
         var defaultTransportDirection = _transportContainerUpdater.MapTransportTypeTransportDirection(transporter.TransportType);

         addTissuePlasmaAndBloodCellsInitialConcentrations(organ, transporter);

         addContainerExpression(organ.Container(Intracellular), transporter,
            defaultTransportDirection,
            RelExpParam(REL_EXP)
         );

         addContainerExpression(organ.Container(Interstitial), transporter,
            TransportDirections.None,
            //We had the basolateral parameter to ensure that we can use the same formula. But this parameter is required only from a technical point of view
            FractionParam(FRACTION_EXPRESSED_BASOLATERAL, CoreConstants.Rate.ONE_RATE, editable: false, visible: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER)
         );
      }

      private void addTissuePlasmaAndBloodCellsInitialConcentrations(IContainer organ, IndividualTransporter transporter)
      {
         addContainerExpression(organ.Container(Plasma), transporter, TransportDirections.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA_TRANSPORTER)
         );

         addContainerExpression(organ.Container(BloodCells), transporter, TransportDirections.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER)
         );
      }

      private TransporterExpressionContainer addContainerExpression(IContainer parentContainer,
         IndividualTransporter transporter, TransportDirection transportDirection, params ParameterMetaData[] parameters)
      {
         var expressionContainer = AddContainerExpression(parentContainer, transporter.Name, parameters);

         //Required to set this first to ensure that we capture container that are purely parameter containers
         expressionContainer.TransportDirection = transportDirection;

         return expressionContainer;
      }
   }
}