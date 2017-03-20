using OSPSuite.Utility.Validation;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.DTO.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_FixedLimitGroupingDTO : ContextSpecification<FixedLimitGroupingDTO>
   {
      protected override void Context()
      {
         sut = new FixedLimitGroupingDTO();
      }
   }

   public class When_checking_if_a_fixed_limit_grouping_item_is_valid : concern_for_FixedLimitGroupingDTO
   {
      [Observation]
      public void should_return_true_if_the_label_and_maximum_is_set()
      {
         sut.Maximum = 10;
         sut.Label = "xxx";
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_label_is_not_set()
      {
         sut.Maximum = 10;
         sut.Label = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_maximum_is_not_set_and_the_maximum_is_editable()
      {
         sut.Maximum = null;
         sut.MaximumEditable = true;
         sut.Label = "xxx";
         sut.IsValid().ShouldBeFalse();
      }
   }
}	