using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using ModelIndividual = PKSim.Core.Model.Individual;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeMapper : ParameterContainerSnapshotMapperBase<IndividualMolecule, Molecule, ModelIndividual, ModelIndividual>
   {
      private readonly ExpressionContainerMapper _expressionContainerMapper;
      private readonly IOntogenyTask<ModelIndividual> _ontogenyTask;
      private readonly IMoleculeExpressionTask<ModelIndividual> _moleculeExpressionTask;
      private readonly OntogenyMapper _ontogenyMapper;

      public MoleculeMapper(
         ParameterMapper parameterMapper,
         ExpressionContainerMapper expressionContainerMapper,
         OntogenyMapper ontogenyMapper,
         IOntogenyTask<ModelIndividual> ontogenyTask,
         IMoleculeExpressionTask<ModelIndividual> moleculeExpressionTask
      ) : base(parameterMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
         _ontogenyTask = ontogenyTask;
         _moleculeExpressionTask = moleculeExpressionTask;
         _ontogenyMapper = ontogenyMapper;
      }

      public override async Task<Molecule> MapToSnapshot(IndividualMolecule molecule, ModelIndividual individual)
      {
         //We do not save parameters anymore for molecule. Those parameters are now saved as part of the individual.
         var snapshot = new Molecule();
         MapModelPropertiesToSnapshot(molecule, snapshot);
         snapshot.Type = molecule.MoleculeType;
         snapshot.Ontogeny = await _ontogenyMapper.MapToSnapshot(molecule.Ontogeny);
         snapshot.Expression = await expressionFor(molecule, individual);
         updateMoleculeSpecificPropertiesToSnapshot(snapshot, molecule);
         return snapshot;
      }

      private Task<ExpressionContainer[]> expressionFor(IndividualMolecule molecule, ModelIndividual individual)
      {
         var allExpressionContainers = individual.AllMoleculeContainersFor(molecule);
         return _expressionContainerMapper.MapToSnapshots(allExpressionContainers);
      }

      private void updateMoleculeSpecificPropertiesToSnapshot(Molecule snapshot, IndividualMolecule molecule)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               snapshot.Localization = protein.Localization;
               break;
            case IndividualTransporter transporter:
               snapshot.TransportType = transporter.TransportType;
               break;
         }
      }

      public override async Task<IndividualMolecule> MapToModel(Molecule snapshot, ModelIndividual individual)
      {
         addMoleculeToIndividual(snapshot, individual);
         var molecule = individual.MoleculeByName(snapshot.Name);
         MapSnapshotPropertiesToModel(snapshot, molecule);

         await UpdateParametersFromSnapshot(snapshot, molecule, snapshot.Type.ToString());

         updateMoleculePropertiesToMolecule(molecule, snapshot, individual);

         var ontogeny = await _ontogenyMapper.MapToModel(snapshot.Ontogeny, individual);
         _ontogenyTask.SetOntogenyForMolecule(molecule, ontogeny, individual);


         var context = new ExpressionContainerMapperContext
         {
            Molecule = molecule,
            ExpressionParameters = individual.AllExpressionParametersFor(molecule),
            MoleculeExpressionContainers = individual.AllMoleculeContainersFor(molecule)
         };

         await _expressionContainerMapper.MapToModels(snapshot.Expression, context);

         //We need to normalize relative expressions when loading from old format
         if (isV9Format(snapshot))
            NormalizeRelativeExpressionCommand.NormalizeExpressions(individual, molecule);

         return molecule;
      }

      private void updateMoleculePropertiesToMolecule(IndividualMolecule molecule, Molecule snapshot, ModelIndividual individual)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               var localization = retrieveLocalizationFrom(snapshot);
               //Set set it first to none to ensure that it is set properly after reading from the snapshot file
               protein.Localization = Localization.None;
               _moleculeExpressionTask.SetExpressionLocalizationFor(protein, localization, individual);

               break;
            case IndividualTransporter transporter:
               _moleculeExpressionTask.SetTransporterTypeFor(transporter, ModelValueFor(snapshot.TransportType));
               break;
         }
      }

      private Localization retrieveLocalizationFrom(Molecule snapshot)
      {
         if (!isV9Format(snapshot))
            return ModelValueFor(snapshot.Localization);

         //reset ot ensure we update all parameters 
         var intracellularVascularEndoLocation = ModelValueFor(snapshot.IntracellularVascularEndoLocation);
         var tissueLocation = ModelValueFor(snapshot.TissueLocation);
         var membraneLocation = ModelValueFor(snapshot.MembraneLocation);

         return LocalizationConverter.ConvertToLocalization(tissueLocation, membraneLocation, intracellularVascularEndoLocation);
      }

      private bool isV9Format(Molecule snapshot) => snapshot.Localization == null;

      private void addMoleculeToIndividual(Molecule molecule, ModelIndividual individual)
      {
         var moleculeType = molecule.Type;
         switch (moleculeType)
         {
            case QuantityType.Enzyme:
               _moleculeExpressionTask.AddMoleculeTo<IndividualEnzyme>(individual, molecule.Name);
               return;
            case QuantityType.OtherProtein:
               _moleculeExpressionTask.AddMoleculeTo<IndividualOtherProtein>(individual, molecule.Name);
               return;
            case QuantityType.Transporter:
               _moleculeExpressionTask.AddMoleculeTo<IndividualTransporter>(individual, molecule.Name);
               return;
            default:
               throw new SnapshotOutdatedException(PKSimConstants.Error.MoleculeTypeNotSupported(moleculeType.ToString()));
         }
      }
   }
}