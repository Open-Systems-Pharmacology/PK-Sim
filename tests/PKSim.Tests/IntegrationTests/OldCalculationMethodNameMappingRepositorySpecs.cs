using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_OldCalculationMethodNameMappingRepository : ContextForIntegration<IOldCalculationMethodNameMappingRepository>
   {
   }

   public class When_retrieving_all_old_calculationMethodname_mappings_from_the_repository : concern_for_OldCalculationMethodNameMappingRepository
   {
      private const string _oldName = "DiffusionIntCell1_WS";
      private const string _newName = "Cellular permeability - Charge dependent Schmitt";

      protected override void Because()
      {
      }

      [Observation]
      public void should_return_old_name_for_DiffusionIntCell1_WS()
      {
         sut.OldCalculationMethodNameFrom(_newName).ShouldBeEqualTo(_oldName);
      }

      [Observation]
      public void should_return_new_name_for_DiffusionIntCell1_WS()
      {
         sut.NewCalculationMethodNameFrom(_oldName).ShouldBeEqualTo(_newName);
      }

      [Observation]
      public void should_return_old_name_for_nonexisting()
      {
         const string name = "Trululu";
         sut.NewCalculationMethodNameFrom(name).ShouldBeEqualTo(name);
      }
   }
}