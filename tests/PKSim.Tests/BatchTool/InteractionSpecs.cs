using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.BatchTool
{
   public abstract class concern_for_InteractionJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("interaction");
      }
   }

   public class When_loading_the_simulation_defined_in_the_interaction_json_file : concern_for_InteractionJson
   {
      [Observation]
      public void should_have_two_compounds()
      {
         var compounds = _simulation.AllBuildingBlocks<Compound>();
         compounds.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_two_applications()
      {
         var protocol = _simulation.AllBuildingBlocks<Protocol>();
         protocol.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_defined_an_interaction_for_the_compound()
      {
         _simulation.InteractionProperties.Any().ShouldBeTrue();
         _simulation.InteractionProperties.InteractingMoleculeNames.ShouldOnlyContain("CYP3A4");
      }
   }
}