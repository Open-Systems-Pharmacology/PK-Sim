using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualToPopulationSettingsDTOMapper : ContextSpecification<IIndividualToPopulationSettingsMapper>
   {
      private IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new IndividualToPopulationSettingsMapper(_representationInfoRepository);
      }
   }

   public class When_mapping_an_individual_to_a_population_settings_dto_when_the_individual_is_not_defined : concern_for_IndividualToPopulationSettingsDTOMapper
   {
      private RandomPopulationSettings _result;

      protected override void Because()
      {
         _result = sut.MapFrom(null);
      }

      [Observation]
      public void should_return_an_default_population_settings_dto()
      {
         _result.BaseIndividual.ShouldBeNull();
      }
   }

   public class When_creating_a_population_settings_for_a_preterm_individual : concern_for_IndividualToPopulationSettingsDTOMapper
   {
      private RandomPopulationSettings _result;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         var originData = new OriginData();
         originData.SpeciesPopulation = new SpeciesPopulation();
         A.CallTo(() => _individual.OriginData).Returns(originData);
         var organism = new Organism();
         var ga = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameters.GESTATIONAL_AGE);
         ga.Info.MinValue = 24;
         ga.Info.MaxValue = 25;
         organism.Add(ga);
         organism.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameters.MEAN_WEIGHT));
         A.CallTo(() => _individual.IsPreterm).Returns(true);
         A.CallTo(() => _individual.Organism).Returns(organism);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_individual);
      }

      [Observation]
      public void should_add_a_range_for_gestational_age()
      {
         _result.ParameterRange(CoreConstants.Parameters.GESTATIONAL_AGE).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_list_of_values_for_the_gestational_age()
      {
         var discreteRange = _result.ParameterRange(CoreConstants.Parameters.GESTATIONAL_AGE).DowncastTo<DiscreteParameterRange>();
         discreteRange.ListOfValues.ShouldOnlyContain(24, 25);
      }
   }
}