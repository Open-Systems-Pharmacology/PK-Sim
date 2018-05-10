using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v7_3;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v7_3
{
   public class When_converting_the_simple_simulation_iv_710_project : ContextWithLoadedProject<Converter721To730>
   {
      private List<Compound> _allCompounds;
      private List<Simulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleSimulationIV_710");

         _allCompounds = All<Compound>().ToList();
         _allSimulations = All<Simulation>().ToList();
         _allCompounds.Each(Load);
         _allSimulations.Each(Load);
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_standard_input_parameter_to_non_default()
      {
         foreach (var compound in _allCompounds)
         {
            checkIsDefaultFlagIn(compound);
         }
      }

      private void checkIsDefaultFlagIn(Compound compound)
      {
         compound.Parameter(CoreConstants.Parameters.MOLECULAR_WEIGHT).IsDefault.ShouldBeFalse();
         var lipoGroup = compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
         var parameters = lipoGroup.AllAlternatives.Select(x => x.Parameter(CoreConstants.Parameters.LIPOPHILICITY));
         parameters.Each(p => p.IsDefault.ShouldBeFalse());
      }

      [Observation]
      public void should_have_added_a_solubility_table_in_all_compound_as_direct_child()
      {
         foreach (var compound in _allCompounds)
         {
            checkSolubilityTableInContainer(compound);
         }
      }

      [Observation]
      public void should_have_added_a_solubility_table_in_all_solubility_alternatives()
      {
         foreach (var solubilityAlternative in _allCompounds.Select(x => x.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_SOLUBILITY)).SelectMany(x => x.AllAlternatives))
         {
            checkSolubilityTableInContainer(solubilityAlternative);
         }
      }

      [Observation]
      public void should_have_added_a_solubility_table_in_all_compound_building_block_defined_in_simulation()
      {
         foreach (var compound in _allSimulations.SelectMany(x => x.Compounds))
         {
            checkSolubilityTableInContainer(compound);
         }
      }

      private void checkSolubilityTableInContainer(IContainer container)
      {
         var solubilityTable = container.Parameter(CoreConstants.Parameters.SOLUBILITY_TABLE);
         solubilityTable.ShouldNotBeNull();
         solubilityTable.Visible.ShouldBeFalse();
         solubilityTable.Value.ShouldBeEqualTo(0);
      }
   }
}