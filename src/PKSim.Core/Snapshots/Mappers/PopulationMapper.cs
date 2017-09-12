using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotPopulation = PKSim.Core.Snapshots.Population;
using ModelPopulation = PKSim.Core.Model.Population;

namespace PKSim.Core.Snapshots.Mappers
{
   public class PopulationMapper : ObjectBaseSnapshotMapperBase<ModelPopulation, SnapshotPopulation>
   {
      private readonly IndividualMapper _individualMapper;
      private readonly ParameterRangeMapper _parameterRangeMapper;
      private readonly AdvancedParameterMapper _advancedParameterMapper;
      private readonly IGenderRepository _genderRepository;

      public PopulationMapper(
         IndividualMapper individualMapper,
         ParameterRangeMapper parameterRangeMapper,
         AdvancedParameterMapper advancedParameterMapper,
         IGenderRepository genderRepository)
      {
         _individualMapper = individualMapper;
         _parameterRangeMapper = parameterRangeMapper;
         _advancedParameterMapper = advancedParameterMapper;
         _genderRepository = genderRepository;
      }

      public override SnapshotPopulation MapToSnapshot(ModelPopulation population)
      {
         return SnapshotFrom(population, snapshot =>
         {
            snapshot.Seed = population.Seed;
            mapPopulationProperties(snapshot, population);
         });
      }

      private void mapPopulationProperties(SnapshotPopulation snapshot, ModelPopulation population)
      {
         switch (population)
         {
            case RandomPopulation randomPopulation:
               mapRandomPopulationProperties(snapshot, randomPopulation);
               break;
            default:
               throw new OSPSuiteException(PKSimConstants.Error.PopulationSnapshotOnlySupportedForRandomPopulation);
         }
      }

      private void mapRandomPopulationProperties(SnapshotPopulation snapshot, RandomPopulation randomPopulation)
      {
         mapIndividualToSnapshot(snapshot, randomPopulation);
         snapshot.NumberOfIndividuals = randomPopulation.Settings.NumberOfIndividuals;
         snapshot.ProportionOfFemales = proportionOfFemalesFrom(randomPopulation.Settings);
         snapshot.Age = snapshotRangeFor(randomPopulation.Settings, CoreConstants.Parameter.AGE);
         snapshot.Weight = snapshotRangeFor(randomPopulation.Settings, CoreConstants.Parameter.MEAN_WEIGHT);
         snapshot.Height = snapshotRangeFor(randomPopulation.Settings, CoreConstants.Parameter.MEAN_HEIGHT);
         snapshot.GestationalAge = snapshotRangeFor(randomPopulation.Settings, CoreConstants.Parameter.GESTATIONAL_AGE);
         snapshot.BMI = snapshotRangeFor(randomPopulation.Settings, CoreConstants.Parameter.BMI);
         snapshot.AdvancedParameters = snapshotAdvancedParametersFrom(randomPopulation);
      }

      private List<AdvancedParameter> snapshotAdvancedParametersFrom(ModelPopulation population)
      {
         return population.AdvancedParameters.Select(_advancedParameterMapper.MapToSnapshot).ToList();
      }

      private ParameterRange snapshotRangeFor(RandomPopulationSettings randomPopulationSettings, string parameterName) => _parameterRangeMapper.MapToSnapshot(randomPopulationSettings.ParameterRange(parameterName));

      private double? proportionOfFemalesFrom(RandomPopulationSettings randomPopulationSettings)
      {
         var female = _genderRepository.Female;
         return randomPopulationSettings.GenderRatio(female)?.Ratio;
      }

      private void mapIndividualToSnapshot(SnapshotPopulation snapshot, ModelPopulation population)
      {
         snapshot.Individual = _individualMapper.MapToSnapshot(population.FirstIndividual);
      }

      public override ModelPopulation MapToModel(SnapshotPopulation snapshot)
      {
         throw new NotImplementedException();
      }
   }
}