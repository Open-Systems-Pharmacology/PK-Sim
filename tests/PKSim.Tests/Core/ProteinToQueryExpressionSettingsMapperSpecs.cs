using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Collections;
using PKSim.Core.Repositories;
using NPOI.SS.Formula.Functions;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeToQueryExpressionSettingsMapper : ContextSpecification<IMoleculeToQueryExpressionSettingsMapper>
   {

      protected QueryExpressionSettings _result;
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new MoleculeToQueryExpressionSettingsMapper(_representationInfoRepository);
      }
   }

   public class When_mapping_a_expression_building_block_update_to_a_query_expression_setting : concern_for_MoleculeToQueryExpressionSettingsMapper
   {
      private ExpressionProfileBuildingBlock _buildingBlock;
      private ExpressionProfileBuildingBlockUpdate _expressionProfileUpdate;

      protected override void Context()
      {
         base.Context();
         _buildingBlock = new ExpressionProfileBuildingBlock();
         _buildingBlock.Name = "MoleculeName|Species|Category";

         var expressionParameter = new ExpressionParameter
         {
            Path = new ObjectPath("disp1", "exp1", CoreConstants.Parameters.REL_EXP),
            Value = 1.0
         };

         _buildingBlock.Add(expressionParameter);

         expressionParameter = new ExpressionParameter
         {
            Path = new ObjectPath("disp2", "exp2", CoreConstants.Parameters.REL_EXP_BLOOD_CELLS),
            Value = 1.0
         };

         _buildingBlock.Add(expressionParameter);

         expressionParameter = new ExpressionParameter
         {
            Path = new ObjectPath("disp4", "exp4", CoreConstants.Parameters.AGE),
            Value = 1.0
         };

         _buildingBlock.Add(expressionParameter);

         expressionParameter = new ExpressionParameter
         {
            Path = new ObjectPath("disp3", "exp3", CoreConstants.Parameters.REL_EXP),
         };

         _buildingBlock.Add(expressionParameter);

         _expressionProfileUpdate = new ExpressionProfileBuildingBlockUpdate(_buildingBlock);

         A.CallTo(() => _representationInfoRepository.ContainerInfoFor(A<string>._)).ReturnsLazily(s => new RepresentationInfo { DisplayName = (string)s.Arguments[0] });
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_expressionProfileUpdate);
      }

      [Observation]
      public void should_only_query_for_relative_expression_parameters_with_a_value()
      {
         _result.ExpressionContainers.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_return_a_query_settings_initialized_with_one_container()
      {
         _result.ExpressionContainers.ElementAt(0).ContainerDisplayName.ShouldBeEqualTo("disp1");
         _result.ExpressionContainers.ElementAt(1).ContainerDisplayName.ShouldBeEqualTo(CoreConstants.Compartment.BLOOD_CELLS);
      }
   }

   public class When_mapping_a_protein_expression_to_a_query_expression_setting : concern_for_MoleculeToQueryExpressionSettingsMapper
   {
      private double _proteinContent;
      private MoleculeExpressionContainer _exp1;
      private MoleculeExpressionContainer _exp2;
      private IParameter _relExp1;
      private IParameter _relExp2;
      protected Individual _individual;
      protected IndividualProtein _protein;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _protein = A.Fake<IndividualProtein>();
         _protein.QueryConfiguration = "toto";
         _exp1 = new MoleculeExpressionContainer().WithName("exp1");
         _exp2 = new MoleculeExpressionContainer().WithName("exp2");
         _relExp1 = DomainHelperForSpecs.ConstantParameterWithValue(1);
         _relExp2 = DomainHelperForSpecs.ConstantParameterWithValue(2);
         var allExpressions = new Cache<string, IParameter> { { _exp1.Name, _relExp1 }, { _exp2.Name, _relExp2 } };
         A.CallTo(() => _individual.AllExpressionParametersFor(_protein)).Returns(allExpressions);

         _proteinContent = 10;
         A.CallTo(() => _protein.ReferenceConcentration).Returns(DomainHelperForSpecs.ConstantParameterWithValue(_proteinContent));
         A.CallTo(() => _representationInfoRepository.ContainerInfoFor(_exp1.Name)).Returns(new RepresentationInfo { DisplayName = "disp1" });
         A.CallTo(() => _representationInfoRepository.ContainerInfoFor(_exp2.Name)).Returns(new RepresentationInfo { DisplayName = "disp2" });
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_protein, _individual, _protein.Name);
      }

      [Observation]
      public void should_return_a_query_settings_initialized_with_one_container()
      {
         _result.ExpressionContainers.Count().ShouldBeEqualTo(2);
         _result.ExpressionContainers.ElementAt(0).ContainerDisplayName.ShouldBeEqualTo("disp1");
         _result.ExpressionContainers.ElementAt(1).ContainerDisplayName.ShouldBeEqualTo("disp2");
      }

      [Observation]
      public void should_have_set_the_query_configuration_to_the_predefined_configuration()
      {
         _result.QueryConfiguration.ShouldBeEqualTo(_protein.QueryConfiguration);
      }
   }
}