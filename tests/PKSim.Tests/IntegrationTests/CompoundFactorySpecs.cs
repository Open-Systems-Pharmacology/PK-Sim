using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CompoundFactory : ContextForIntegration<ICompoundFactory>
   {
   }

   public class When_the_compound_factory_is_creating_a_compound : concern_for_CompoundFactory
   {
      private Compound _compound;

      protected override void Because()
      {
         _compound = sut.Create();
      }

      [Observation]
      public void should_only_have_parameters_of_type_compound()
      {
         _compound.GetAllChildren<IParameter>().Each(x => x.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Compound));
      }

      [Observation]
      public void should_contain_four_default_calculation_methods()
      {
         _compound.CalculationMethodCache.Count().ShouldBeEqualTo(4);
      }

      [Observation]
      public void should_return_a_loaded_compound()
      {
         _compound.IsLoaded.ShouldBeTrue();
      }

      [Observation]
      public void should_have_added_one_parameter_alternative_parameter_group_for_which_alternative_need_to_be_defined()
      {
         foreach (string groupName in CoreConstants.Groups.GroupsWithAlternative)
         {
            string compGroupName = groupName;
            _compound.AllParameterAlternativeGroups().FirstOrDefault(x => Equals(x.Name, compGroupName)).ShouldNotBeNull();
         }
      }

      [Observation]
      public void should_have_added_one_alternative_for_each_defined_group()
      {
         foreach (var group in _compound.AllParameterAlternativeGroups())
         {
            group.AllAlternatives.Count().ShouldBeGreaterThan(0);
         }
      }
   }
}