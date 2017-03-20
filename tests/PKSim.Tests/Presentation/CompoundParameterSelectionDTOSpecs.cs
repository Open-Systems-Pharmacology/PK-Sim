using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;

using FakeItEasy;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_compund_parameter_selection_dto : ContextSpecification<CompoundParameterSelectionDTO>
   {
      private PKSim.Core.Model.ParameterAlternativeGroup _compoundParameterGroup;

      protected override void Context()
      {
         _compoundParameterGroup = A.Fake<PKSim.Core.Model.ParameterAlternativeGroup>();
         A.CallTo(() => _compoundParameterGroup.Name).Returns("tralala");
         sut = new CompoundParameterSelectionDTO(_compoundParameterGroup);
      }
   }

   
   public class When_validating_an_compound_parameter_selection_dto_for_which_no_selection_was_defined : concern_for_compund_parameter_selection_dto
   {
      protected override void Context()
      {
         base.Context();
         sut.SelectedAlternative = null;
      }

      [Observation]
      public void should_return_the_the_dto_is_not_valid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   
   public class When_validating_an_compound_parameter_selection_dto_for_which_a_selection_was_defined : concern_for_compund_parameter_selection_dto
   {
      protected override void Context()
      {
         base.Context();
         sut.SelectedAlternative = A.Fake<PKSim.Core.Model.ParameterAlternative>();
      }

      [Observation]
      public void should_return_the_the_dto_is_not_valid()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }
}