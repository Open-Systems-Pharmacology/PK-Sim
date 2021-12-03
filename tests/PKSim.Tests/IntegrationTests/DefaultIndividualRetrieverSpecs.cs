using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DefaultIndividualRetriever : ContextForIntegration<IDefaultIndividualRetriever>
   {
      protected override void Context()
      {
         sut = IoC.Resolve<IDefaultIndividualRetriever>();
      }
   }

   public class When_retrieving_the_default_individual_for_a_human : concern_for_DefaultIndividualRetriever
   {
      private Individual _individual;

      protected override void Because()
      {
         _individual = sut.DefaultHuman();
      }

      [Observation]
      public void should_return_the_expected_results()
      {
         _individual.Species.Name.ShouldBeEqualTo(CoreConstants.Species.HUMAN);
         _individual.OriginData.Gender.Name.ShouldBeEqualTo(CoreConstants.Gender.Male);
         _individual.OriginData.Age.Value.ShouldBeEqualTo(30);
      }
   }
}