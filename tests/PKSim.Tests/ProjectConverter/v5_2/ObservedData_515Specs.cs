using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_2
{
   public class When_converting_the_ObservedData_P515_project : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("ObservedData_515");
      }

      [Observation]
      public void should_have_converted_the_mol_weight_in_the_observed_data()
      {
         var observedData = FirstObservedData();
         foreach (var column in observedData.AllButBaseGrid())
         {
            column.DataInfo.MolWeight.ShouldBeEqualTo(250 * 1e-9);
         }
      }
   }
}