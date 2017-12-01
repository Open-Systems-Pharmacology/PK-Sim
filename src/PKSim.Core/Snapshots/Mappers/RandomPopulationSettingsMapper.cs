using System.Threading.Tasks;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Snapshots.Mappers
{
   public class RandomPopulationSettingsMapper : SnapshotMapperBase<RandomPopulationSettings, PopulationSettings>
   {
      private readonly ParameterRangeMapper _parameterRangeMapper;
      private readonly IGenderRepository _genderRepository;
      private readonly IIndividualToPopulationSettingsMapper _populationSettingsMapper;
      private readonly IndividualMapper _individualMapper;

      public RandomPopulationSettingsMapper(
         ParameterRangeMapper parameterRangeMapper,
         IndividualMapper individualMapper,
         IIndividualToPopulationSettingsMapper populationSettingsMapper,
         IGenderRepository genderRepository)
      {
         _parameterRangeMapper = parameterRangeMapper;
         _populationSettingsMapper = populationSettingsMapper;
         _genderRepository = genderRepository;
         _individualMapper = individualMapper;
      }

      public override async Task<PopulationSettings> MapToSnapshot(RandomPopulationSettings randomPopulationSettings)
      {
         var snapshot = await SnapshotFrom(randomPopulationSettings);
         snapshot.Individual = await _individualMapper.MapToSnapshot(randomPopulationSettings.BaseIndividual);
         snapshot.NumberOfIndividuals = randomPopulationSettings.NumberOfIndividuals;
         snapshot.ProportionOfFemales = proportionOfFemalesFrom(randomPopulationSettings);
         snapshot.Age = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameter.AGE);
         snapshot.Weight = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameter.MEAN_WEIGHT);
         snapshot.Height = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameter.MEAN_HEIGHT);
         snapshot.GestationalAge = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameter.GESTATIONAL_AGE);
         snapshot.BMI = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameter.BMI);
         return snapshot;
      }

      private Task<ParameterRange> snapshotRangeFor(RandomPopulationSettings randomPopulationSettings, string parameterName)
      {
         return _parameterRangeMapper.MapToSnapshot(randomPopulationSettings.ParameterRange(parameterName));
      }

      private Task updateModelRange(RandomPopulationSettings randomPopulationSettings, string parameterName, ParameterRange parameterRange)
      {
         return _parameterRangeMapper.MapToModel(parameterRange, randomPopulationSettings.ParameterRange(parameterName));
      }

      public override async Task<RandomPopulationSettings> MapToModel(PopulationSettings snapshot)
      {
         var individual = await _individualMapper.MapToModel(snapshot.Individual);
         var settings = _populationSettingsMapper.MapFrom(individual);
         settings.NumberOfIndividuals = snapshot.NumberOfIndividuals;
         updateGenderRatios(settings, snapshot);
         await updateModelRange(settings, CoreConstants.Parameter.AGE, snapshot.Age);
         await updateModelRange(settings, CoreConstants.Parameter.MEAN_WEIGHT, snapshot.Weight);
         await updateModelRange(settings, CoreConstants.Parameter.MEAN_HEIGHT, snapshot.Height);
         await updateModelRange(settings, CoreConstants.Parameter.GESTATIONAL_AGE, snapshot.GestationalAge);
         await updateModelRange(settings, CoreConstants.Parameter.BMI, snapshot.BMI);
         return settings;
      }

      private void updateGenderRatios(RandomPopulationSettings randomPopulationSettings, PopulationSettings snapshot)
      {
         if (snapshot.ProportionOfFemales == null)
            return;

         var femaleRatio = randomPopulationSettings.GenderRatio(_genderRepository.Female);
         var maleRatio = randomPopulationSettings.GenderRatio(_genderRepository.Male);
         if (femaleRatio == null || maleRatio == null)
            return;

         femaleRatio.Ratio = snapshot.ProportionOfFemales.Value;
         maleRatio.Ratio = 100 - femaleRatio.Ratio;
      }

      private int? proportionOfFemalesFrom(RandomPopulationSettings randomPopulationSettings)
      {
         var female = _genderRepository.Female;
         return randomPopulationSettings.GenderRatio(female)?.Ratio;
      }
   }
}