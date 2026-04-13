using System;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_UserSettingsDTO : ContextSpecification<UserSettingsDTO>
   {
      protected override void Context()
      {
         sut = new UserSettingsDTO
         {
            DefaultFractionUnboundName = "default",
            DefaultSolubilityName = "default",
            DefaultLipophilicityName = "default",
            MaximumNumberOfCoresToUse = 1,
         };
      }
   }

   public class When_validating_decimal_places : concern_for_UserSettingsDTO
   {
      [TestCase(0u)]
      [TestCase(5u)]
      [TestCase(15u)]
      public void should_be_valid_for_values_between_0_and_15(uint decimalPlace)
      {
         sut.DecimalPlace = decimalPlace;
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_invalid_for_value_greater_than_15()
      {
         sut.DecimalPlace = 16;
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_maximum_number_of_cores_to_use : concern_for_UserSettingsDTO
   {
      [Observation]
      public void should_be_valid_for_one_core()
      {
         sut.MaximumNumberOfCoresToUse = 1;
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_valid_for_processor_count()
      {
         sut.MaximumNumberOfCoresToUse = Environment.ProcessorCount;
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_invalid_for_zero()
      {
         sut.MaximumNumberOfCoresToUse = 0;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_invalid_for_negative_value()
      {
         sut.MaximumNumberOfCoresToUse = -1;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_invalid_when_exceeding_processor_count()
      {
         sut.MaximumNumberOfCoresToUse = Environment.ProcessorCount + 1;
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_validating_default_fraction_unbound_name : concern_for_UserSettingsDTO
   {
      [Observation]
      public void should_be_invalid_when_empty()
      {
         sut.DefaultFractionUnboundName = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_valid_when_set()
      {
         sut.DefaultFractionUnboundName = "some value";
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_validating_default_solubility_name : concern_for_UserSettingsDTO
   {
      [Observation]
      public void should_be_invalid_when_empty()
      {
         sut.DefaultSolubilityName = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_valid_when_set()
      {
         sut.DefaultSolubilityName = "some value";
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_validating_default_lipophilicity_name : concern_for_UserSettingsDTO
   {
      [Observation]
      public void should_be_invalid_when_empty()
      {
         sut.DefaultLipophilicityName = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_valid_when_set()
      {
         sut.DefaultLipophilicityName = "some value";
         sut.IsValid().ShouldBeTrue();
      }
   }
}