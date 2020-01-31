using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public class When_checking_if_a_version_can_be_loaded_: StaticContextSpecification
   {
      [Observation]
      public void should_return_true_for_a_know_version_that_is_smaller_than_the_current_version()
      {
         ProjectVersions.CanLoadVersion(ProjectVersions.V6_2_1).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_an_unknown_version_that_is_smaller_than_the_current_version()
      {
         ProjectVersions.CanLoadVersion(35).ShouldBeFalse();
      }

      [Observation]
      public void should_return_null_if_a_version_number_is_not_found()
      {
         ProjectVersions.FindBy(-50).ShouldBeNull();
      }
   }
}	