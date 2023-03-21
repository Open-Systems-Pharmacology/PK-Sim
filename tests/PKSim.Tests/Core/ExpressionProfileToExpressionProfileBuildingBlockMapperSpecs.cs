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
using PKSim.Core.Services;
using ILazyLoadTask = OSPSuite.Core.Domain.Services.ILazyLoadTask;

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
      private ICloner _cloner;

      protected override void Context()
      {
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _objectPathFactory = new EntityPathResolverForSpecs();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _cloner= A.Fake<ICloner>();
         //cloning formula always returns the same formula for testing
         A.CallTo(() => _cloner.Clone(A<IFormula>._)).ReturnsLazily(x => x.GetArgument<IFormula>(0));

         A.CallTo(() => _objectBaseFactory.Create<ExpressionProfileBuildingBlock>()).Returns(new ExpressionProfileBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<ExpressionParameter>()).ReturnsLazily(() => new ExpressionParameter());

         sut = new ExpressionProfileToExpressionProfileBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _applicationConfiguration, _lazyLoadTask, _cloner);
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
      protected override void Context()
      {
         base.Context();
         _individual = DomainHelperForSpecs.CreateIndividual("TestSpecies");
         _individual.AddMolecule(new IndividualEnzyme { Name = "TestEnzyme" });
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
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_expressionProfile);
      }

      [Observation]
      public void resulting_expression_parameter_should_have_formula_or_value()
      {
         _result.Count(x => x.Formula == null).ShouldBeEqualTo(2);
         _result.Count(x => x.Formula != null).ShouldBeEqualTo(1);
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
         _result.FormulaCache.Count.ShouldBeEqualTo(2);
      }
   }
}