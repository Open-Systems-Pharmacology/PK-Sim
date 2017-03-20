using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualProteinToProteinExpressionDTOMapper : ContextSpecification<IIndividualProteinToProteinExpressionDTOMapper>
   {
      private IExpressionContainerDTOUpdater _expressionContainerDTOUpdater;

      protected override void Context()
      {
         _expressionContainerDTOUpdater = A.Fake<IExpressionContainerDTOUpdater>();
         sut = new IndividualProteinToProteinExpressionDTOMapper(_expressionContainerDTOUpdater);
      }
   }

   public class When_mapping_a_protein_expression_for_an_individual : concern_for_IndividualProteinToProteinExpressionDTOMapper
   {
      private IndividualEnzyme _enzyme;
      private ProteinExpressionDTO _result;
      private IMoleculeExpressionContainer _container1;
      private IMoleculeExpressionContainer _container2;

      protected override void Context()
      {
         base.Context();
         _enzyme = A.Fake<IndividualEnzyme>();
         A.CallTo(() => _enzyme.Rules).Returns(A.Fake<IBusinessRuleSet>());
         _enzyme.Name = "CYP";
         _container1 = new MoleculeExpressionContainer();
         _container2 = new MoleculeExpressionContainer();
         A.CallTo(() => _enzyme.GetChildren<IMoleculeExpressionContainer>()).Returns(new[] {_container1, _container2});
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_enzyme);
      }

      [Observation]
      public void should_return_a_metabolism_expression_dto_containing_one_entry_for_each_defined_enzyme_container()
      {
         var rootExpression = _result.AllContainerExpressions;
         rootExpression.Count().ShouldBeEqualTo(2);
      }
   }
}