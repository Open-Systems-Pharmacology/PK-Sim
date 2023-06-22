using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeOntogenyVariabilityUpdater : ContextSpecification<IMoleculeOntogenyVariabilityUpdater>
   {
      protected IOntogenyRepository _ontogenyRepository;
      protected IndividualMolecule _molecule;
      protected Ontogeny _ontogeny;
      protected IEntityPathResolver _entityPathResolver;
      protected IFormulaFactory _formulaFactory;

      protected override void Context()
      {
         _molecule = new IndividualEnzyme
         {
            DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR),
            DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_TABLE),
            DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_GI),
            DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_GI_TABLE),
         };

         _ontogeny = new DatabaseOntogeny();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         _formulaFactory= A.Fake<IFormulaFactory>();
         _entityPathResolver = new EntityPathResolverForSpecs();
         sut = new MoleculeOntogenyVariabilityUpdater(_ontogenyRepository, _entityPathResolver, _formulaFactory);
      }
   }

   public class When_updating_the_ontogeny_factor_for_a_molecule_defined_in_an_individual : concern_for_MoleculeOntogenyVariabilityUpdater
   {
      private Individual _individual;
      private readonly DistributedTableFormula _duoTable = new DistributedTableFormula();
      private readonly DistributedTableFormula _liverTable = new DistributedTableFormula();

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         A.CallTo(() => _ontogenyRepository.OntogenyToDistributedTableFormula(_ontogeny, CoreConstants.Groups.ONTOGENY_DUODENUM)).Returns(_duoTable);
         A.CallTo(() => _ontogenyRepository.OntogenyToDistributedTableFormula(_ontogeny, CoreConstants.Groups.ONTOGENY_LIVER)).Returns(_liverTable);
      }

      protected override void Because()
      {
         sut.UpdateMoleculeOntogeny(_molecule, _ontogeny, _individual);
      }

      [Observation]
      public void should_set_the_factors_defined_in_the_database_for_liver_and_duodenum()
      {
         _molecule.OntogenyFactorTableParameter.Formula.ShouldBeEqualTo(_liverTable);
         _molecule.OntogenyFactorGITableParameter.Formula.ShouldBeEqualTo(_duoTable);
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