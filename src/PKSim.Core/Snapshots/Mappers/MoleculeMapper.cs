using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeMapper : ParameterContainerSnapshotMapperBase<IndividualMolecule, Molecule, ISimulationSubject>
   {
      private readonly ExpressionContainerMapper _expressionContainerMapper;
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IExecutionContext _executionContext;
      private readonly OntogenyMapper _ontogenyMapper;

      public MoleculeMapper(
         ParameterMapper parameterMapper,
         ExpressionContainerMapper expressionContainerMapper,
         OntogenyMapper ontogenyMapper,
         IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver,
         IExecutionContext executionContext
      ) : base(parameterMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _executionContext = executionContext;
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

      public override async Task<IndividualMolecule> MapToModel(Molecule snapshot, ISimulationSubject simulationSubject)
      {
         var molecule = createMoleculeFrom(snapshot, simulationSubject);
         await UpdateParametersFromSnapshot(snapshot, molecule, snapshot.Type.ToString());
         MapSnapshotPropertiesToModel(snapshot, molecule);
         updateMoleculePropertiesToMolecule(molecule, snapshot);
         molecule.Ontogeny = await _ontogenyMapper.MapToModel(snapshot.Ontogeny, simulationSubject);
         await updateExpression(snapshot, new ExpressionContainerMapperContext {Molecule = molecule, SimulationSubject = simulationSubject});
         return molecule;
      }

      private async Task updateExpression(Molecule snapshot, ExpressionContainerMapperContext context)
      {
         await _expressionContainerMapper.MapToModels(snapshot.Expression, context);

         //once expression have been set, we need to update normalized parameter
         var normalizeExpressionCommand = new NormalizeRelativeExpressionCommand(context.Molecule, _executionContext);
         normalizeExpressionCommand.Execute(_executionContext);
      }

      private IndividualMolecule createMoleculeFrom(Molecule molecule, ISimulationSubject simulationSubject)
      {
         return factoryFor(molecule).CreateFor(simulationSubject);
      }

      private IIndividualMoleculeFactory factoryFor(Molecule molecule)
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