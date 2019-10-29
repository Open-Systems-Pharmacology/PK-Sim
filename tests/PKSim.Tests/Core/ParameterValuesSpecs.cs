using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Populations;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterValues : ContextSpecification<ParameterValues>
   {
      protected override void Context()
      {
         sut = new ParameterValues("PATH1");
      }
   }

   public class The_count_of_an_empty_parameter_values : concern_for_ParameterValues
   {
      [Observation]
      public void should_return_zero()
      {
         sut.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_adding_a_value_to_a_parameter_values : concern_for_ParameterValues
   {
      protected override void Because()
      {
         sut.Add(25);
      }

      [Observation]
      public void should_add_the_default_percentile()
      {
         sut.Percentiles.ShouldOnlyContain(CoreConstants.DEFAULT_PERCENTILE);
      }

      [Observation]
      public void should_add_the_given_values()
      {
         sut.Values.ShouldOnlyContain(25);
      }
   }

   public class When_adding_a_number_of_empty_items_to_the_parameter_values : concern_for_ParameterValues
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(10,0.1);
         sut.Add(20, 0.2);
      }

      protected override void Because()
      {
         sut.AddEmptyItems(5, -15);
      }

      [Observation]
      public void should_fill_it_up_with_the_default_values()
      {
         sut.Count.ShouldBeEqualTo(7);
         sut.Values.ShouldOnlyContain(10, 20, -15, -15, -15, -15, -15);
      }
   }

   public class When_merging_two_parameter_values : concern_for_ParameterValues
   {
      private ParameterValues _valuesToMerge;

      protected override void Context()
      {
         base.Context();
         sut.Add(1, 0.1);
         sut.Add(2,0.2);
         _valuesToMerge = new ParameterValues("PATH1");
         _valuesToMerge.Add(3,0.3);
         _valuesToMerge.Add(4,0.4);
         _valuesToMerge.Add(5,0.5);
      }

      protected override void Because()
      {
         sut.Merge(_valuesToMerge);
      }

      [Observation]
      public void should_have_increase_the_count_with_the_number_of_values_merged()
      {
         sut.Count.ShouldBeEqualTo(5);   
      }

      [Observation]
      public void should_add_the_valuesvalues_to_merge()
      {
         sut.Values.ShouldOnlyContain(1, 2, 3, 4, 5);
      }

      [Observation]
      public void should_add_percentile_from_the_parameter_values_to_merge()
      {
         sut.Percentiles.ShouldOnlyContain(0.1, 0.2, 0.3, 0.4, 0.5);
      }
   }
}