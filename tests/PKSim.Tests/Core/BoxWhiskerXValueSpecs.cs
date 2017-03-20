using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;

namespace PKSim.Core
{
   public abstract class concern_for_BoxWhiskerXValue : ContextSpecification<BoxWhiskerXValue>
   {
      protected override void Context()
      {
         sut = new BoxWhiskerXValue(new[] {"Normal", "Young"});
      }
   }

   public class When_calling_BoxWhiskerXValue_constructor_with_empty_List : concern_for_BoxWhiskerXValue
   {
      protected override void Because()
      {
         sut = new BoxWhiskerXValue(new List<string>());
      }

      [Observation]
      public void should_count_be_0()
      {
         sut.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_X_be_NaN()
      {
         sut.X.ShouldBeEqualTo(float.NaN);
      }

      [Observation]
      public void access_to_item_should_throw_an_exception()
      {
         The.Action(() => { var x = sut[0]; }).ShouldThrowAn<ArgumentOutOfRangeException>();
      }
   }

   public class When_calling_BoxWhiskerXValue_constructor_with_2_elements_List : concern_for_BoxWhiskerXValue
   {
      [Observation]
      public void should_count_be_2()
      {
         sut.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_X_be_NaN()
      {
         sut.X.ShouldBeEqualTo(float.NaN);
      }

      [Observation]
      public void should_return_values_in_right_order()
      {
         sut[0].ShouldBeEqualTo("Normal");
         sut[1].ShouldBeEqualTo("Young");
      }
   }
}