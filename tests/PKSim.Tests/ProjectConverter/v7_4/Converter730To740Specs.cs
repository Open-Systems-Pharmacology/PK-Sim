using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v7_4;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v7_4
{
   public class When_converting_the_simple_project_730_project : ContextWithLoadedProject<Converter730To740>
   {
      private List<Simulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_730");
         _allSimulations = All<Simulation>().ToList();
         _allSimulations.Each(Load);
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_parameter_tablet_time_delay_factor_to_variable_and_not_readonly_in_the_oral_simulations()
      {
         var oralSimulation = _allSimulations.FindByName("S1");
         var allParameters = oralSimulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameter.TabletTimeDelayFactor)).ToList();
         allParameters.Count.ShouldBeGreaterThan(0);
         allParameters.Each(p =>
         {
            p.Visible.ShouldBeTrue();
            p.Info.ReadOnly.ShouldBeFalse();
         });
      }
   }
}