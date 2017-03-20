using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataYValue : ContextSpecification<ObservedDataYValue>
   {
      protected override void Context()
      {
         sut = new ObservedDataYValue();
      }
   }

   public class When_creating_an_observed_data_y_value : concern_for_ObservedDataYValue
   {
      [Observation]
      public void it_should_be_valid_only_if_a_mean_value_was_set()
      {
         sut.IsValid.ShouldBeFalse();
         
         sut.Mean = 5;
         sut.IsValid.ShouldBeTrue();
      }
   }

   public class When_retrieving_the_upper_value_for_an_observed_data_y_value : concern_for_ObservedDataYValue
   {
      protected override void Context()
      {
         base.Context();
         sut.Mean = 5;
      }

      [Observation]
      public void should_return_the_mean_if_no_error_is_defined()
      {
         sut.ErrorType = AuxiliaryType.Undefined;
         sut.UpperValue.ShouldBeEqualTo(sut.Mean);

         sut.ErrorType = AuxiliaryType.ArithmeticStdDev;
         sut.Error = 0;
         sut.UpperValue.ShouldBeEqualTo(sut.Mean);

         sut.Error = -5;
         sut.UpperValue.ShouldBeEqualTo(sut.Mean);

      }

      [Observation]
      public void should_return_the_expected_value_for_an_arithmetical_error()
      {
         sut.ErrorType = AuxiliaryType.ArithmeticStdDev;
         sut.Error = 2;
         sut.UpperValue.ShouldBeEqualTo(sut.Mean + sut.Error);
      }

      [Observation]
      public void should_return_the_expected_value_for_a_geometrical_error()
      {
         sut.ErrorType = AuxiliaryType.GeometricStdDev;
         sut.Error = 1.5f;
         sut.UpperValue.ShouldBeEqualTo(sut.Mean *sut.Error);
      }
   }

   public class When_retrieving_the_lower_value_for_an_observed_data_y_value : concern_for_ObservedDataYValue
   {
      protected override void Context()
      {
         base.Context();
         sut.Mean = 5;
      }

      [Observation]
      public void should_return_the_mean_if_no_error_is_defined()
      {
         sut.ErrorType = AuxiliaryType.Undefined;
         sut.LowerValue.ShouldBeEqualTo(sut.Mean);

         sut.ErrorType = AuxiliaryType.ArithmeticStdDev;
         sut.Error = 0;
         sut.LowerValue.ShouldBeEqualTo(sut.Mean);

         sut.Error = -5;
         sut.LowerValue.ShouldBeEqualTo(sut.Mean);

      }

      [Observation]
      public void should_return_the_expected_value_for_an_arithmetical_error()
      {
         sut.ErrorType = AuxiliaryType.ArithmeticStdDev;
         sut.Error = 2;
         sut.LowerValue.ShouldBeEqualTo(sut.Mean - sut.Error);
      }

      [Observation]
      public void should_return_the_expected_value_for_a_geometrical_error()
      {
         sut.ErrorType = AuxiliaryType.GeometricStdDev;
         sut.Error = 1.5f;
         sut.LowerValue.ShouldBeEqualTo(sut.Mean / sut.Error);
      }
   }
}	