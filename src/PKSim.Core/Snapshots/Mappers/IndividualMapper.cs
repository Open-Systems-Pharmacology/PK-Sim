using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;

namespace PKSim.Core.Snapshots.Mappers
{
   public class IndividualMapper : ObjectBaseSnapshotMapperBase<ModelIndividual, SnapshotIndividual>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly MoleculeMapper _moleculeMapper;
      private readonly IIndividualFactory _individualFactory;
      private readonly IOriginDataMapper _originDataMapper;

      public IndividualMapper(ParameterMapper parameterMapper, IDimensionRepository dimensionRepository, MoleculeMapper moleculeMapper,
         IIndividualFactory individualFactory, IOriginDataMapper originDataMapper)
      {
         _parameterMapper = parameterMapper;
         _dimensionRepository = dimensionRepository;
         _moleculeMapper = moleculeMapper;
         _individualFactory = individualFactory;
         _originDataMapper = originDataMapper;
      }

      public override async Task<SnapshotIndividual> MapToSnapshot(ModelIndividual individual)
      {
         var snapshot = await SnapshotFrom(individual, x =>
         {
            mapOriginDataToSnapshot(x, individual.OriginData);
            x.Seed = individual.Seed;
         });

         snapshot.Parameters = await allParametersChangedByUserFrom(individual);
         snapshot.Molecules = await allMoleculesFrom(individual);
         return snapshot;
      }

      private Task<Molecule[]> allMoleculesFrom(ModelIndividual individual)
      {
         return _moleculeMapper.MapToSnapshots(individual.AllDefinedMolecules());
      }

      private void mapOriginDataToSnapshot(SnapshotIndividual snapshot, OriginData originData)
      {
         snapshot.Species = originData.Species.Name;
         snapshot.Population = originData.SpeciesPopulation.Name;
         snapshot.Gender = originData.Gender.Name;
         snapshot.Age = parameterFrom(originData.Age, originData.AgeUnit, _dimensionRepository.AgeInYears);
         snapshot.GestationalAge = parameterFrom(originData.GestationalAge, originData.GestationalAgeUnit, _dimensionRepository.AgeInWeeks);
         snapshot.Height = parameterFrom(originData.Height, originData.HeightUnit, _dimensionRepository.Length);
         snapshot.Weight = parameterFrom(originData.Weight, originData.WeightUnit, _dimensionRepository.Mass);
      }

      private Parameter parameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         return _parameterMapper.ParameterFrom(parameterBaseValue, parameterDisplayUnit, dimension);
      }

      private Task<LocalizedParameter[]> allParametersChangedByUserFrom(ModelIndividual individual)
      {
         var changedParameters = individual.Organism.GetAllChildren<IParameter>(x => x.ValueDiffersFromDefault());
         return _parameterMapper.LocalizedParametersFrom(changedParameters);
      }

      public override async Task<ModelIndividual> MapToModel(SnapshotIndividual individualSnapshot)
      {
         var originData = createOriginDataFrom(individualSnapshot);
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

      private OriginData createOriginDataFrom(SnapshotIndividual individualSnapshot)
      {
         var batchOriginData = new Batch.OriginData
         {
            Species = individualSnapshot.Species,
            Population = individualSnapshot.Population,
            Gender = individualSnapshot.Gender,
            Age = baseParameterValueFrom(individualSnapshot.Age, _dimensionRepository.AgeInYears),
            Height = baseParameterValueFrom(individualSnapshot.Height, _dimensionRepository.Length),
            Weight = baseParameterValueFrom(individualSnapshot.Weight, _dimensionRepository.Mass),
            GestationalAge = baseParameterValueFrom(individualSnapshot.GestationalAge, _dimensionRepository.AgeInWeeks),
         };

         return _originDataMapper.MapFrom(batchOriginData);
      }

      private double baseParameterValueFrom(Parameter snapshot, IDimension dimension)
      {
         if (snapshot?.Value == null)
            return double.NaN;

         var unit = dimension.Unit(ModelValueFor(snapshot.Unit));
         return dimension.UnitValueToBaseUnitValue(unit, snapshot.Value.Value);
      }
   }
}