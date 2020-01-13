using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeOntogenyVariabilityUpdater : ContextSpecification<IMoleculeOntogenyVariabilityUpdater>
   {
      protected IOntogenyRepository _ontogenyRepository;
      protected IndividualMolecule _molecule;
      protected Ontogeny _ontogeny;
      protected IEntityPathResolver _entityPathResolver;

      protected override void Context()
      {
         _molecule = new IndividualEnzyme
         {
            DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR),
            DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_GI)
         };

         _ontogeny = new DatabaseOntogeny();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _entityPathResolver = new EntityPathResolverForSpecs();
         sut = new MoleculeOntogenyVariabilityUpdater(_ontogenyRepository, _entityPathResolver);
      }
   }

   public class When_updating_the_ontogeny_factor_for_a_molecule_defined_in_an_individual : concern_for_MoleculeOntogenyVariabilityUpdater
   {
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM, _individual.OriginData, null)).Returns(10);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_LIVER, _individual.OriginData, null)).Returns(20);
      }

      protected override void Because()
      {
         sut.UpdateMoleculeOntogeny(_molecule, _ontogeny, _individual);
      }

      [Observation]
      public void should_set_the_factors_defined_in_the_database_for_liver_and_duodenum()
      {
         _molecule.OntogenyFactor.ShouldBeEqualTo(20);
         _molecule.OntogenyFactorParameter.DefaultValue.ShouldBeEqualTo(_molecule.OntogenyFactor);
         _molecule.OntogenyFactorGI.ShouldBeEqualTo(10);
         _molecule.OntogenyFactorGIParameter.DefaultValue.ShouldBeEqualTo(_molecule.OntogenyFactorGI);
      }
   }

   public class When_updating_the_ontogeny_factor_for_a_molecule_defined_in_a_population : concern_for_MoleculeOntogenyVariabilityUpdater
   {
      private Population _population;
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual {Root = new RootContainer()};
         var organism = new Organism();
         _individual.Add(organism);

         var ageParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.AGE);
         organism.Add(ageParameter);
         var gaParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.GESTATIONAL_AGE);
         organism.Add(gaParameter);

         var ageValues = new ParameterValues(_entityPathResolver.PathFor(ageParameter));
         ageValues.Add(1);
         ageValues.Add(2);

         var gaValues = new ParameterValues(_entityPathResolver.PathFor(gaParameter));
         gaValues.Add(30);
         gaValues.Add(40);

         _population = new RandomPopulation {Settings = new RandomPopulationSettings {BaseIndividual = _individual, NumberOfIndividuals = 2}};
         _population.IndividualValuesCache.Add(ageValues);
         _population.IndividualValuesCache.Add(gaValues);
         _population.IndividualValuesCache.IndividualIds.AddRange(new []{1, 2});

         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_LIVER, 1, 30, _population.RandomGenerator)).Returns(10);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_LIVER, 2, 40, _population.RandomGenerator)).Returns(11);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM, 1, 30, _population.RandomGenerator)).Returns(20);
         A.CallTo(() => _ontogenyRepository.OntogenyFactorFor(_ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM, 2, 40, _population.RandomGenerator)).Returns(21);
      }

      protected override void Because()
      {
         sut.UpdateMoleculeOntogeny(_molecule, _ontogeny, _population);
      }

      [Observation]
      public void should_set_the_factors_defined_in_the_database_for_liver_and_duodenum()
      {
         _population.AllValuesFor(_entityPathResolver.PathFor(_molecule.OntogenyFactorParameter)).ShouldOnlyContain(10, 11);
         _population.AllValuesFor(_entityPathResolver.PathFor(_molecule.OntogenyFactorGIParameter)).ShouldOnlyContain(20, 21);
      }
   }

   public class When_updating_all_ontogenies_for_a_population_not_using_the_species_human : concern_for_MoleculeOntogenyVariabilityUpdater
   {
      private Population _mousePopulation;

      protected override void Context()
      {
         base.Context();
         _mousePopulation = A.Fake<Population>();
         A.CallTo(() => _mousePopulation.IsHuman).Returns(false);
      }

      protected override void Because()
      {
         sut.UpdateAllOntogenies(_mousePopulation);
      }

      [Observation]
      public void should_not_try_to_update_the_ontogenies()
      {
         A.CallTo(() => _mousePopulation.AllOrganismValuesFor(A<string>._, _entityPathResolver)).MustNotHaveHappened();
      }
   }
}