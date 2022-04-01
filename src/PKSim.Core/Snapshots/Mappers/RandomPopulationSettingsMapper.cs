using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Snapshots.Mappers
{
   public class RandomPopulationSettingsMapper : SnapshotMapperBase<RandomPopulationSettings, PopulationSettings, PKSimProject>, ISnapshotMapperWithProjectAsContext<RandomPopulationSettings, PopulationSettings>
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
         var individual = randomPopulationSettings.BaseIndividual;
         snapshot.Individual = await _individualMapper.MapToSnapshot(individual);
         snapshot.NumberOfIndividuals = randomPopulationSettings.NumberOfIndividuals;
         snapshot.ProportionOfFemales = proportionOfFemalesFrom(randomPopulationSettings);
         snapshot.Age = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameters.AGE);
         snapshot.Weight = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameters.MEAN_WEIGHT);
         snapshot.Height = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameters.MEAN_HEIGHT);
         snapshot.GestationalAge = await snapshotRangeFor(randomPopulationSettings, Constants.Parameters.GESTATIONAL_AGE);
         snapshot.BMI = await snapshotRangeFor(randomPopulationSettings, CoreConstants.Parameters.BMI);
         snapshot.DiseaseStateParameters = await snapshotDiseaseStateParameters(individual, randomPopulationSettings);

         return snapshot;
      }

      private async Task<ParameterRange[]> snapshotDiseaseStateParameters(Model.Individual individual, RandomPopulationSettings randomPopulationSettings)
      {
         var diseaseStateParameterRanges = individual.OriginData?.DiseaseStateParameters
            .Select(x => randomPopulationSettings.ParameterRange(x.Name))
            .ToList();

         if (diseaseStateParameterRanges == null)
            return null;

         if (!diseaseStateParameterRanges.Any())
            return null;

         var snapshotParameterRanges = new List<ParameterRange>();

         foreach (var parameterRange in diseaseStateParameterRanges)
         {
            var snapshot = await _parameterRangeMapper.MapToSnapshot(parameterRange);
            snapshot.Name = parameterRange.ParameterName;
            snapshotParameterRanges.Add(snapshot);
         }

         return snapshotParameterRanges.ToArray();
      }

      private Task<ParameterRange> snapshotRangeFor(RandomPopulationSettings randomPopulationSettings, string parameterName)
      {
         return _parameterRangeMapper.MapToSnapshot(randomPopulationSettings.ParameterRange(parameterName));
      }

      public override async Task<RandomPopulationSettings> MapToModel(PopulationSettings snapshot, PKSimProject project)
      {
         var individual = await _individualMapper.MapToModel(snapshot.Individual, project);
         var settings = _populationSettingsMapper.MapFrom(individual);
         settings.NumberOfIndividuals = snapshot.NumberOfIndividuals;
         updateGenderRatios(settings, snapshot);
         await updateModelRange(settings, CoreConstants.Parameters.AGE, snapshot.Age);
         await updateModelRange(settings, CoreConstants.Parameters.MEAN_WEIGHT, snapshot.Weight);
         await updateModelRange(settings, CoreConstants.Parameters.MEAN_HEIGHT, snapshot.Height);
         await updateModelRange(settings, Constants.Parameters.GESTATIONAL_AGE, snapshot.GestationalAge);
         await updateModelRange(settings, CoreConstants.Parameters.BMI, snapshot.BMI);

         await addDiseaseStateParameters(settings, snapshot);
         return settings;
      }

      private async Task addDiseaseStateParameters(RandomPopulationSettings randomPopulationSettings, PopulationSettings snapshot)
      {
         if (snapshot.DiseaseStateParameters == null)
            return;

         foreach (var snapshotDiseaseStateParameter in snapshot.DiseaseStateParameters)
         {
            await updateModelRange(randomPopulationSettings, snapshotDiseaseStateParameter.Name, snapshotDiseaseStateParameter);
         }
      }

      private Task updateModelRange(RandomPopulationSettings randomPopulationSettings, string parameterName, ParameterRange parameterRange)
      {
         return updateModelRange(parameterRange, randomPopulationSettings.ParameterRange(parameterName));
      }

      private Task updateModelRange(ParameterRange parameterRange, Model.ParameterRange modelParameterRange)
      {
         return _parameterRangeMapper.MapToModel(parameterRange, modelParameterRange);
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