using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualExtractionOptions : ContextSpecification<IndividualExtractionOptions>
   {
      protected override void Context()
      {
         sut = new IndividualExtractionOptions();
      }
   }

   public class When_generating_an_individual_name_following_a_given_naming_pattern : concern_for_IndividualExtractionOptions
   {
      protected override void Context()
      {
         base.Context();
         sut.NamingPattern = $"{IndividualExtractionOptions.POP} with Individual Id = {IndividualExtractionOptions.INDIVIDUAL_ID}";
      }

      [Observation]
      public void should_return_the_expected_name()
      {
         sut.GenerateIndividualName("My Pop", 25).ShouldBeEqualTo($"My Pop with Individual Id = 25");
      }
   }
}	