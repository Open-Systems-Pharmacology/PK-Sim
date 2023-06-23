using PKSim.IntegrationTests;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Infrastructure.ProjectConverter.v12;
using PKSim.Core.Model;

namespace PKSim.ProjectConverter.v12
{
   public class When_converting_the_simple_project_730_project_to_12 : ContextWithLoadedProject<Converter11To12>
   {
      private List<PopulationSimulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>();
         _allSimulations.Each(Load);
      }

      [Observation]
      public void all_simulation_should_have_settings()
      {
         _allSimulations.Each(simulation => simulation.Settings.ShouldNotBeNull());
      }
   }
}