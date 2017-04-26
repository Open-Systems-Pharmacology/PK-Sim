using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualFactory : ContextSpecification<IIndividualFactory>
   {
      protected ICreateIndividualAlgorithm _createIndvidualAlgorithm;
      protected IIndividualModelTask _individualModelTask;
      protected IObjectBaseFactory _entityBaseFactory;
      protected ISpeciesRepository _speciesRepository;
      protected IEntityValidator _entityValidator;
      private IReportGenerator _reportGenerator;
      private IMoleculeOntogenyVariabilityUpdater _moleculeOntogenyVariabilityUpdater;

      protected override void Context()
      {
         _createIndvidualAlgorithm = A.Fake<ICreateIndividualAlgorithm>();
         _entityBaseFactory = A.Fake<IObjectBaseFactory>();
         _individualModelTask = A.Fake<IIndividualModelTask>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _entityValidator = A.Fake<IEntityValidator>();
         _reportGenerator = A.Fake<IReportGenerator>();
         _moleculeOntogenyVariabilityUpdater = A.Fake<IMoleculeOntogenyVariabilityUpdater>();
         sut = new IndividualFactory(_individualModelTask, _entityBaseFactory, _createIndvidualAlgorithm, _speciesRepository, _entityValidator, _reportGenerator, _moleculeOntogenyVariabilityUpdater);
      }
   }

   public class When_creating_an_individual_for_the_predefined_origine_data : concern_for_IndividualFactory
   {
      private OriginData _originData;
      private Individual _individual;
      private Individual _result;
      private Organism _organism;
      private IContainer _neighborhoods;
      private IRootContainer _rootContainer;

      protected override void Context()
      {
         base.Context();
         _originData = new OriginData {Species = A.Fake<Species>().WithName("toto"), SpeciesPopulation = A.Fake<SpeciesPopulation>()};
         _individual = new Individual();
         _organism = A.Fake<Organism>();
         _neighborhoods = A.Fake<IContainer>();
         _rootContainer = new RootContainer();
         A.CallTo(() => _entityBaseFactory.Create<IRootContainer>()).Returns(_rootContainer);
         A.CallTo(() => _entityBaseFactory.Create<Individual>()).Returns(_individual);
         A.CallTo(() => _entityBaseFactory.Create<Organism>()).Returns(_organism);
         A.CallTo(() => _entityBaseFactory.Create<IContainer>()).Returns(_neighborhoods);
      }

      protected override void Because()
      {
         _result = sut.CreateAndOptimizeFor(_originData);
      }

      [Observation]
      public void should_create_a_standard_individual_based_on_the_predefined_value_for_the_orgine_data()
      {
         _result.ShouldBeEqualTo(_individual);
         _result.Neighborhoods.ShouldBeEqualTo(_neighborhoods);
         _result.Organism.ShouldBeEqualTo(_organism);
         A.CallTo(() => _individualModelTask.CreateModelFor(_individual)).MustHaveHappened();
      }

      [Observation]
      public void the_created_individual_should_have_an_organism_that_match_the_origine_data()
      {
         _result.ShouldBeEqualTo(_individual);
      }

      [Observation]
      public void should_set_the_icon_name_to_the_name_of_the_species_so_that_individuals_can_be_differentiated()
      {
         _result.Icon.ShouldBeEqualTo(_originData.Species.Name);
      }

      [Observation]
      public void should_use_the_registered_create_individual_algorithm_to_modify_the_standard_parameters()
      {
         A.CallTo(() => _createIndvidualAlgorithm.Optimize(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_individual_as_loaded()
      {
         _individual.IsLoaded.ShouldBeTrue();
      }

      [Observation]
      public void individual_should_have_a_random_seed()
      {
         _individual.Seed.ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_validate_the_created_individual()
      {
         A.CallTo(() => _entityValidator.Validate(_individual)).MustHaveHappened();
      }
   }

   public class When_creating_an_individual_for_the_predefined_origine_data_with_a_predefined_seed : concern_for_IndividualFactory
   {
      private OriginData _originData;
      private Individual _individual;
      private Individual _result;
      private int _seed;

      protected override void Context()
      {
         base.Context();
         _seed = 20;
         _originData = new OriginData {Species = A.Fake<Species>().WithName("toto"), SpeciesPopulation = A.Fake<SpeciesPopulation>()};
         _individual = new Individual();
         A.CallTo(() => _entityBaseFactory.Create<Individual>()).Returns(_individual);
      }

      protected override void Because()
      {
         _result = sut.CreateAndOptimizeFor(_originData, _seed);
      }

      [Observation]
      public void should_have_used_the_predefined_seed()
      {
         _result.Seed.ShouldBeEqualTo(_seed);
      }

      public void should_use_the_registered_create_individual_algorithm_to_modify_the_standard_parameters()
      {
         A.CallTo(() => _createIndvidualAlgorithm.Optimize(_individual)).MustHaveHappened();
      }
   }

   public class When_told_to_create_an_individual_without_parameters : concern_for_IndividualFactory
   {
      private Individual _individual;
      private Individual _result;
      private Organism _organism;
      private IContainer _neighborhoods;
      private IRootContainer _rootContainer;

      protected override void Context()
      {
         base.Context();
         var species = new Species {Name = CoreConstants.Species.Human, Id = CoreConstants.Species.Human};
         species.AddPopulation(new SpeciesPopulation {Name = CoreConstants.Population.ICRP});
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {species});
         _individual = new Individual();
         _organism = A.Fake<Organism>();
         _neighborhoods = A.Fake<IContainer>();
         _rootContainer = new RootContainer();
         A.CallTo(() => _entityBaseFactory.Create<IRootContainer>()).Returns(_rootContainer);
         A.CallTo(() => _entityBaseFactory.Create<Individual>()).Returns(_individual);
         A.CallTo(() => _entityBaseFactory.Create<Organism>()).Returns(_organism);
         A.CallTo(() => _entityBaseFactory.Create<IContainer>()).Returns(_neighborhoods);
      }

      protected override void Because()
      {
         _result = sut.CreateParameterLessIndividual();
      }

      [Observation]
      public void should_return_an_individual_for_the_given_species_without_parameters()
      {
         A.CallTo(() => _individualModelTask.CreateModelStructureFor(_individual)).MustHaveHappened();
      }
   }

   public class When_creating_an_individual_for_the_predefined_origine_data_that_is_not_valid : concern_for_IndividualFactory
   {
      private OriginData _originData;
      private Individual _individual;
      private Organism _organism;
      private IContainer _neighborhoods;
      private IRootContainer _rootContainer;
      private ValidationResult _invalidResults;

      protected override void Context()
      {
         base.Context();
         _originData = new OriginData {Species = A.Fake<Species>().WithName("toto"), SpeciesPopulation = A.Fake<SpeciesPopulation>()};
         _individual = new Individual();
         _invalidResults = A.Fake<ValidationResult>();
         A.CallTo(() => _invalidResults.ValidationState).Returns(ValidationState.Invalid);
         _organism = A.Fake<Organism>();
         _neighborhoods = A.Fake<IContainer>();
         _rootContainer = new RootContainer();
         A.CallTo(() => _entityBaseFactory.Create<IRootContainer>()).Returns(_rootContainer);
         A.CallTo(() => _entityBaseFactory.Create<Individual>()).Returns(_individual);
         A.CallTo(() => _entityBaseFactory.Create<Organism>()).Returns(_organism);
         A.CallTo(() => _entityBaseFactory.Create<IContainer>()).Returns(_neighborhoods);
         A.CallTo(() => _entityValidator.Validate(_individual)).Returns(_invalidResults);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.CreateAndOptimizeFor(_originData)).ShouldThrowAn<CannotCreateIndividualWithConstraintsException>();
      }
   }
}