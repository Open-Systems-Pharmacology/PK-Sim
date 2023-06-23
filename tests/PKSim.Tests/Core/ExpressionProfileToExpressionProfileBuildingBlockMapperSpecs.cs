using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionProfileToExpressionProfileBuildingBlockMapper : ContextSpecification<ExpressionProfileToExpressionProfileBuildingBlockMapper>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IEntityPathResolver _objectPathFactory;
      protected ExpressionProfile _expressionProfile;
      protected Individual _individual;
      protected ExpressionProfileBuildingBlock _result;
      private IApplicationConfiguration _applicationConfiguration;
      private ILazyLoadTask _lazyLoadTask;
      private IFormulaFactory _formulaFactory;
      protected IInitialConditionsCreator _initialConditionsCreator;
      protected IMoleculeBuilderFactory _moleculeBuilderFactory;

      protected override void Context()
      {
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _objectPathFactory = new EntityPathResolverForSpecs();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _initialConditionsCreator = A.Fake<IInitialConditionsCreator>();
         _moleculeBuilderFactory = A.Fake<IMoleculeBuilderFactory>();

         A.CallTo(() => _objectBaseFactory.Create<ExpressionProfileBuildingBlock>()).Returns(new ExpressionProfileBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<ExpressionParameter>()).ReturnsLazily(() => new ExpressionParameter());

         sut = new ExpressionProfileToExpressionProfileBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _applicationConfiguration, _lazyLoadTask, _formulaFactory, _initialConditionsCreator, _moleculeBuilderFactory);
      }
   }

   public class When_mapping_an_empty_expression_profile : concern_for_ExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      protected override void Context()
      {
         base.Context();
         _individual = new Individual();
         _individual.AddMolecule(new IndividualEnzyme());
         var species = new Species
         {
            DisplayName = "Foo"
         };
         _individual.OriginData = new OriginData
         {
            Species = species
         };

         _expressionProfile = new ExpressionProfile
         {
            Individual = _individual
         };
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_expressionProfile);
      }

      [Observation]
      public void should_create_an_empty_building_block()
      {
         _result.ShouldNotBeNull();
         _result.Name.ShouldBeEmpty();
         _result.Category.ShouldBeNull();
         _result.Description.ShouldBeEmpty();
         _result.FormulaCache.ShouldBeEmpty();
      }
   }

   public class When_mapping_a_non_empty_expression_profile : concern_for_ExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      private Container _physicalContainer;
      private MoleculeBuilder _moleculeBuilder;

      protected override void Context()
      {
         base.Context();
         _individual = DomainHelperForSpecs.CreateIndividual("TestSpecies");
         var individualMolecule = new IndividualEnzyme {Name = "TestEnzyme"};
         _individual.AddMolecule(individualMolecule);
         _expressionProfile = new ExpressionProfile
         {
            Individual = _individual,
            Category = "TestCategory",
            Description = "TestDescription"
         };

         _expressionProfile.GetAllChildren<IParameter>().First().Value = 99;
         var parameter = _expressionProfile.GetAllChildren<IParameter>().Last();
         parameter.Value = 99;
         parameter.Formula = null;
         var topContainer = new Container
         {
            ContainerType = ContainerType.Organ,
            Mode = ContainerMode.Physical,
            Name = "topContainer"
         };

         var container = new Container
         {
            ContainerType = ContainerType.Organ,
            Mode = ContainerMode.Physical,
            Name = "physicalContainer"
         };
         _physicalContainer = container;

         container.Add(new Container
         {
            ContainerType = ContainerType.Molecule,
            Name = _expressionProfile.MoleculeName,
            Mode = ContainerMode.Logical
         });
         topContainer.Add(container);
         _individual.Organism.Add(topContainer);

         _moleculeBuilder = new MoleculeBuilder();

         A.CallTo(() => _moleculeBuilderFactory.Create(individualMolecule.MoleculeType, A<FormulaCache>._)).Returns(_moleculeBuilder);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_expressionProfile);
      }

      [Observation]
      public void the_initial_conditions_should_be_added_to_the_building_block_using_the_initialConditionsCreator()
      {
         A.CallTo(() => _initialConditionsCreator.AddToExpressionProfile(_result, A<IReadOnlyList<IContainer>>.That.Matches(containers => containers.Contains(_physicalContainer)), _moleculeBuilder)).MustHaveHappened();
      }

      [Observation]
      public void resulting_expression_parameter_should_have_formula_or_value()
      {
         _result.ExpressionParameters.Count(x => x.Formula == null).ShouldBeEqualTo(1);
         _result.ExpressionParameters.Count(x => x.Formula != null).ShouldBeEqualTo(2);
      }

      [Observation]
      public void resulting_building_block_should_have_the_correct_values()
      {
         _result.Name.ShouldBeEqualTo("TestEnzyme|TestSpecies|TestCategory");
         _result.Category.ShouldBeEqualTo("TestCategory");
         _result.Description.ShouldBeEqualTo("TestDescription");
         _result.MoleculeName.ShouldBeEqualTo("TestEnzyme");
         _result.Type.ShouldBeEqualTo(ExpressionTypes.MetabolizingEnzyme);
      }
   }
}