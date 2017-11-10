using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;

namespace PKSim.Core.Snapshots.Mappers
{
   public class IndividualMapper : ObjectBaseSnapshotMapperBase<ModelIndividual, SnapshotIndividual>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly MoleculeMapper _moleculeMapper;
      private readonly IIndividualFactory _individualFactory;
      private readonly OriginDataMapper _originDataMapper;

      public IndividualMapper(
         ParameterMapper parameterMapper, 
         MoleculeMapper moleculeMapper,
         OriginDataMapper originDataMapper,
         IIndividualFactory individualFactory
         )
      {
         _parameterMapper = parameterMapper;
         _moleculeMapper = moleculeMapper;
         _individualFactory = individualFactory;
         _originDataMapper = originDataMapper;
         _originDataMapper = originDataMapper;
      }

      public override async Task<SnapshotIndividual> MapToSnapshot(ModelIndividual individual)
      {
         var snapshot = await SnapshotFrom(individual, x => { x.Seed = individual.Seed; });

         snapshot.OriginData = await _originDataMapper.MapToSnapshot(individual.OriginData);
         snapshot.Parameters = await allParametersChangedByUserFrom(individual);
         snapshot.Molecules = await allMoleculesFrom(individual);
         return snapshot;
      }

      private Task<Molecule[]> allMoleculesFrom(ModelIndividual individual)
      {
         return _moleculeMapper.MapToSnapshots(individual.AllDefinedMolecules());
      }

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelIndividual individual)
      {
         var changedParameters = individual.Organism.GetAllChildren<IParameter>(x => x.ValueDiffersFromDefault());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      public override async Task<ModelIndividual> MapToModel(SnapshotIndividual individualSnapshot)
      {
         var originData = await _originDataMapper.MapToModel(individualSnapshot.OriginData);
         var individual = _individualFactory.CreateAndOptimizeFor(originData, individualSnapshot.Seed);
         MapSnapshotPropertiesToModel(individualSnapshot, individual);
         await updateIndividualParameters(individualSnapshot, individual);
         var molecules = await _moleculeMapper.MapToModels(individualSnapshot.Molecules, individual);
         molecules?.Each(individual.AddMolecule);
         return individual;
      }

      private Task updateIndividualParameters(SnapshotIndividual snapshot, ModelIndividual individual)
      {
         return _parameterMapper.MapLocalizedParameters(snapshot.Parameters, individual.Organism);
      }
   }
}