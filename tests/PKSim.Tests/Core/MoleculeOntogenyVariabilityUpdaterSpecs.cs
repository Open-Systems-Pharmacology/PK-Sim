using System;
using System.Linq.Expressions;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
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
      protected IDiseaseStateImplementationRepository _diseaseStateImplementationRepository;

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
         _formulaFactory = A.Fake<IFormulaFactory>();
         _entityPathResolver = new EntityPathResolverForSpecs();
         _diseaseStateImplementationRepository = A.Fake<IDiseaseStateImplementationRepository>();
         A.CallTo(() => _diseaseStateImplementationRepository.FindFor(A<DiseaseState>._)).Returns(new HealthyDiseaseStateImplementation());
         sut = new MoleculeOntogenyVariabilityUpdater(_ontogenyRepository, _entityPathResolver, _formulaFactory, _diseaseStateImplementationRepository);
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
         _population.IndividualValuesCache.IndividualIds.AddRange(new[] {1, 2});

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
         A.CallTo((Expression<Action>) (() => _mousePopulation.AllOrganismValuesFor(A<string>._, _entityPathResolver))).MustNotHaveHappened();
      }
   }

   public class When_updating_the_plasma_protein_ontogeny_for_a_population_with_a_disease_state_scaling_the_plasma_proteins : concern_for_MoleculeOntogenyVariabilityUpdater
   {
      private Population _population;
      private IParameter _ontogenyFactorAgp;

      protected override void Context()
      {
         base.Context();
         var individual = new Individual {Root = new RootContainer(), OriginData = new OriginData {Species = new Species().WithName("Human")}};
         var organism = new Organism();
         individual.Add(organism);
         var ageParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.AGE);
         var gaParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(Constants.Parameters.GESTATIONAL_AGE);
         _ontogenyFactorAgp = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP);
         organism.Add(ageParameter);
         organism.Add(gaParameter);
         organism.Add(_ontogenyFactorAgp);

         var ageValues = new ParameterValues(_entityPathResolver.PathFor(ageParameter));
         ageValues.Add(30);
         ageValues.Add(40);

         var gaValues = new ParameterValues(_entityPathResolver.PathFor(gaParameter));
         gaValues.Add(40);
         gaValues.Add(40);

         _population = new RandomPopulation {Settings = new RandomPopulationSettings {BaseIndividual = individual, NumberOfIndividuals = 2}};
         _population.IndividualValuesCache.Add(ageValues);
         _population.IndividualValuesCache.Add(gaValues);
         _population.IndividualValuesCache.IndividualIds.AddRange(new[] {1, 2});

         var agpProtein = new SupportedProtein(CoreConstants.Molecule.AGP, CoreConstants.Parameters.ONTOGENY_FACTOR_AGP, CoreConstants.Parameters.ONTOGENY_FACTOR_AGP_TABLE);
         A.CallTo(() => _ontogenyRepository.SupportedProteins).Returns(new[] {agpProtein});
         A.CallTo(() => _ontogenyRepository.PlasmaProteinOntogenyValueFor(agpProtein.Name, 30, 40, A<string>._, _population.RandomGenerator)).Returns(2);
         A.CallTo(() => _ontogenyRepository.PlasmaProteinOntogenyValueFor(agpProtein.Name, 40, 40, A<string>._, _population.RandomGenerator)).Returns(3);

         var diseaseStateImplementation = A.Fake<IDiseaseStateImplementation>();
         A.CallTo(() => diseaseStateImplementation.PlasmaProteinOntogenyFactorFor(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP, _population.OriginData)).Returns(0.5);
         A.CallTo(() => _diseaseStateImplementationRepository.FindFor(A<DiseaseState>._)).Returns(diseaseStateImplementation);
      }

      protected override void Because()
      {
         sut.UpdatePlasmaProteinsOntogenyFor(_population);
      }

      [Observation]
      public void should_scale_the_ontogeny_factor_sampled_for_each_individual_by_the_disease_state_factor()
      {
         _population.AllValuesFor(_entityPathResolver.PathFor(_ontogenyFactorAgp)).ShouldOnlyContain(1, 1.5);
      }
   }

   public class When_updating_the_plasma_protein_ontogeny_for_a_given_individual : concern_for_MoleculeOntogenyVariabilityUpdater
   {
      private ISimulationSubject _individual;
      private SupportedProtein _agpProtein;
      private IParameter _ontogenyFactorAgpTable;
      private DistributedTableFormula _tableFormula;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual();
         _tableFormula = new DistributedTableFormula();
         var organism = new Organism();
         _individual.Add(organism);
         _ontogenyFactorAgpTable = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP_TABLE);
         organism.Add(_ontogenyFactorAgpTable);

         _agpProtein = new SupportedProtein(CoreConstants.Molecule.AGP, CoreConstants.Parameters.ONTOGENY_FACTOR_AGP, CoreConstants.Parameters.ONTOGENY_FACTOR_AGP_TABLE);
         A.CallTo(() => _ontogenyRepository.SupportedProteins).Returns(new[] {_agpProtein});

         A.CallTo(() => _ontogenyRepository.PlasmaProteinOntogenyTableFormula(_agpProtein.Name, _individual.OriginData)).Returns(_tableFormula);
      }

      protected override void Because()
      {
         sut.UpdatePlasmaProteinsOntogenyFor(_individual);
      }

      [Observation]
      public void should_ensure_that_that_the_default_value_is_set_to_null_if_the_formula_is_a_table_formula()
      {
         _ontogenyFactorAgpTable.Formula.ShouldBeEqualTo(_tableFormula);
         _ontogenyFactorAgpTable.DefaultValue.ShouldBeNull();
      }
   }
}