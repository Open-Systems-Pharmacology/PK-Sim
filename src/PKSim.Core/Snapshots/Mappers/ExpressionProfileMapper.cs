using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using ModelIndividual = PKSim.Core.Model.Individual;
using ModelExpressionProfile = PKSim.Core.Model.ExpressionProfile;
using SnapshotExpressionProfile = PKSim.Core.Snapshots.ExpressionProfile;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExpressionProfileMapper : ObjectBaseSnapshotMapperBase<ModelExpressionProfile, SnapshotExpressionProfile>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly ExpressionContainerMapper _expressionContainerMapper;
      private readonly IOntogenyTask _ontogenyTask;
      private readonly IMoleculeExpressionTask<ModelIndividual> _moleculeExpressionTask;
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly OntogenyMapper _ontogenyMapper;

      public ExpressionProfileMapper(
         ParameterMapper parameterMapper,
         ExpressionContainerMapper expressionContainerMapper,
         OntogenyMapper ontogenyMapper,
         IOntogenyTask ontogenyTask,
         IMoleculeExpressionTask<ModelIndividual> moleculeExpressionTask,
         IExpressionProfileFactory expressionProfileFactory
      )
      {
         _parameterMapper = parameterMapper;
         _expressionContainerMapper = expressionContainerMapper;
         _ontogenyTask = ontogenyTask;
         _moleculeExpressionTask = moleculeExpressionTask;
         _expressionProfileFactory = expressionProfileFactory;

         _ontogenyMapper = ontogenyMapper;
      }

      public override async Task<SnapshotExpressionProfile> MapToSnapshot(ModelExpressionProfile expressionProfile)
      {
         var (molecule, individual) = expressionProfile;
         //We do not use the base method here as we want to save the name differently using the composite part of the name
         var snapshot = new SnapshotExpressionProfile
         {
            Type = molecule.MoleculeType,
            Species = individual.Species.Name,
            Category = expressionProfile.Category,
            Molecule = molecule.Name,
            Description = SnapshotValueFor(expressionProfile.Description),
            Ontogeny = await _ontogenyMapper.MapToSnapshot(molecule.Ontogeny),
            Expression = await expressionFor(molecule, individual),
            Parameters = await allParametersChangedByUserFrom(individual)
         };

         updateMoleculeSpecificPropertiesToSnapshot(snapshot, molecule);
         return snapshot;
      }

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelIndividual individual)
      {
         var changedParameters = individual.GetAllChildren<IParameter>(x => x.ShouldExportToSnapshot());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      private Task<ExpressionContainer[]> expressionFor(IndividualMolecule molecule, ModelIndividual individual)
      {
         var allExpressionContainers = individual.AllMoleculeContainersFor(molecule);
         return _expressionContainerMapper.MapToSnapshots(allExpressionContainers);
      }

      private void updateMoleculeSpecificPropertiesToSnapshot(SnapshotExpressionProfile snapshot, IndividualMolecule molecule)
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

      public override async Task<ModelExpressionProfile> MapToModel(SnapshotExpressionProfile snapshot)
      {
         var expressionProfile = _expressionProfileFactory.Create(snapshot.Type, snapshot.Species, snapshot.Molecule);
         expressionProfile.Description = snapshot.Description;
         expressionProfile.Category = snapshot.Category;

         var (molecule, individual) = expressionProfile;
         await _parameterMapper.MapLocalizedParameters(snapshot.Parameters, individual, !isV9Format(snapshot));

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
         {
            //Global parameters were saved directly under the snapshot parameter 
            await updateGlobalMoleculeParameters(snapshot, molecule);
            NormalizeRelativeExpressionCommand.NormalizeExpressions(individual, molecule);
         }

         return expressionProfile;
      }

      private Task updateGlobalMoleculeParameters(SnapshotExpressionProfile snapshot, IndividualMolecule molecule)
      {
         return _parameterMapper.MapParameters(snapshot.Parameters, molecule, molecule.Name);
      }

      private void updateMoleculePropertiesToMolecule(IndividualMolecule molecule, SnapshotExpressionProfile snapshot, ModelIndividual individual)
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

      private Localization retrieveLocalizationFrom(SnapshotExpressionProfile snapshot)
      {
         if (!isV9Format(snapshot))
            return ModelValueFor(snapshot.Localization);

         //reset ot ensure we update all parameters 
         var intracellularVascularEndoLocation = ModelValueFor(snapshot.IntracellularVascularEndoLocation);
         var tissueLocation = ModelValueFor(snapshot.TissueLocation);
         var membraneLocation = ModelValueFor(snapshot.MembraneLocation);

         return LocalizationConverter.ConvertToLocalization(tissueLocation, membraneLocation, intracellularVascularEndoLocation);
      }

      private bool isV9Format(SnapshotExpressionProfile snapshot)
      {
         //This was a protein
         if (snapshot.TransportType == null)
            return snapshot.Localization == null;

         //this is a transporter
         if (snapshot.Expression == null)
            return true;

         //We have the Membrane location flag that has disappeared in v10 and above
         return snapshot.Expression.Any(x => x.MembraneLocation != null);
      }
   }
}