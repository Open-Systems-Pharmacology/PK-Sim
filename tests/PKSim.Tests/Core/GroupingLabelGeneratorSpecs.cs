using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_GroupingLabelGenerator : ContextSpecification<IGroupingLabelGenerator>
   {
      protected LabelGenerationOptions _options;
      protected IReadOnlyCollection<string> _result;
      protected List<double> _limits;

      protected override void Context()
      {
         sut = new GroupingLabelGenerator();
         _options = new LabelGenerationOptions();
         _limits = new List<double>();

      }

      protected override void Because()
      {
         _result = sut.GenerateLabels(_options, _limits);
      }
   }

   public class When_generating_grouping_labels_using_a_pattern_that_does_not_contain_the_iteration : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "A";
         _limits = new List<double> { 1, 2,3};
         _options.Strategy = LabelGenerationStrategies.Alpha;
      }

      [Observation]
      public void should_return_an_list_containing_only_the_given_pattern()
      {
         _result.ShouldOnlyContainInOrder("A", "A");
      }
   }

   public class When_generating_grouping_labels_using_the_alpha_strategy : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label_#";
         _limits = new List<double> { 1, 2,3,4 };
         _options.Strategy = LabelGenerationStrategies.Alpha;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label_A", "Label_B", "Label_C");
      }
   }

   public class When_generating_grouping_labels_using_some_interval_definitions_and_iteration_pattern : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label_# {start} to {end}";
         _limits = new List<double> { 1, 2, 3, 4 };
         _options.Strategy = LabelGenerationStrategies.Alpha;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label_A 1.00 to 2.00", "Label_B 2.00 to 3.00", "Label_C 3.00 to 4.00");
      }
   }

   public class When_generating_grouping_labels_using_some_interval_definitions : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label {start} to {end}";
         _limits = new List<double> { 1, 2, 3, 4 };
         _options.Strategy = LabelGenerationStrategies.Alpha;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label 1.00 to 2.00", "Label 2.00 to 3.00", "Label 3.00 to 4.00");
      }
   }

   public class When_generating_grouping_labels_using_some_interval_definitions_with_number_of_decimal_set : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label {start:2} to {end:2}";
         _limits = new List<double> { 1, 2, 3, 4 };
         _options.Strategy = LabelGenerationStrategies.Alpha;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label 1.00 to 2.00", "Label 2.00 to 3.00", "Label 3.00 to 4.00");
      }
   }

   public class When_generating_grouping_labels_using_the_alpha_strategy_with_30_labels : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label_#";
         _limits = new List<double>(Enumerable.Range(1,31).Select(x=>x.ConvertedTo<double>()));
         _options.Strategy = LabelGenerationStrategies.Alpha;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label_A", "Label_B", "Label_C", "Label_D", "Label_E", "Label_F", "Label_G", "Label_H", "Label_I", "Label_J", "Label_K", "Label_L", "Label_M", "Label_N", "Label_O", "Label_P", "Label_Q", "Label_R", "Label_S", "Label_T", "Label_U", "Label_V", "Label_W", "Label_X", "Label_Y", "Label_Z", "Label_AA", "Label_AB", "Label_AC", "Label_AD");
      }
   }

   public class When_generating_grouping_labels_using_the_numeric_strategy : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label_#";
         _limits = new List<double> { 1, 2, 3, 4 };
         _options.Strategy = LabelGenerationStrategies.Numeric;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label_1", "Label_2", "Label_3");
      }
   }

   public class When_generating_grouping_labels_using_the_roman_strategy : concern_for_GroupingLabelGenerator
   {
      protected override void Context()
      {
         base.Context();
         _options.Pattern = "Label_#";
         _limits = new List<double> { 1, 2, 3, 4 };
         _options.Strategy = LabelGenerationStrategies.Roman;
      }

      [Observation]
      public void should_return_the_expected_labels()
      {
         _result.ShouldOnlyContainInOrder("Label_I", "Label_II", "Label_III");
      }
   }
}