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
         _individual.OriginData.Gender.Name.ShouldBeEqualTo(CoreConstants.Gender.MALE);
         _individual.OriginData.Age.Value.ShouldBeEqualTo(30);
      }
   }

   public class When_resolving_the_default_individual_retriever_more_than_once : concern_for_DefaultIndividualRetriever
   {
      //the cache lives on the instance, so two effective registrations would hand out two different default individuals
      [Observation]
      public void should_always_resolve_the_same_instance()
      {
         ReferenceEquals(sut, IoC.Resolve<IDefaultIndividualRetriever>()).ShouldBeTrue();
      }
   }
}
