using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using Compound = PKSim.Core.Model.Compound;
using static PKSim.Core.CoreConstants.Groups;

namespace PKSim.IntegrationTests
{
   public class When_sorting_the_advanced_compound_parameters_displayed_in_a_simulation : ContextForIntegration<ICompoundFactory>
   {
      private static readonly string[] _expectedGroupOrder =
      {
         COMPOUND_DISSOLUTION,
         COMPOUND_TWO_PORE,
         COMPOUND_ADVANCED_SOLUBILITY,
         COMPOUND_BILE_SALT_PARTITION_COEFFICIENT
      };

      private Compound _compound;
      private List<string> _groupNamesOrderedBySequence;

      protected override void Context()
      {
         base.Context();
         _compound = sut.Create();
      }

      protected override void Because()
      {
         _groupNamesOrderedBySequence = _compound.AllParameters(x => _expectedGroupOrder.Contains(x.GroupName))
            .OrderBy(x => x.Sequence)
            .Select(x => x.GroupName)
            .ToList();
      }

      [Observation]
      public void should_display_all_parameters_of_a_given_group_one_after_the_other()
      {
         //all parameters of the same group should be consecutive: a group name should not appear again once another group started
         _groupNamesOrderedBySequence.Distinct().Count().ShouldBeEqualTo(consecutiveGroups().Count);
      }

      [Observation]
      public void should_display_the_groups_in_the_expected_order()
      {
         consecutiveGroups().ShouldOnlyContainInOrder(_expectedGroupOrder);
      }

      private List<string> consecutiveGroups()
      {
         var groups = new List<string>();
         _groupNamesOrderedBySequence.Each(groupName =>
         {
            if (!string.Equals(groups.LastOrDefault(), groupName))
               groups.Add(groupName);
         });
         return groups;
      }
   }
}
