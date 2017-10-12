using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeToQueryExpressionSettingsMapper : ContextSpecification<IMoleculeToQueryExpressionSettingsMapper>
   {
      protected IndividualProtein _protein;
      protected QueryExpressionSettings _result;
      protected IProteinExpressionContainerToExpressionContainerInfoMapper _expressionContainerMapper;

      protected override void Context()
      {
         _expressionContainerMapper = A.Fake<IProteinExpressionContainerToExpressionContainerInfoMapper>();
         sut = new MoleculeToQueryExpressionSettingsMapper(_expressionContainerMapper);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_protein);
      }
   }

   public class When_mapping_a_protein_expression_to_a_query_expression_setting : concern_for_MoleculeToQueryExpressionSettingsMapper
   {
      private double _proteinContent;
      private MoleculeExpressionContainer _exp1;
      private MoleculeExpressionContainer _exp2;
      private ExpressionContainerInfo _expInfo1;
      private ExpressionContainerInfo _expInfo2;

      protected override void Context()
      {
         base.Context();
         _protein = A.Fake<IndividualProtein>();
         _protein.QueryConfiguration = "toto";
         _exp1 = new MoleculeExpressionContainer().WithName("exp1");
         _exp2 = new MoleculeExpressionContainer().WithName("exp2");
         _expInfo1 = A.Fake<ExpressionContainerInfo>();
         _expInfo2 = A.Fake<ExpressionContainerInfo>();
         A.CallTo(() => _protein.AllExpressionsContainers()).Returns(new[] {_exp1, _exp2});
         _proteinContent = 10;
         A.CallTo(() => _protein.ReferenceConcentration).Returns(DomainHelperForSpecs.ConstantParameterWithValue(_proteinContent));
         A.CallTo(() => _expressionContainerMapper.MapFrom(_exp1)).Returns(_expInfo1);
         A.CallTo(() => _expressionContainerMapper.MapFrom(_exp2)).Returns(_expInfo2);
      }

      [Observation]
      public void should_return_a_query_settings_initialized_with_one_container()
      {
         _result.ExpressionContainers.ShouldOnlyContainInOrder(_expInfo1, _expInfo2);
      }

      [Observation]
      public void should_have_set_the_query_configuration_to_the_predefined_configuration()
      {
         _result.QueryConfiguration.ShouldBeEqualTo(_protein.QueryConfiguration);
      }
   }
}