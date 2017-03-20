using System.Collections.Generic;
using System.Drawing;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;

namespace PKSim.Core
{
   public abstract class concern_for_ValueMappingGroupingDefinition : ContextSpecification<ValueMappingGroupingDefinition>
   {
      protected GroupingItem _groupingItemMale;
      protected GroupingItem _groupingItemFemale;

      protected override void Context()
      {
         sut = new ValueMappingGroupingDefinition("Field");
         _groupingItemMale = new GroupingItem {Label = "Male", Color = Color.Blue, Symbol = Symbols.Circle};
         _groupingItemFemale = new GroupingItem {Label = "Female", Color = Color.Red, Symbol = Symbols.Diamond};
      }
   }

   public class When_creating_a_ValueMappingGroupingDefinition : concern_for_ValueMappingGroupingDefinition
   {
      [Observation]
      public void the_expression_should_be_generated()
      {
         sut.AddValueLabel("0",_groupingItemMale);
         sut.AddValueLabel("1", _groupingItemFemale);

         var expectedResults = string.Format("iif([Field] = '0', 'Male', iif([Field] = '1', 'Female', 'Unknown'))");
         sut.GetExpression().ShouldBeEqualTo(expectedResults);
      }
   }

   public class When_retrieving_the_group_info_defined_for_a_value_mapping : concern_for_ValueMappingGroupingDefinition
   {
      protected override void Context()
      {
         base.Context();
         sut.AddValueLabel("0", _groupingItemMale);
         sut.AddValueLabel("1", _groupingItemFemale);
      }

      [Observation]
      public void should_return_a_set_containing_the_expected_grouping_information()
      {
         var groupInfo = sut.GroupingItems;
         groupInfo.Count.ShouldBeEqualTo(2);
         groupInfo[0].ShouldBeEqualTo(_groupingItemMale);
         groupInfo[1].ShouldBeEqualTo(_groupingItemFemale);
      }

   }
}