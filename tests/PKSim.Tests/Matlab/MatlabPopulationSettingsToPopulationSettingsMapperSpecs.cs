using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Matlab.Mappers;

namespace PKSim.Matlab
{
   public abstract class concern_for_MatlabPopulationSettingsToPopulationSettingsMapper : ContextSpecification<IMatlabPopulationSettingsToPopulationSettingsMapper>
   {
      private IMatlabOriginDataToOriginDataMapper _orginDataMapper;
      protected IIndividualFactory _individualFactory;
      protected IIndividualToPopulationSettingsMapper _popSettingsMapper;
      protected IGenderRepository _genderRepository;
      private Gender _female;
      private Gender _male;

      protected override void Context()
      {
         _orginDataMapper = A.Fake<IMatlabOriginDataToOriginDataMapper>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _popSettingsMapper = A.Fake<IIndividualToPopulationSettingsMapper>();
         _genderRepository = A.Fake<IGenderRepository>();
         _female = new Gender();
         _male = new Gender();
         A.CallTo(() => _genderRepository.Female).Returns(_female);
         A.CallTo(() => _genderRepository.Male).Returns(_male);
         sut = new MatlabPopulationSettingsToPopulationSettingsMapper(_orginDataMapper, _individualFactory, _popSettingsMapper, _genderRepository);
      }
   }

   public class When_mapping_the_batch_population_settings_to_some_population_settings : concern_for_MatlabPopulationSettingsToPopulationSettingsMapper
   {
      private Individual _individual;
      private RandomPopulationSettings _populationSettings;
      private PopulationSettings _batchPopSettings;
      private RandomPopulationSettings _result;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         A.CallTo(() => _individual.AvailableGenders()).Returns(new List<Gender> {_genderRepository.Male, _genderRepository.Female});
         _populationSettings = new RandomPopulationSettings();
         _batchPopSettings = new PopulationSettings();
         _batchPopSettings.Population = "Population";
         _batchPopSettings.MinAge = 25;
         _batchPopSettings.MaxAge = 40;
         _batchPopSettings.MinGestationalAge = 30;
         _batchPopSettings.MinHeight = 10;
         _batchPopSettings.MaxHeight = 20;
         _batchPopSettings.MinWeight = double.NaN;
         _batchPopSettings.MaxWeight = 40;
         _batchPopSettings.MinBMI = 8;
         _batchPopSettings.MaxBMI = double.NaN;
         _batchPopSettings.NumberOfIndividuals = 2589;
         _batchPopSettings.ProportionOfFemales = 40;
         _populationSettings.AddParameterRange(new ParameterRange {ParameterName = CoreConstants.Parameter.BMI});
         _populationSettings.AddParameterRange(new ParameterRange {ParameterName = CoreConstants.Parameter.AGE});
         _populationSettings.AddParameterRange(new ParameterRange {ParameterName = CoreConstants.Parameter.MEAN_HEIGHT});
         _populationSettings.AddParameterRange(new ParameterRange {ParameterName = CoreConstants.Parameter.MEAN_WEIGHT});
         _populationSettings.AddGenderRatio(new GenderRatio {Gender = _genderRepository.Male});
         _populationSettings.AddGenderRatio(new GenderRatio {Gender = _genderRepository.Female});
         A.CallTo(() => _individualFactory.CreateStandardFor(A<Core.Model.OriginData>._)).Returns(_individual);
         A.CallTo(() => _popSettingsMapper.MapFrom(_individual)).Returns(_populationSettings);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_batchPopSettings);
      }

      [Observation]
      public void should_create_an_individual_based_on_the_default_origin_data()
      {
         _result.ShouldBeEqualTo(_populationSettings);
      }

      [Observation]
      public void should_update_the_number_of_individual()
      {
         _result.NumberOfIndividuals.ShouldBeEqualTo(_batchPopSettings.NumberOfIndividuals);
      }

      [Observation]
      public void should_update_the_min_max_for_height()
      {
         _result.ParameterRange(CoreConstants.Parameter.MEAN_HEIGHT).MinValue.ShouldBeEqualTo(_batchPopSettings.MinHeight);
         _result.ParameterRange(CoreConstants.Parameter.MEAN_HEIGHT).MaxValue.ShouldBeEqualTo(_batchPopSettings.MaxHeight);
      }

      [Observation]
      public void should_update_the_min_max_for_weight()
      {
         _result.ParameterRange(CoreConstants.Parameter.MEAN_WEIGHT).MinValue.ShouldBeNull();
         _result.ParameterRange(CoreConstants.Parameter.MEAN_WEIGHT).MaxValue.ShouldBeEqualTo(_batchPopSettings.MaxWeight);
      }

      [Observation]
      public void should_update_the_min_max_for_age()
      {
         _result.ParameterRange(CoreConstants.Parameter.AGE).MinValue.ShouldBeEqualTo(_batchPopSettings.MinAge);
         _result.ParameterRange(CoreConstants.Parameter.AGE).MaxValue.ShouldBeEqualTo(_batchPopSettings.MaxAge);
      }

      public void should_update_the_min_max_for_gestational_age()
      {
         _result.ParameterRange(CoreConstants.Parameter.GESTATIONAL_AGE).MinValue.ShouldBeEqualTo(_batchPopSettings.MinGestationalAge);

         //because not explicitely set: max_GA should be equal to the max of the range
         _result.ParameterRange(CoreConstants.Parameter.GESTATIONAL_AGE).MaxValue.ShouldBeEqualTo(CoreConstants.PRETERM_RANGE.Max());
      }

      [Observation]
      public void should_update_the_min_max_for_bmi()
      {
         _result.ParameterRange(CoreConstants.Parameter.BMI).MinValue.ShouldBeEqualTo(_batchPopSettings.MinBMI);
         _result.ParameterRange(CoreConstants.Parameter.BMI).MaxValue.ShouldBeNull();
      }

      [Observation]
      public void should_update_the_gender_ration()
      {
         _result.GenderRatio(_genderRepository.Male).Ratio.ShouldBeEqualTo(60);
         _result.GenderRatio(_genderRepository.Female).Ratio.ShouldBeEqualTo(40);
      }
   }
}