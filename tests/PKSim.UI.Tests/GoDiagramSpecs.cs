using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.UI
{
   internal class concern_for_GoDiagram : StaticContextSpecification
   {
      [Observation]
      public void use_licensed_go_diagram_version_only()
      {
         Northwoods.Go.GoView.VersionName.ShouldBeEqualTo("5.2.0.46");
      }
   }
}
