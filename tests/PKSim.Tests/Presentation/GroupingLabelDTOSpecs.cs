using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.DTO.PopulationAnalyses;
using OSPSuite.Core.Chart;

namespace PKSim.Presentation
{
   public abstract class concern_for_GroupingLabelDTO : ContextSpecification<GroupingLabelDTO>
   {
      protected override void Context()
      {
         sut = new GroupingLabelDTO();
      }
   }

   public class When_creating_a_new_grouping_label : concern_for_GroupingLabelDTO
   {
      [Observation]
      public void the_default_symbol_should_be_set_to_circle()
      {
         sut.Symbol.ShouldBeEqualTo(Symbols.Circle);
      }
   }
}