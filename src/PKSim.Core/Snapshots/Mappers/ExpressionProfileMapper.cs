using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using ModelIndividual = PKSim.Core.Model.Individual;
using SnapshotExpressionProfile = PKSim.Core.Snapshots.ExpressionProfile;
using ModelExpressionProfile = PKSim.Core.Model.ExpressionProfile;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExpressionProfileMapper : ObjectBaseSnapshotMapperBase<ModelExpressionProfile, SnapshotExpressionProfile>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly ExpressionContainerMapper _expressionContainerMapper;
      private readonly IOntogenyTask _ontogenyTask;
      private readonly IMoleculeExpressionTask<ModelIndividual> _moleculeExpressionTask;
      private readonly IExpressionProfileFactory _expressionProfileFactory;
      private readonly IMoleculeParameterTask _moleculeParameterTask;
      private readonly OntogenyMapper _ontogenyMapper;

      public ExpressionProfileMapper(
         ParameterMapper parameterMapper,
         ExpressionContainerMapper expressionContainerMapper,
         OntogenyMapper ontogenyMapper,
         IOntogenyTask ontogenyTask,
         IMoleculeExpressionTask<ModelIndividual> moleculeExpressionTask,
         IExpressionProfileFactory expressionProfileFactory,
         IMoleculeParameterTask moleculeParameterTask 
      )
      {
         _parameterMapper = parameterMapper;
         _expressionContainerMapper = expressionContainerMapper;
         _ontogenyTask = ontogenyTask;
         _moleculeExpressionTask = moleculeExpressionTask;
         _expressionProfileFactory = expressionProfileFactory;
         _moleculeParameterTask = moleculeParameterTask;

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

      public override async Task<ModelExpressionProfile> MapToModel(SnapshotExpressionProfile snapshot, SnapshotContext snapshotContext)
      {
         var expressionProfile = _expressionProfileFactory.Create(snapshot.Type, snapshot.Species, snapshot.Molecule);
         expressionProfile.Description = snapshot.Description;
         expressionProfile.Category = snapshot.Category;

         var (molecule, individual) = expressionProfile;
         //Update molecule properties first
         updateMoleculePropertiesToMolecule(molecule, snapshot, individual, snapshotContext);

         //Then override all parameters that were set 
         await _parameterMapper.MapLocalizedParameters(snapshot.Parameters, individual, snapshotContext, !snapshotContext.IsV9FormatOrEarlier);

         var snapshotWithSubjectContext = new SnapshotContextWithSubject(individual, snapshotContext);
         var ontogeny = await _ontogenyMapper.MapToModel(snapshot.Ontogeny, snapshotWithSubjectContext);
         _ontogenyTask.SetOntogenyForMolecule(molecule, ontogeny, individual);

         var context = new ExpressionContainerMapperContext(snapshotContext)
         {
            Molecule = molecule,
            ExpressionParameters = individual.AllExpressionParametersFor(molecule),
            MoleculeExpressionContainers = individual.AllMoleculeContainersFor(molecule),
         };
         await _expressionContainerMapper.MapToModels(snapshot.Expression, context);

         //We need to normalize relative expressions when loading from old format
         if (snapshotContext.IsV9FormatOrEarlier)
         {
            //Make sure we load the default parameters from db just in case we were dealing with a standard molecule
            _moleculeParameterTask.SetDefaultMoleculeParameters(molecule);

            //Global parameters were saved directly under the snapshot parameter 
            await updateGlobalMoleculeParameters(snapshot, molecule, snapshotContext);
            NormalizeRelativeExpressionCommand.NormalizeExpressions(individual, molecule);
         }

         return expressionProfile;
      }

      private Task updateGlobalMoleculeParameters(SnapshotExpressionProfile snapshot, IndividualMolecule molecule, SnapshotContext snapshotContext)
      {
         return _parameterMapper.MapParameters(snapshot.Parameters, molecule, molecule.Name, snapshotContext);
      }

      private void updateMoleculePropertiesToMolecule(IndividualMolecule molecule, SnapshotExpressionProfile snapshot, ModelIndividual individual, SnapshotContext snapshotContext)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               var localization = retrieveLocalizationFrom(snapshot, snapshotContext);
               //Set set it first to none to ensure that it is set properly after reading from the snapshot file
               protein.Localization = Localization.None;
               _moleculeExpressionTask.SetExpressionLocalizationFor(protein, localization, individual);

               break;
            case IndividualTransporter transporter:
               _moleculeExpressionTask.SetTransporterTypeFor(transporter, ModelValueFor(snapshot.TransportType));
               break;
         }
      }

      private Localization retrieveLocalizationFrom(SnapshotExpressionProfile snapshot, SnapshotContext snapshotContext)
      {
         if (!snapshotContext.IsV9FormatOrEarlier)
            return ModelValueFor(snapshot.Localization);

         //reset ot ensure we update all parameters 
         var intracellularVascularEndoLocation = ModelValueFor(snapshot.IntracellularVascularEndoLocation);
         var tissueLocation = ModelValueFor(snapshot.TissueLocation);
         var membraneLocation = ModelValueFor(snapshot.MembraneLocation);

         return LocalizationConverter.ConvertToLocalization(tissueLocation, membraneLocation, intracellularVascularEndoLocation);
      }

   }
}