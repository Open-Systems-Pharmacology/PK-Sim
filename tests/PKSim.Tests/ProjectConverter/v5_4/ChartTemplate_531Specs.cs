using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ProjectConverter.v5_4;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v5_4
{
   public class When_converting_the_ChartTemplate_531_project : ContextWithLoadedProject<Converter532To541>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("ChartTemplate_531");
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }
   }
}