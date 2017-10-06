using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.BatchTool
{
   public abstract class concern_for_OralJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("oral");
      }
   }

   public class When_running_the_simulation_defined_in_the_oral_file : concern_for_OralJson
   {
      [Observation]
      public void should_have_create_a_simulation_using_the_lint80_simulation()
      {
         var formulation = _simulation.BuildingBlock<Formulation>();
         formulation.FormulationType.ShouldBeEqualTo(CoreConstants.Formulation.Lint80);
         formulation.Parameter(CoreConstants.Parameter.DISS_TIME80).Value.ShouldBeEqualTo(60);
         formulation.Parameter(CoreConstants.Parameter.LAG_TIME).Value.ShouldBeEqualTo(10);

         var appContainer = _simulation.Model.Root.GetSingleChildByName<IContainer>(OSPSuite.Core.Domain.Constants.APPLICATIONS);
         var formContainer = appContainer.GetAllChildren<IContainer>().FindByName(formulation.Name);
         formContainer.ShouldNotBeNull();
      }
   }
}