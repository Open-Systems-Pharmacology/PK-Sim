using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ProjectConverter.v6_0;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;

namespace PKSim.ProjectConverter.V6_0
{
   public class When_converting_the_SimulationAndObservedData_531_project : ContextWithLoadedProject<Converter601To602>
   {
      private DataRepository _observedData;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimulationAndObservedData_531");
         _observedData = FirstObservedData();
      }

      [Observation]
      public void should_update_the_quantity_info_for_base_grids_in_observed_data()
      {
         _observedData.BaseGrid.QuantityInfo.PathAsString.ShouldBeEqualTo(new[] { _observedData.Name, _observedData.BaseGrid.Name }.ToPathString());
      }
   }
}