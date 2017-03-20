using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_ContinuousDistributionData : ContextSpecification<ContinuousDistributionData>
   {
      protected override void Context()
      {
         sut = new ContinuousDistributionData(AxisCountMode.Count, 5);
      }
   }

   public class When_retrieving_the_bar_width_of_a_constant_interval : concern_for_ContinuousDistributionData
   {
      protected override void Context()
      {
         base.Context();
         sut.XMaxData = 100;
         sut.XMinData = 100;
      }

      [Observation]
      public void should_return_a_width_around_the_maximum_value()
      {
         sut.BarWidth.ShouldBeGreaterThan(0);
         sut.BarWidth.ShouldBeEqualTo(sut.XMaxData / 10);
      }
   }

   public class When_retrieving_the_bar_width_of_a_constant_zero_interval : concern_for_ContinuousDistributionData
   {
      protected override void Context()
      {
         base.Context();
         sut.XMaxData = 0;
         sut.XMinData = 0;
      }

      [Observation]
      public void should_return_a_bar_width_not_zero()
      {
         sut.BarWidth.ShouldBeGreaterThan(0);
      }
   }

   public class When_adding_a_value_to_the_distribution_data : concern_for_ContinuousDistributionData
   {
      protected override void Context()
      {
         base.Context();
         _gender = "tralal";
      }

      private string _gender;

      protected override void Because()
      {
         sut.AddData(10, 5,_gender);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_given_value()
      {
         sut.DataTable.Rows.Count.ShouldBeEqualTo(1);
         var values = sut.DataTable.Rows[0].ItemArray;
         values[0].ShouldBeEqualTo(_gender);
         values[1].ShouldBeEqualTo(10);
         values[2].ShouldBeEqualTo(5);
      }
   }

   public class When_intializing_a_distribution_data : concern_for_ContinuousDistributionData
   {
      [Observation]
      public void should_be_able_to_retrieve_the_groupping_name()
      {
         string.IsNullOrEmpty(sut.GroupingName).ShouldBeFalse();
      }

      [Observation]
      public void should_be_able_to_retrieve_the_xaxis_name()
      {
         string.IsNullOrEmpty(sut.XAxisName).ShouldBeFalse();
      }

      [Observation]
      public void should_be_able_to_retrieve_the_yaxis_name()
      {
         string.IsNullOrEmpty(sut.YAxisName).ShouldBeFalse();
      }
   }

   public class When_creating_distribution_data_with_the_scale_mode_percent : concern_for_ContinuousDistributionData
   {
      protected override void Context()
      {
         sut = new ContinuousDistributionData(AxisCountMode.Percent, 10);
      }

      [Observation]
      public void the_type_of_the_y_column_should_be_double()
      {
         sut.DataTable.Columns[sut.YAxisName].DataType.ShouldBeEqualTo(typeof(double));
      }
   }

   public class When_creating_distribution_data_with_the_count_mode : ContextSpecification<ContinuousDistributionData>
   {
      protected override void Context()
      {
         sut = new ContinuousDistributionData(AxisCountMode.Count, 10);
      }

      [Observation]
      public void the_type_of_the_y_column_should_be_integer()
      {
         sut.DataTable.Columns[sut.YAxisName].DataType.ShouldBeEqualTo(typeof(int));
      }
   }
}