using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public class When_converting_the_P_endothelial_515_project: ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         //Regression test for 47-5585 Klonen von Simulationen aus älterer Version (P endothelial)
         base.GlobalContext();
         LoadProject("P_endothelial_515");
      }

      [Observation]
      public void should_have_set_the_default_value_for_p_endothelial()
      {
         var sim = First<Simulation>();
         var neigh = sim.Model.Neighborhoods.GetSingleChildByName<INeighborhood>(ConverterConstants.Neighborhoods.HeartPlasmaToHeartInterstial);
         var p_endo = neigh.Container("Diclofenac").Parameter(ConverterConstants.Parameter.P_endothelial);
         p_endo.DefaultValue.ShouldNotBeNull();
         p_endo.IsFixedValue.ShouldBeTrue();
      }
   }
}	