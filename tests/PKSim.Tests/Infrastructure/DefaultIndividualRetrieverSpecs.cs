using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_DefaultIndividualRetriever : ContextSpecification<IDefaultIndividualRetriever>
   {
      private ISpeciesRepository _speciesRepository;
      protected IIndividualFactory _individualFactory;
      protected IIndividualSettingsDTOToOriginDataMapper _insividualSettingsMapper;
      protected IIndividualDefaultValueRetriever _individualDefaultValueRetriever;

      protected override void Context()
      {
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _insividualSettingsMapper = A.Fake<IIndividualSettingsDTOToOriginDataMapper>();
         _individualDefaultValueRetriever = A.Fake<IIndividualDefaultValueRetriever>();

         sut = new DefaultIndividualRetriever(_speciesRepository, _individualFactory, _insividualSettingsMapper, _individualDefaultValueRetriever);
      }
   }

   public class When_creating_a_default_individual_for_a_given_species : concern_for_DefaultIndividualRetriever
   {
      private Individual _result;
      private Species _species;
      private SpeciesPopulation _defaultPopulation;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _species = A.Fake<Species>();
         _defaultPopulation = new SpeciesPopulation().WithName("POP");
         A.CallTo(() => _species.DefaultPopulation).Returns(_defaultPopulation);

         var individualSettingsDTO = new IndividualSettingsDTO();
         A.CallTo(() => _individualDefaultValueRetriever.DefaultSettingFor(_defaultPopulation)).Returns(individualSettingsDTO);

         var originData = new OriginData();
         A.CallTo(() => _insividualSettingsMapper.MapFrom(individualSettingsDTO)).Returns(originData);

         _individual = new Individual();
         A.CallTo(() => _individualFactory.CreateStandardFor(originData)).Returns(_individual);
      }

      protected override void Because()
      {
         _result = sut.DefaultIndividualFor(_species);
      }

      [Observation]
      public void should_leverage_teh_individual_factory_to_create_an_individual_based_on_the_species_and_its_default_population()
      {
         _result.ShouldBeEqualTo(_individual);
      }
   }
}