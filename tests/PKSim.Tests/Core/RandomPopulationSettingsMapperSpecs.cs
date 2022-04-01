using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Individual = PKSim.Core.Model.Individual;
using OriginData = PKSim.Core.Model.OriginData;
using ParameterRange = PKSim.Core.Model.ParameterRange;

namespace PKSim.Core
{
   public abstract class concern_for_RandomPopulationSettingsMapper : ContextSpecificationAsync<RandomPopulationSettingsMapper>
   {
      protected ParameterRangeMapper _parameterRangeMapper;
      protected IndividualMapper _individualMapper;
      protected IIndividualToPopulationSettingsMapper _populationSettingsMapper;
      protected IGenderRepository _genderRepository;
      protected ParameterRange _ageParameterRange;
      protected ParameterRange _weightParameterRange;
      protected ParameterRange _diseaseStateParameterRange;
      protected Snapshots.ParameterRange _ageRangeSnapshot;
      protected Snapshots.ParameterRange _weightRangeSnapshot;
      protected Snapshots.ParameterRange _diseaseStateParameterRangeSnapshot;
      protected Individual _baseIndividual;
      protected Snapshots.Individual _snapshotIndividual;
      protected RandomPopulationSettings _randomPopulationSettings;
      protected PopulationSettings _snapshot;
      protected OriginDataParameter _diseaseStateParameter;

      protected override Task Context()
      {
         _individualMapper = A.Fake<IndividualMapper>();
         _parameterRangeMapper = A.Fake<ParameterRangeMapper>();
         _genderRepository = A.Fake<IGenderRepository>();
         _populationSettingsMapper = A.Fake<IIndividualToPopulationSettingsMapper>();
         sut = new RandomPopulationSettingsMapper(_parameterRangeMapper, _individualMapper, _populationSettingsMapper, _genderRepository);

         _ageParameterRange = new ConstrainedParameterRange {ParameterName = CoreConstants.Parameters.AGE};
         _weightParameterRange = new ParameterRange {ParameterName = CoreConstants.Parameters.MEAN_WEIGHT};

         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(null)).Returns((Snapshots.ParameterRange) null);
         _ageRangeSnapshot = new Snapshots.ParameterRange();
         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(_ageParameterRange)).Returns(_ageRangeSnapshot);

         _weightRangeSnapshot = new Snapshots.ParameterRange();
         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(_weightParameterRange)).Returns(_weightRangeSnapshot);


         _baseIndividual = new Individual
         {
            OriginData = new OriginData()
         };
         _diseaseStateParameter = new OriginDataParameter(10, "mg/kg", "DIS_STATE_PARAM");
         _baseIndividual.OriginData.AddDiseaseStateParameter(_diseaseStateParameter);

         _diseaseStateParameterRangeSnapshot = new Snapshots.ParameterRange();
         _diseaseStateParameterRange = new ConstrainedParameterRange {ParameterName = _diseaseStateParameter.Name};
         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(_diseaseStateParameterRange)).Returns(_diseaseStateParameterRangeSnapshot);


         _snapshotIndividual = new Snapshots.Individual();
         A.CallTo(() => _individualMapper.MapToSnapshot(_baseIndividual)).Returns(_snapshotIndividual);

         _randomPopulationSettings = new RandomPopulationSettings
         {
            NumberOfIndividuals = 10,
            BaseIndividual = _baseIndividual
         };

         _randomPopulationSettings.AddParameterRange(_weightParameterRange);
         _randomPopulationSettings.AddParameterRange(_ageParameterRange);
         _randomPopulationSettings.AddParameterRange(_diseaseStateParameterRange);

         A.CallTo(() => _genderRepository.Female).Returns(new Gender {Id = "Female", Name = "Female"});
         A.CallTo(() => _genderRepository.Male).Returns(new Gender {Id = "Male", Name = "Male"});
         return Task.FromResult(true);
      }
   }

   public class When_mapping_a_random_population_settings_to_snapshot : concern_for_RandomPopulationSettingsMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_randomPopulationSettings);
      }

      [Observation]
      public void should_save_the_base_individual()
      {
         _snapshot.Individual.ShouldBeEqualTo(_snapshotIndividual);
      }

      [Observation]
      public void should_save_the_available_ranges()
      {
         _snapshot.Age.ShouldBeEqualTo(_ageRangeSnapshot);
         _snapshot.Weight.ShouldBeEqualTo(_weightRangeSnapshot);
      }

      [Observation]
      public void should_set_all_other_ranges_to_null()
      {
         _snapshot.Height.ShouldBeNull();
         _snapshot.GestationalAge.ShouldBeNull();
         _snapshot.BMI.ShouldBeNull();
      }

      [Observation]
      public void should_have_added_named_disease_state_parameters_when_available()
      {
         _snapshot.DiseaseStateParameters.ShouldNotBeNull();
         _snapshot.DiseaseStateParameters.ShouldContain(_diseaseStateParameterRangeSnapshot);
         _diseaseStateParameterRangeSnapshot.Name.ShouldBeEqualTo(_diseaseStateParameterRange.ParameterName);
      }
   }

   public class When_mapping_a_valid_population_settings_snapshot_to_a_random_population_settings_with_gender : concern_for_RandomPopulationSettingsMapper
   {
      private ParameterRange _newAgeRange;
      private ParameterRange _newWeightRange;
      private Individual _newIndividual;
      private RandomPopulationSettings _newSettings;
      private RandomPopulationSettings _mappedSettings;
      private GenderRatio _maleRatio;
      private GenderRatio _femaleRatio;
      private readonly int _proportionOfFemale = 30;
      private PKSimProject _project;

      protected override async Task Context()
      {
         await base.Context();
         _project = new PKSimProject();
         _newIndividual = new Individual();
         _newAgeRange = new ParameterRange {ParameterName = CoreConstants.Parameters.AGE};
         _newWeightRange = new ParameterRange {ParameterName = CoreConstants.Parameters.MEAN_WEIGHT};

         _snapshot = await sut.MapToSnapshot(_randomPopulationSettings);

         _snapshot.ProportionOfFemales = _proportionOfFemale;

         A.CallTo(() => _parameterRangeMapper.MapToModel(_snapshot.Age, _newAgeRange)).Returns(_newAgeRange);
         A.CallTo(() => _parameterRangeMapper.MapToModel(_snapshot.Weight, _newWeightRange)).Returns(_newWeightRange);
         A.CallTo(() => _individualMapper.MapToModel(_snapshotIndividual, _project)).Returns(_newIndividual);

         _mappedSettings = new RandomPopulationSettings();
         A.CallTo(() => _populationSettingsMapper.MapFrom(_newIndividual)).Returns(_mappedSettings);
         _maleRatio = new GenderRatio
         {
            Gender = _genderRepository.Male,
            Ratio = 50
         };

         _femaleRatio = new GenderRatio
         {
            Gender = _genderRepository.Female,
            Ratio = 50
         };

         _mappedSettings.AddGenderRatio(_maleRatio);
         _mappedSettings.AddGenderRatio(_femaleRatio);

         _snapshot.DiseaseStateParameters = null;
      }

      protected override async Task Because()
      {
         _newSettings = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_have_created_a_random_population_setting_based_on_the_snapshot_individual()
      {
         _newSettings.ShouldBeEqualTo(_mappedSettings);
      }

      [Observation]
      public void should_have_updated_the_gender_ratios()
      {
         _femaleRatio.Ratio.ShouldBeEqualTo(_proportionOfFemale);
         _maleRatio.Ratio.ShouldBeEqualTo(100 - _proportionOfFemale);
      }

      [Observation]
      public void should_have_updated_the_parameter_as_expected()
      {
         A.CallTo(() => _parameterRangeMapper.MapToModel(_snapshot.Age, _newSettings.ParameterRange(CoreConstants.Parameters.AGE))).MustHaveHappened();
         A.CallTo(() => _parameterRangeMapper.MapToModel(_snapshot.Height, _newSettings.ParameterRange(CoreConstants.Parameters.MEAN_WEIGHT))).MustHaveHappened();
      }
   }

   public class When_mapping_a_valid_population_settings_snapshot_to_a_random_population_settings_with_disease_state_parameters : concern_for_RandomPopulationSettingsMapper
   {
      private ParameterRange _newAgeRange;
      private ParameterRange _newWeightRange;
      private Individual _newIndividual;
      private RandomPopulationSettings _newSettings;
      private RandomPopulationSettings _mappedSettings;
      private readonly int _proportionOfFemale = 30;
      private PKSimProject _project;
      private ParameterRange _newDiseaseStateParameterRange;

      protected override async Task Context()
      {
         await base.Context();
         _project = new PKSimProject();
         _newIndividual = new Individual
         {
            OriginData = new OriginData()
         };
         _newIndividual.OriginData.AddDiseaseStateParameter(_diseaseStateParameter);

         _newAgeRange = new ParameterRange {ParameterName = CoreConstants.Parameters.AGE};
         _newWeightRange = new ParameterRange {ParameterName = CoreConstants.Parameters.MEAN_WEIGHT};

         _snapshot = await sut.MapToSnapshot(_randomPopulationSettings);
         _snapshot.ProportionOfFemales = _proportionOfFemale;

         A.CallTo(() => _parameterRangeMapper.MapToModel(_snapshot.Age, _newAgeRange)).Returns(_newAgeRange);
         A.CallTo(() => _parameterRangeMapper.MapToModel(_snapshot.Weight, _newWeightRange)).Returns(_newWeightRange);
         A.CallTo(() => _individualMapper.MapToModel(_snapshotIndividual, _project)).Returns(_newIndividual);

         var diseaseStateParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(_diseaseStateParameter.Name);
         _newIndividual.OriginData.DiseaseState = new DiseaseState { diseaseStateParameter };

         _newDiseaseStateParameterRange = new ConstrainedParameterRange {ParameterName = _diseaseStateParameter.Name};
         A.CallTo(() => _populationSettingsMapper.ConstrainedParameterRangeFrom(diseaseStateParameter)).Returns(_newDiseaseStateParameterRange);

         _mappedSettings = new RandomPopulationSettings {BaseIndividual = _newIndividual};
         A.CallTo(() => _populationSettingsMapper.MapFrom(_newIndividual)).Returns(_mappedSettings);
      }

      protected override async Task Because()
      {
         _newSettings = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_have_created_a_random_population_setting_based_on_the_snapshot_individual()
      {
         _newSettings.ShouldBeEqualTo(_mappedSettings);
      }

      [Observation]
      public void should_have_added_dynamic_parameters()
      {
         _newSettings.ParameterRange(_diseaseStateParameter.Name).ShouldBeEqualTo(_newDiseaseStateParameterRange);
      }
   }

   public class When_mapping_a_valid_population_settings_snapshot_to_a_random_population_settings_without_gender : concern_for_RandomPopulationSettingsMapper
   {
      private Individual _newIndividual;
      private RandomPopulationSettings _newSettings;
      private RandomPopulationSettings _mappedSettings;
      private GenderRatio _unknownGender;
      private PKSimProject _project;

      protected override async Task Context()
      {
         await base.Context();
         _newIndividual = new Individual();
         _project = new PKSimProject();

         _snapshot = await sut.MapToSnapshot(_randomPopulationSettings);
         _snapshot.ProportionOfFemales = null;
         A.CallTo(() => _individualMapper.MapToModel(_snapshotIndividual, _project)).Returns(_newIndividual);

         _mappedSettings = new RandomPopulationSettings();
         A.CallTo(() => _populationSettingsMapper.MapFrom(_newIndividual)).Returns(_mappedSettings);
         _unknownGender = new GenderRatio
         {
            Gender = new Gender {Id = "Gender"},
            Ratio = 100
         };
         _mappedSettings.AddGenderRatio(_unknownGender);
      }

      protected override async Task Because()
      {
         _newSettings = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_have_created_a_random_population_setting_based_on_the_snapshot_individual()
      {
         _newSettings.ShouldBeEqualTo(_mappedSettings);
      }

      [Observation]
      public void should_not_update_the_gender_ratio()
      {
         _unknownGender.Ratio.ShouldBeEqualTo(100);
      }
   }
}