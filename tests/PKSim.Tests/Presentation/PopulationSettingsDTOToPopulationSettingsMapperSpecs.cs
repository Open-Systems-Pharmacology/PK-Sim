using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Populations;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationSettingsDTOToPopulationSettingsMapper : ContextSpecification<IPopulationSettingsDTOMapper>
   {
      protected ICloner _cloneManager;
      protected IIndividualToPopulationSettingsMapper _individualToPopulationSettingsMapper;

      protected override void Context()
      {
         _cloneManager = A.Fake<ICloner>();
         _individualToPopulationSettingsMapper = A.Fake<IIndividualToPopulationSettingsMapper>();

         sut = new PopulationSettingsDTOMapper(_individualToPopulationSettingsMapper, _cloneManager);
      }
   }

   public class When_mapping_a_population_settings_dto_to_a_population_settings : concern_for_PopulationSettingsDTOToPopulationSettingsMapper
   {
      private PopulationSettingsDTO _populationSettingsDTO;
      private RandomPopulationSettings _result;
      private Gender _male;
      private Gender _female;
      private Individual _cloneIndividual;

      protected override void Context()
      {
         base.Context();
         _male = new Gender().WithName(CoreConstants.Gender.MALE);
         _female = new Gender().WithName(CoreConstants.Gender.FEMALE);
         _populationSettingsDTO = new PopulationSettingsDTO
         {
            Individual = A.Fake<Individual>()
         };
         _cloneIndividual = A.Fake<Individual>();
         A.CallTo(() => _cloneManager.Clone(_populationSettingsDTO.Individual)).Returns(_cloneIndividual);
         A.CallTo(() => _populationSettingsDTO.Individual.AvailableGenders).Returns(new[] {_male, _female,});
         _populationSettingsDTO.NumberOfIndividuals = 150;
         _populationSettingsDTO.Parameters.Add(new ParameterRangeDTO(new ParameterRange {ParameterName = "P1"}));
         _populationSettingsDTO.Parameters.Add(new ParameterRangeDTO(new ParameterRange {ParameterName = "P2"}));
         _populationSettingsDTO.ProportionOfFemales = 80;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_populationSettingsDTO);
      }

      [Observation]
      public void should_return_a_population_settings_with_the_accurate_gender_distribution()
      {
         _result.GenderRatios.Count().ShouldBeEqualTo(2);
         _result.GenderRatio(_male).Ratio.ShouldBeEqualTo(20);
         _result.GenderRatio(_female).Ratio.ShouldBeEqualTo(80);
      }

      [Observation]
      public void should_return_a_population_settings_with_containing_the_parameter_ranges_defined_by_the_user()
      {
         _result.ParameterRanges.ShouldOnlyContain(_populationSettingsDTO.Parameters.Select(x => x.ParameterRange));
      }

      [Observation]
      public void should_have_set_the_based_individual_as_a_clone_into_the_resulting_settings()
      {
         _result.BaseIndividual.ShouldBeEqualTo(_cloneIndividual);
      }
   }

   public class When_mapping_population_settings_with_only_male_to_a_population_dto_settings : concern_for_PopulationSettingsDTOToPopulationSettingsMapper
   {
      private RandomPopulationSettings _settings;
      private PopulationSettingsDTO _settingsDTO;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         Gender male = new Gender {Name = CoreConstants.Gender.MALE};
         Gender female = new Gender {Name = CoreConstants.Gender.FEMALE};
         A.CallTo(() => _individual.AvailableGenders).Returns(new[] {male, female});
         _settings = new RandomPopulationSettings
         {
            BaseIndividual = _individual
         };
         _settings.AddGenderRatio(new GenderRatio {Gender = male, Ratio = 100});
         _settings.AddGenderRatio(new GenderRatio {Gender = female, Ratio = 0});
      }

      protected override void Because()
      {
         _settingsDTO = sut.MapFrom(_settings);
      }

      [Observation]
      public void should_return_a_population_dto_containing_only_males()
      {
         _settingsDTO.ProportionOfFemales.ShouldBeEqualTo(0);
      }
   }

   public class When_mapping_an_individual_to_some_population_settings_dto : concern_for_PopulationSettingsDTOToPopulationSettingsMapper
   {
      private Individual _individual;
      private RandomPopulationSettings _settings;
      private PopulationSettingsDTO _settingsDTO;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _settings = new RandomPopulationSettings();
         A.CallTo(() => _individualToPopulationSettingsMapper.MapFrom(_individual)).Returns(_settings);
         var male = new Gender {Name = CoreConstants.Gender.MALE};
         var female = new Gender {Name = CoreConstants.Gender.FEMALE};
         _settings.BaseIndividual = _individual;
         _settings.AddGenderRatio(new GenderRatio {Gender = male, Ratio = 100});
         _settings.AddGenderRatio(new GenderRatio {Gender = female, Ratio = 0});
      }

      protected override void Because()
      {
         _settingsDTO = sut.MapFrom(_individual);
      }

      [Observation]
      public void should_return_the_settings_based_on_the_dto()
      {
         _settingsDTO.ProportionOfFemales.ShouldBeEqualTo(0);
      }
   }
}