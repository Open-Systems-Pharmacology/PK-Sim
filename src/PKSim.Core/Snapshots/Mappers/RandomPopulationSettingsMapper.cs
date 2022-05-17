using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Assets;
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
      private readonly IOSPSuiteLogger _logger;

      public RandomPopulationSettingsMapper(
         ParameterRangeMapper parameterRangeMapper,
         IndividualMapper individualMapper,
         IIndividualToPopulationSettingsMapper populationSettingsMapper,
         IGenderRepository genderRepository,
         IOSPSuiteLogger logger)
      {
         _parameterRangeMapper = parameterRangeMapper;
         _populationSettingsMapper = populationSettingsMapper;
         _genderRepository = genderRepository;
         _logger = logger;
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

      public override async Task<RandomPopulationSettings> MapToModel(PopulationSettings snapshot, SnapshotContext snapshotContext)
      {
         var individual = await _individualMapper.MapToModel(snapshot.Individual, snapshotContext);
         var settings = _populationSettingsMapper.MapFrom(individual);
         settings.NumberOfIndividuals = snapshot.NumberOfIndividuals;
         updateGenderRatios(settings, snapshot);
         await updateModelRange(snapshot.Age, CoreConstants.Parameters.AGE, settings, snapshotContext);
         await updateModelRange(snapshot.Weight, CoreConstants.Parameters.MEAN_WEIGHT, settings, snapshotContext);
         await updateModelRange(snapshot.Height, CoreConstants.Parameters.MEAN_HEIGHT, settings, snapshotContext);
         await updateModelRange(snapshot.GestationalAge, Constants.Parameters.GESTATIONAL_AGE, settings, snapshotContext);
         await updateModelRange(snapshot.BMI, CoreConstants.Parameters.BMI, settings, snapshotContext);

         await addDiseaseStateParameters(settings, snapshot, snapshotContext);
         return settings;
      }

      private async Task addDiseaseStateParameters(RandomPopulationSettings randomPopulationSettings, PopulationSettings snapshot, SnapshotContext snapshotContext)
      {
         if (snapshot.DiseaseStateParameters == null)
            return;

         foreach (var snapshotDiseaseStateParameter in snapshot.DiseaseStateParameters)
         {
            await updateModelRange(snapshotDiseaseStateParameter, snapshotDiseaseStateParameter.Name, randomPopulationSettings, snapshotContext, isDiseaseStateParameter: true);
         }
      }

      private Task updateModelRange(ParameterRange parameterRange, string parameterName, RandomPopulationSettings randomPopulationSettings, SnapshotContext snapshotContext, bool isDiseaseStateParameter = false)
      {
         var modelParameterRange = randomPopulationSettings.ParameterRange(parameterName);
         if (modelParameterRange == null && isDiseaseStateParameter)
         {
            _logger.AddWarning(PKSimConstants.Warning.ParameterRangeNotFoundInPopulation(parameterName));
            return Task.CompletedTask;
         }

         return _parameterRangeMapper.MapToModel(parameterRange, new ParameterRangeSnapshotContext(modelParameterRange, snapshotContext));
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