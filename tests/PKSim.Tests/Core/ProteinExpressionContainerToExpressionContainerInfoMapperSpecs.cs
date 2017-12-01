using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_protein_expression_to_expression_container_info_mapper : ContextSpecification<IProteinExpressionContainerToExpressionContainerInfoMapper>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new ProteinExpressionContainerToExpressionContainerInfoMapper(_representationInfoRepository);
      }
   }

   
   public class When_mapping_a_protein_expression_container_to_an_expression_container_info : concern_for_protein_expression_to_expression_container_info_mapper
   {
      private MoleculeExpressionContainer _expressionContainer;
      private string _displayName;
      private ExpressionContainerInfo _result;

      protected override void Context()
      {
         base.Context();
         _displayName = "tutu";
         _expressionContainer = new MoleculeExpressionContainer().WithName("tralal");
         _expressionContainer.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameter.REL_EXP));
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_expressionContainer)).Returns(_displayName);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_expressionContainer);
      }

      [Observation]
      public void should_return_an_expression_container_info_initialized_with_the_name_of_the_container()
      {
         _result.ContainerName.ShouldBeEqualTo(_expressionContainer.Name);
      }

      [Observation]
      public void should_return_an_expression_container_info_initialized_with_the_display_name_of_the_container()
      {
         _result.ContainerDiplayName.ShouldBeEqualTo(_displayName);
      }

      [Observation]
      public void should_return_an_expression_container_info_initialized_with_the_relative_expression_of_the_container()
      {
         _result.RelativeExpression.ShouldBeEqualTo(_expressionContainer.RelativeExpression);
      }
   }
}