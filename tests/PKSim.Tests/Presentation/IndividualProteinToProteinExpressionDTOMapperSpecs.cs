using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using OSPSuite.Core.Domain;
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
      private MoleculeExpressionContainer _container1;
      private MoleculeExpressionContainer _container2;

      protected override void Context()
      {
         base.Context();
         _enzyme = new IndividualEnzyme {Name = "CYP"};
         _container1 = new MoleculeExpressionContainer().WithName("EXP1");
         _container2 = new MoleculeExpressionContainer().WithName("EXP2");
         _enzyme.AddChildren(_container1,_container2);
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