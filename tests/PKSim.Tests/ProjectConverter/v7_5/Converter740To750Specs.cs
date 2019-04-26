using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v7_5;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v7_5
{
   public class When_converting_the_simple_project_730_project_to_750 : ContextWithLoadedProject<Converter740To750>
   {
      private List<Simulation> _allSimulations;
      private List<Compound> _allCompounds;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_730");
         _allSimulations = All<Simulation>().ToList();
         _allCompounds = All<Compound>().ToList();
         _allSimulations.Each(Load);
         _allCompounds.Each(Load);
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_enable_saturation_parameter_to_each_building_block_compound()
      {
         _allCompounds.Each(verifyEnableSaturationParameter);
      }

      private void verifyEnableSaturationParameter(Compound compound)
      {
         compound.SupersaturationEnabled.ShouldBeFalse();
         var enableSaturation = compound.Parameter(CoreConstants.Parameters.ENABLE_SUPERSATURATION);
         enableSaturation.CanBeVaried.ShouldBeTrue();
         enableSaturation.Info.ReadOnly.ShouldBeFalse();
         enableSaturation.Visible.ShouldBeTrue();
         enableSaturation.CanBeVariedInPopulation.ShouldBeFalse();
         enableSaturation.IsDefault.ShouldBeFalse();
      }

      [Observation]
      public void should_have_added_the_enable_saturation_parameter_to_each_simulation_compound()
      {
         _allSimulations.SelectMany(x=>x.Compounds).Each(verifyEnableSaturationParameter);
      }
   }
}