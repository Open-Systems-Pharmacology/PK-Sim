using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Model;

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

      protected override void Context()
      {
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _objectPathFactory = new EntityPathResolverForSpecs();
         A.CallTo(() => _objectBaseFactory.Create<ExpressionProfileBuildingBlock>()).Returns(new ExpressionProfileBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<ExpressionParameter>()).Returns(new ExpressionParameter());

         sut = new ExpressionProfileToExpressionProfileBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _applicationConfiguration);
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
         _result.FormulaCache.Count.ShouldBeEqualTo(3);
      }
   }
}