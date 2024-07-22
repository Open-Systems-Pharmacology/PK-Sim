using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_StaticReactionRepository : ContextForIntegration<IStaticReactionRepository>
   {
      protected const string _reactionName1 = "FcRn binding tissue";
      protected const string _reactionPrefix = "FcRn binding drug in ";
   }

   public class When_retrieving_all_static_reactions_the_repository : concern_for_StaticReactionRepository
   {
      private IEnumerable<PKSimReaction> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_getting_reactions_with_nonempty_reaction_partners_list : concern_for_StaticReactionRepository
   {
      protected PKSimReaction _reaction;

      protected override void Because()
      {
         _reaction = sut.All().FirstOrDefault(r => r.Name.Equals(_reactionName1));
      }

      [Observation]
      public void should_return_the_reaction_with_partners()
      {
         _reaction.ShouldNotBeNull();
         _reaction.Educts.Count().ShouldBeGreaterThan(0);
         _reaction.Products.Count().ShouldBeGreaterThan(0);
         _reaction.ContainerCriteria.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_getting_reactions_for_model : concern_for_StaticReactionRepository
   {
      private IList<PKSimReaction> _fourCompReactions;
      private IList<PKSimReaction> _twoPoreReactions;

      protected override void Because()
      {
         _fourCompReactions = sut.AllFor(CoreConstants.Model.FOUR_COMP).ToList();
         _twoPoreReactions = sut.AllFor(CoreConstants.Model.TWO_PORES).ToList();
      }

      [Observation]
      public void four_comp_reactions_list_should_be_empty()
      {
         _fourCompReactions.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void two_pores_reactions_list_should_contain_FcRn_binding_tissue()
      {
         _twoPoreReactions.Any(r => r.Name.Equals(_reactionName1)).ShouldBeTrue();
      }

      [Observation]
      public void two_pores_FcRn_binding_tissue_reaction_should_not_have_container_criteria()
      {
         _twoPoreReactions.First(r => r.Name.Equals(_reactionName1)).ContainerCriteria.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void two_pores_endogenousIgg_FcRn_binding_reactions_Should_have_container_criteria()
      {
         var reactions = _twoPoreReactions.Where(r => r.Name.StartsWith(_reactionPrefix)).ToList();
         reactions.Count.ShouldBeEqualTo(3);
         reactions.Each(r=>r.ContainerCriteria.Count.ShouldBeGreaterThan(0));
      }
   }
}