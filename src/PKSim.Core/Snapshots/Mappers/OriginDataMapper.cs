﻿using System;
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
      private readonly DiseaseStateMapper _diseaseStateMapper;
      private readonly ParameterMapper _parameterMapper;
      private readonly IOriginDataTask _originDataTask;
      private readonly IIndividualModelTask _individualModelTask;
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;
      private readonly ValueOriginMapper _valueOriginMapper;

      public OriginDataMapper(ParameterMapper parameterMapper,
         CalculationMethodCacheMapper calculationMethodCacheMapper,
         ValueOriginMapper valueOriginMapper,
         DiseaseStateMapper diseaseStateMapper,
         IOriginDataTask originDataTask,
         IDimensionRepository dimensionRepository,
         IIndividualModelTask individualModelTask,
         ISpeciesRepository speciesRepository)
      {
         _dimensionRepository = dimensionRepository;
         _speciesRepository = speciesRepository;
         _diseaseStateMapper = diseaseStateMapper;
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
            x.Population = originData.Species.Populations.Count > 1 ? originData.Population.Name : null;
            x.Gender = originData.Population.Genders.Count > 1 ? originData.Gender.Name : null;
         });

         if (originData.Population.IsAgeDependent)
         {
            //Always generate age for Age dependent species
            snapshot.Age = parameterFrom(originData.Age, _dimensionRepository.AgeInYears);
            snapshot.GestationalAge = originDataParameterFor(originData, _individualModelTask.MeanGestationalAgeFor, originData.GestationalAge, _dimensionRepository.AgeInWeeks);
         }

         snapshot.Weight = originDataParameterFor(originData, _individualModelTask.MeanWeightFor, originData.Weight, _dimensionRepository.Mass);

         if (originData.Population.IsHeightDependent)
            snapshot.Height = originDataParameterFor(originData, _individualModelTask.MeanHeightFor, originData.Height, _dimensionRepository.Length);

         snapshot.ValueOrigin = await _valueOriginMapper.MapToSnapshot(originData.ValueOrigin);
         snapshot.CalculationMethods = await _calculationMethodCacheMapper.MapToSnapshot(originData.CalculationMethodCache, originData.Species.Name);

         snapshot.Disease = await _diseaseStateMapper.MapToSnapshot(originData);

         return snapshot;
      }

      private Parameter originDataParameterFor(ModelOriginData originData, Func<ModelOriginData, IParameter> meanParameterRetrieverFunc, OriginDataParameter parameter, IDimension dimension)
      {
         if (parameter == null)
            return null;

         var (value, _) = parameter;
         var meanParameter = meanParameterRetrieverFunc(originData);

         if (ValueComparer.AreValuesEqual(meanParameter.Value, value))
            return null;

         return parameterFrom(parameter, dimension);
      }

      public override async Task<ModelOriginData> MapToModel(SnapshotOriginData snapshot, SnapshotContext snapshotContext)
      {
         var originData = new ModelOriginData { Species = speciesFrom(snapshot) };

         originData.Population = speciesPopulationFrom(snapshot, originData.Species);
         originData.Gender = genderFrom(snapshot, originData.Population);
         originData.SubPopulation = _originDataTask.DefaultSubPopulationFor(originData.Species);

         _valueOriginMapper.UpdateValueOrigin(originData.ValueOrigin, snapshot.ValueOrigin);

         updateAgeFromSnapshot(snapshot, originData);
         updateCalculationMethodsFromSnapshot(snapshot, originData);

         updateWeightFromSnapshot(snapshot, originData);
         updateHeightFromSnapshot(snapshot, originData);

         await updateDiseaseStateFromSnapshot(snapshot, originData, snapshotContext);
         return originData;
      }

      private async Task updateDiseaseStateFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData, SnapshotContext snapshotContext)
      {
         //we might have an old disease state implementation
         if (snapshot.DiseaseState != null)
         {
            //conversion for older version
            snapshot.Disease = new DiseaseState
            {
               Name = snapshot.DiseaseState,
               Parameters = snapshot.DiseaseStateParameters
            };
         }

         await _diseaseStateMapper.MapToModel(snapshot.Disease, new DiseaseStateContext(originData, snapshotContext));
      }

      private void updateWeightFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         originData.Weight = getOriginDataValues(originData, _individualModelTask.MeanWeightFor, snapshot.Weight);
      }

      private void updateHeightFromSnapshot(SnapshotOriginData snapshot, ModelOriginData originData)
      {
         if (!originData.Population.IsHeightDependent)
            return;

         originData.Height = getOriginDataValues(originData, _individualModelTask.MeanHeightFor, snapshot.Height);
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
         if (!originData.Population.IsAgeDependent)
            return;

         originData.Age = getOriginDataValues(originData, _individualModelTask.MeanAgeFor, snapshot.Age);
         originData.GestationalAge = getOriginDataValues(originData, _individualModelTask.MeanGestationalAgeFor, snapshot.GestationalAge);
      }

      private OriginDataParameter getOriginDataValues(ModelOriginData originData, Func<ModelOriginData, IParameter> meanParameterRetrieverFunc, Parameter snapshotParameter)
      {
         var meanParameter = meanParameterRetrieverFunc(originData);
         var value = baseParameterValueFrom(snapshotParameter, meanParameter.Dimension, meanParameter.Value);
         var unit = meanParameter.Dimension.UnitOrDefault(snapshotParameter?.Unit).Name;
         return new OriginDataParameter { Value = value, Unit = unit };
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

      private double baseParameterValueFrom(Parameter snapshot, IDimension dimension, double defaultValueInBaseUnit)
      {
         if (snapshot?.Value == null)
            return defaultValueInBaseUnit;

         var unit = dimension.Unit(ModelValueFor(snapshot.Unit));
         return dimension.UnitValueToBaseUnitValue(unit, snapshot.Value.Value);
      }

      private Parameter parameterFrom(OriginDataParameter parameter, IDimension dimension)
      {
         if (parameter == null)
            return null;

         return _parameterMapper.ParameterFrom(parameter.Value, parameter.Unit, dimension);
      }
   }
}