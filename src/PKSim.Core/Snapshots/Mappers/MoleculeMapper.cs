using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ModelIndividual = PKSim.Core.Model.Individual;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeMapper : ParameterContainerSnapshotMapperBase<IndividualMolecule, Molecule, ModelIndividual>
   {
      private readonly ExpressionContainerMapper _expressionContainerMapper;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IExecutionContext _executionContext;
      private readonly IOntogenyTask<ModelIndividual> _ontogenyTask;
      private readonly IMoleculeParameterTask _moleculeExpressionTask;
      private readonly OntogenyMapper _ontogenyMapper;

      public MoleculeMapper(
         ParameterMapper parameterMapper,
         ExpressionContainerMapper expressionContainerMapper,
         OntogenyMapper ontogenyMapper,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IExecutionContext executionContext,
         IOntogenyTask<ModelIndividual> ontogenyTask,
         IMoleculeParameterTask moleculeExpressionTask
      ) : base(parameterMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _executionContext = executionContext;
         _ontogenyTask = ontogenyTask;
         _moleculeExpressionTask = moleculeExpressionTask;
         _ontogenyMapper = ontogenyMapper;
      }

      public override async Task<Molecule> MapToSnapshot(IndividualMolecule molecule)
      {
         var snapshot = await SnapshotFrom(molecule, x =>
         {
            updateMoleculeSpecificPropertiesToSnapshot(x, molecule);
            x.Type = molecule.MoleculeType;
         });

         snapshot.Expression = await expressionFor(molecule);
         snapshot.Ontogeny = await _ontogenyMapper.MapToSnapshot(molecule.Ontogeny);
         return snapshot;
      }

      protected override bool ShouldExportParameterToSnapshot(IParameter parameter)
      {
         //For a protein, we export all global parameters to ensure that they do not get out of sync when loading from snapshot 
         var defaultShouldExport = base.ShouldExportParameterToSnapshot(parameter);
         return defaultShouldExport || parameter.IsIndividualMoleculeGlobal();
      }

      private async Task<ExpressionContainer[]> expressionFor(IndividualMolecule molecule)
      {
         var expression = await _expressionContainerMapper.MapToSnapshots(molecule.AllExpressionsContainers());
         return expression?.ToArray();
      }

      private void updateMoleculeSpecificPropertiesToSnapshot(Molecule snapshot, IndividualMolecule molecule)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               snapshot.IntracellularVascularEndoLocation = protein.IntracellularVascularEndoLocation;
               snapshot.TissueLocation = protein.TissueLocation;
               snapshot.MembraneLocation = protein.MembraneLocation;
               break;
            case IndividualTransporter transporter:
               snapshot.TransportType = transporter.TransportType;
               break;
         }
      }

      private void updateMoleculePropertiesToMolecule(IndividualMolecule molecule, Molecule snapshot)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               protein.IntracellularVascularEndoLocation = ModelValueFor(snapshot.IntracellularVascularEndoLocation);
               protein.TissueLocation = ModelValueFor(snapshot.TissueLocation);
               protein.MembraneLocation = ModelValueFor(snapshot.MembraneLocation);
               break;
            case IndividualTransporter transporter:
               transporter.TransportType = ModelValueFor(snapshot.TransportType);
               break;
         }
      }

      public override async Task<IndividualMolecule> MapToModel(Molecule snapshot, ModelIndividual individual)
      {
         var molecule = createMoleculeFrom(snapshot, individual);
         MapSnapshotPropertiesToModel(snapshot, molecule);

         //This call should happen before updating parameters from snapshot to ensure that default molecule 
         //parameters that were updated by the user are taking precedence.
         _moleculeExpressionTask.SetDefaulMoleculeParameters(molecule);

         await UpdateParametersFromSnapshot(snapshot, molecule, snapshot.Type.ToString());

         updateMoleculePropertiesToMolecule(molecule, snapshot);

         var ontogeny = await _ontogenyMapper.MapToModel(snapshot.Ontogeny, individual);
         _ontogenyTask.SetOntogenyForMolecule(molecule, ontogeny, individual);

         await updateExpression(snapshot, new ExpressionContainerMapperContext {Molecule = molecule, SimulationSubject = individual});
         return molecule;
      }

      private async Task updateExpression(Molecule snapshot, ExpressionContainerMapperContext context)
      {
         await _expressionContainerMapper.MapToModels(snapshot.Expression, context);

         //once expression have been set, we need to update normalized parameter
         var normalizeExpressionCommand = new NormalizeRelativeExpressionCommand(context.Molecule, context.SimulationSubject,  _executionContext);
         normalizeExpressionCommand.Execute(_executionContext);
      }

      private IndividualMolecule createMoleculeFrom(Molecule molecule, ISimulationSubject simulationSubject)
      {
         var moleculeFactory = factoryFor(molecule);
         var transporterFactory = moleculeFactory as IIndividualTransporterTask;
         if (transporterFactory == null || molecule.TransportType == null)
            return moleculeFactory.AddMoleculeTo(simulationSubject, molecule.Name);

         return transporterFactory.CreateFor(simulationSubject, molecule.TransportType.Value);
      }

      private IIndividualMoleculeTask factoryFor(Molecule molecule)
      {
         var moleculeType = molecule.Type;
         switch (moleculeType)
         {
            case QuantityType.Enzyme:
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualEnzyme>();
            case QuantityType.OtherProtein:
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualOtherProtein>();
            case QuantityType.Transporter:
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualTransporter>();

            default:
               throw new SnapshotOutdatedException(PKSimConstants.Error.MoleculeTypeNotSupported(moleculeType.ToString()));
         }
      }
   }
}