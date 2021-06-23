using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Parameters;
using static PKSim.Core.Model.TransportDirections;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualTransporterFactory : IIndividualMoleculeFactory
   {
      IndividualTransporter AddUndefinedLiverTransporterTo(Individual individual);
      IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType);
   }

   public class IndividualTransporterFactory : IndividualMoleculeFactory<IndividualTransporter, TransporterExpressionContainer>,
      IIndividualTransporterFactory
   {
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      public IndividualTransporterFactory(IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander, IIdGenerator idGenerator) : base(objectBaseFactory, parameterFactory,
         objectPathFactory, entityPathResolver, idGenerator)
      {
         _individualPathWithRootExpander = individualPathWithRootExpander;
      }

      public IndividualTransporter CreateFor(ISimulationSubject simulationSubject, string moleculeName, TransportType transporterType)
      {
         var transporter = CreateMolecule(moleculeName);
         //default transporter type
         transporter.TransportType = transporterType;

         //default transport direction
         addGlobalExpression(transporter, BLOOD_CELLS, DefaultBloodCellsDirectionFor(transporterType), RelExpParam(REL_EXP_BLOOD_CELLS));

         //Special direction for vascular endothelium that is independent from the default direction choice
         addGlobalExpression(transporter, VASCULAR_ENDOTHELIUM, DefaultVascularEndotheliumDirectionFor(transporterType),
            RelExpParam(REL_EXP_VASCULAR_ENDOTHELIUM));

         addVascularSystemInitialConcentration(simulationSubject, transporter);
         addTissueOrgansExpression(simulationSubject, transporter);
         addMucosaExpression(simulationSubject, transporter);

         simulationSubject.AddMolecule(transporter);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);

         return transporter;
      }

      private void addGlobalExpression(IndividualTransporter transporter, string globalContainerName, TransportDirectionId transportDirection,
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

      public IndividualTransporter AddUndefinedLiverTransporterTo(Individual individual)
      {
         var transporter = CreateMolecule(CoreConstants.Molecule.UndefinedLiverTransporter);
         transporter.TransportType = TransportType.Efflux;
         var liver = individual.Organism.Organ(CoreConstants.Organ.LIVER);
         LiverZones.Each(zoneName =>
         {
            var zone = liver.Compartment(zoneName);
            addContainerExpression(zone.Container(INTRACELLULAR), transporter, TransportDirectionId.ExcretionLiver,
               RelExpParam(REL_EXP, defaultValue: 1),
               FractionParam(FRACTION_EXPRESSED_APICAL, CoreConstants.Rate.ONE_RATE),
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR_TRANSPORTER)
            );
         });

         _individualPathWithRootExpander.AddRootToPathIn(individual, transporter.Name);
         individual.AddMolecule(transporter);

         return transporter;
      }

      private void addVascularSystemInitialConcentration(ISimulationSubject simulationSubject, IndividualTransporter transporter)
      {
         var organism = simulationSubject.Organism;
         organism.OrgansByType(OrganType.VascularSystem).Each(organ =>
         {
            addContainerExpression(organ.Container(BLOOD_CELLS), transporter, TransportDirectionId.None,
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
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SMALL_INTESTINE, CoreConstants.Organ.LARGE_INTESTINE))
         {
            var organMucosa = organ.Compartment(MUCOSA);
            organMucosa.GetChildren<Compartment>().Each(x => addOrganWithLumenParameters(x, transporter));
         }
      }

      private void addOrganWithLumenParameters(IContainer organ, IndividualTransporter transporter)
      {
         addTissuePlasmaAndBloodCellsInitialConcentrations(organ, transporter);

         var transportDirection = organ.IsInMucosa() ? DefaultMucosaDirectionFor(transporter.TransportType) :
            organ.IsKidney() ? TransportDirectionId.ExcretionKidney : TransportDirectionId.ExcretionLiver;

         addContainerExpression(organ.Container(INTERSTITIAL), transporter, DefaultTissueDirectionFor(transporter.TransportType),
            FractionParam(FRACTION_EXPRESSED_BASOLATERAL, CoreConstants.Rate.PARAM_F_EXP_BASOLATERAL, editable: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER)
         );

         addContainerExpression(organ.Container(INTRACELLULAR), transporter, transportDirection,
            RelExpParam(REL_EXP),
            FractionParam(FRACTION_EXPRESSED_APICAL, transporter.TransportType==TransportType.Influx ? CoreConstants.Rate.ZERO_RATE :   CoreConstants.Rate.ONE_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR_TRANSPORTER)
         );
      }

      private void addBrainParameters(IContainer organ, IndividualTransporter transporter)
      {
         addContainerExpression(organ.Container(PLASMA), transporter, DefaultBloodBrainBarrierDirectionFor(transporter.TransportType),
            FractionParam(FRACTION_EXPRESSED_AT_BLOOD_BRAIN_BARRIER, CoreConstants.Rate.ONE_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BRAIN_PLASMA_TRANSPORTER)
         );

         addContainerExpression(organ.Container(BLOOD_CELLS), transporter, TransportDirectionId.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER)
         );

         addContainerExpression(organ.Container(INTERSTITIAL), transporter, DefaultBrainTissueDirectionFor(transporter.TransportType),
            FractionParam(FRACTION_EXPRESSED_BRAIN_TISSUE, CoreConstants.Rate.PARAM_F_EXP_BRN_TISSUE, editable: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BRAIN_INTERSTITIAL_TRANSPORTER)
         );

         addContainerExpression(organ.Container(INTRACELLULAR), transporter, TransportDirectionId.None,
            RelExpParam(REL_EXP)
         );
      }

      private void addTissueParameters(IContainer organ, IndividualTransporter transporter)
      {
         addTissuePlasmaAndBloodCellsInitialConcentrations(organ, transporter);

         addContainerExpression(organ.Container(INTRACELLULAR), transporter,
            DefaultTissueDirectionFor(transporter.TransportType),
            RelExpParam(REL_EXP)
         );

         addContainerExpression(organ.Container(INTERSTITIAL), transporter,
            TransportDirectionId.None,
            //We had the tissue side parameter to ensure that we can use the same formula. But this parameter is required only from a technical point of view
            FractionParam(FRACTION_EXPRESSED_BASOLATERAL, CoreConstants.Rate.ONE_RATE, editable: false, visible: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER)
         );
      }

      private void addTissuePlasmaAndBloodCellsInitialConcentrations(IContainer organ, IndividualTransporter transporter)
      {
         addContainerExpression(organ.Container(PLASMA), transporter, TransportDirectionId.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA_TRANSPORTER)
         );

         addContainerExpression(organ.Container(BLOOD_CELLS), transporter, TransportDirectionId.None,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER)
         );
      }

      private TransporterExpressionContainer addContainerExpression(IContainer parentContainer,
         IndividualTransporter transporter, TransportDirectionId transportDirection, params ParameterMetaData[] parameters)
      {
         var expressionContainer = AddContainerExpression(parentContainer, transporter.Name, parameters);

         //Required to set this first to ensure that we capture container that are purely parameter containers
         expressionContainer.TransportDirection = transportDirection;

         return expressionContainer;
      }
   }
}