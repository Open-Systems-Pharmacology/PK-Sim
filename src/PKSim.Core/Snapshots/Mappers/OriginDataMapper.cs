using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using ModelOriginData = PKSim.Core.Model.OriginData;
using SnapshotOriginData = PKSim.Core.Snapshots.OriginData;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OriginDataMapper : SnapshotMapperBase<ModelOriginData, SnapshotOriginData>
   {
      private readonly IDimensionRepository _dimensionRepository;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ParameterMapper _parameterMapper;
      private readonly IOriginDataTask _originDataTask;
      private readonly IIndividualModelTask _individualModelTask;
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly ValueOriginMapper _valueOriginMapper;

      public OriginDataMapper(
         ParameterMapper parameterMapper,
         CalculationMethodCacheMapper calculationMethodCacheMapper,
         ValueOriginMapper valueOriginMapper,
         IOriginDataTask originDataTask,
         IDimensionRepository dimensionRepository,
         IIndividualModelTask individualModelTask,
         ISpeciesRepository speciesRepository
      )
      {
         _dimensionRepository = dimensionRepository;
         _speciesRepository = speciesRepository;
         _parameterMapper = parameterMapper;
         _originDataTask = originDataTask;
         _individualModelTask = individualModelTask;
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
         _valueOriginMapper = valueOriginMapper;
      }

      public override async Task<SnapshotOriginData> MapToSnapshot(ModelOriginData originData)
      {
         var snapshot = await SnapshotFrom(originData, x =>
         {
            x.Species = originData.Species.Name;
            x.Population = originData.Species.Populations.Count > 1 ? originData.SpeciesPopulation.Name : null;
            x.Gender = originData.SpeciesPopulation.Genders.Count > 1 ? originData.Gender.Name : null;
         });

         if (originData.SpeciesPopulation.IsAgeDependent)
         {
            snapshot.Age = originDataParameterFor(originData, _individualModelTask.MeanAgeFor, originData.Age, originData.AgeUnit, _dimensionRepository.AgeInYears);
            snapshot.GestationalAge = originDataParameterFor(originData, _individualModelTask.MeanGestationalAgeFor, originData.GestationalAge, originData.GestationalAgeUnit, _dimensionRepository.AgeInWeeks);
         }

         snapshot.Weight = originDataParameterFor(originData, _individualModelTask.MeanWeightFor, originData.Weight, originData.WeightUnit, _dimensionRepository.Mass);

         if (originData.SpeciesPopulation.IsHeightDependent)
            snapshot.Height = originDataParameterFor(originData, _individualModelTask.MeanHeightFor, originData.Height, originData.HeightUnit, _dimensionRepository.Length);

         snapshot.ValueOrigin = await _valueOriginMapper.MapToSnapshot(originData.ValueOrigin);
         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(originData.CalculationMethodCache, originData.Species.Name);
         return snapshot;
      }

      private Parameter originDataParameterFor(ModelOriginData originData, Func<ModelOriginData, IParameter> meanParameterRetrieverFunc, double? value, string unit, IDimension dimension)
      {
         var meanParameter = meanParameterRetrieverFunc(originData);

         if (value == null || ValueComparer.AreValuesEqual(meanParameter.Value, value.Value))
            return null;

         return parameterFrom(value, unit, dimension);
      }

      public override Task<ModelOriginData> MapToModel(SnapshotOriginData snapshot)
      {
         var originData = new ModelOriginData {Species = speciesFrom(snapshot)};

         originData.SpeciesPopulation = speciesPopulationFrom(snapshot, originData.Species);
         originData.Gender = genderFrom(snapshot, originData.SpeciesPopulation);
         originData.SubPopulation = _originDataTask.DefaultSubPopulationFor(originData.Species);

         _valueOriginMapper.UpdateValueOrigin(originData.ValueOrigin, snapshot.ValueOrigin);

         updateAgeFromSnapshot(snapshot, originData);
         updateCalculationMethodsFromSnapshot(snapshot, originData);

         updateWeightFromSnapshot(snapshot, originData);
         updateHeightFromSnapshot(snapshot, originData);

         return Task.FromResult(originData);
      }

      private void updateWeightFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         (originData.Weight, originData.WeightUnit) = getOriginDataValues(originData, _individualModelTask.MeanWeightFor, snapshot.Weight);
      }

      private void updateHeightFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         if (!originData.SpeciesPopulation.IsHeightDependent)
            return;

         (originData.Height, originData.HeightUnit) = getOriginDataValues(originData, _individualModelTask.MeanHeightFor, snapshot.Height);
      }

      private void updateCalculationMethodsFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         var defaultCalculationMethodsForSpecs = _originDataTask.AllCalculationMethodCategoryFor(originData.Species)
            .Select(x => x.DefaultItemForSpecies(originData.Species));

         defaultCalculationMethodsForSpecs.Each(originData.AddCalculationMethod);

         _calculationMethodCacheMapper.UpdateCalculationMethodCache(originData, snapshot.CalculationMethods);
      }

      private void updateAgeFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         if (!originData.SpeciesPopulation.IsAgeDependent)
            return;

         (originData.Age, originData.AgeUnit) = getOriginDataValues(originData, _individualModelTask.MeanAgeFor, snapshot.Age);
         (originData.GestationalAge, originData.GestationalAgeUnit) = getOriginDataValues(originData, _individualModelTask.MeanGestationalAgeFor, snapshot.GestationalAge);
      }

      private (double value, string unit) getOriginDataValues(ModelOriginData originData, Func<ModelOriginData, IParameter> meanParameterRetrieverFunc, Parameter snapshotParameter)
      {
         var meanParameter = meanParameterRetrieverFunc(originData);
         var value = baseParameterValueFrom(snapshotParameter, meanParameter.Dimension, meanParameter.Value);
         var unit = meanParameter.Dimension.UnitOrDefault(snapshotParameter?.Unit).Name;
         return (value, unit);
      }

      private Gender genderFrom(SnapshotOriginData snapshot, SpeciesPopulation speciesPopulation)
      {
         var gender = speciesPopulation.GenderByName(snapshot.Gender);
         if (gender != null)
            return gender;

         if (string.IsNullOrEmpty(snapshot.Gender))
            return speciesPopulation.Genders.ElementAt(0);

         throw new PKSimException(PKSimConstants.Error.CouldNotFindGenderForPopulation(snapshot.Gender, snapshot.Population, speciesPopulation.Genders.AllNames()));
      }

      private SpeciesPopulation speciesPopulationFrom(SnapshotOriginData snapshot, Species species)
      {
         var population = species.PopulationByName(snapshot.Population);
         if (population != null)
            return population;

         if (string.IsNullOrEmpty(snapshot.Population) && species.Populations.Count() == 1)
            return species.Populations.ElementAt(0);

         throw new PKSimException(PKSimConstants.Error.CouldNotFindPopulationForSpecies(snapshot.Population, snapshot.Species, species.Populations.AllNames()));
      }

      private Species speciesFrom(SnapshotOriginData snapshot)
      {
         var species = _speciesRepository.FindByName(snapshot.Species);
         return species ?? throw new PKSimException(PKSimConstants.Error.CouldNotFindSpecies(snapshot.Species, _speciesRepository.AllNames()));
      }

      private double baseParameterValueFrom(Parameter snapshot, IDimension dimension, double defaultValue)
      {
         if (snapshot?.Value == null)
            return defaultValue;

         var unit = dimension.Unit(ModelValueFor(snapshot.Unit));
         return dimension.UnitValueToBaseUnitValue(unit, snapshot.Value.Value);
      }

      private Parameter parameterFrom(double? parameterBaseValue, string parameterDisplayUnit, IDimension dimension)
      {
         return _parameterMapper.ParameterFrom(parameterBaseValue, parameterDisplayUnit, dimension);
      }
   }
}