using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionProfileToExpressionProfileBuildingBlockMapper : ContextSpecification<ExpressionProfileToExpressionProfileBuildingBlockMapper>
   {
      protected IObjectBaseFactory _objectBaseFactory;
      protected IEntityPathResolver _objectPathFactory;
      protected ExpressionProfile _expressionProfile;
      protected IFormulaFactory _formulaFactory;
      protected Individual _individual;
      protected ExpressionProfileBuildingBlock _result;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _objectPathFactory = new EntityPathResolverForSpecs();
         _formulaFactory = A.Fake<IFormulaFactory>();
         A.CallTo(() => _formulaFactory.RateFor(A<string>._, A<string>._, A<IFormulaCache>._)).Returns(new ExplicitFormula());
         A.CallTo(() => _objectBaseFactory.Create<ExpressionProfileBuildingBlock>()).Returns(new ExpressionProfileBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<ExpressionParameter>()).Returns(new ExpressionParameter());

         sut = new ExpressionProfileToExpressionProfileBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _formulaFactory);
      }
   }

   public class When_mapping_an_empty_expression_profile : concern_for_ExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
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
      protected override void Context()
      {
         base.Context();
         _individual = DomainHelperForSpecs.CreateIndividual("TestSpecies");
         _individual.AddMolecule(new IndividualEnzyme() { Name = "TestEnzyme" });
         _expressionProfile = new ExpressionProfile
         {
            Individual = _individual
         };
         _expressionProfile.Category = "TestCategory";
         _expressionProfile.Description = "TestDescription";
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_expressionProfile);
      }

      [Observation]
      public void resulting_building_block_should_have_the_correct_values()
      {
         _result.Name.ShouldBeEqualTo("TestEnzyme|TestSpecies|TestCategory");
         _result.Category.ShouldBeEqualTo("TestCategory");
         _result.Description.ShouldBeEqualTo("TestDescription");
         _result.MoleculeName.ShouldBeEqualTo("TestEnzyme");
         _result.Type.DisplayName.ShouldBeEqualTo("Enzyme");
         _result.Type.IconName.ShouldBeEqualTo("Enzyme");
         A.CallTo(() => _formulaFactory.RateFor(A<string>._, A<string>._, A<IFormulaCache>._)).MustHaveHappened(3, Times.Exactly);
      }
   }
}