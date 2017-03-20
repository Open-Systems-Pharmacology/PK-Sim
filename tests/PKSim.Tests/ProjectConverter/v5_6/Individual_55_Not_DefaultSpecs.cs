using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v5_6;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_6
{
   public class When_converting_the_Individual_55_Not_Default_project : ContextWithLoadedProject<Converter552To561>
   {
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Individual_55_Not_Default");
         _individual = First<Individual>();
      }

      [Observation]
      public void should_have_converted_the_expected_liver_values()
      {
         var volume = _individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.Liver, Constants.Parameters.VOLUME);
         volume.Value.ShouldBeEqualTo(0.187,1e-2);

         var bloodFlow = _individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.Liver, CoreConstants.Parameter.BLOOD_FLOW);
         bloodFlow.Value.ShouldBeEqualTo(0.03944, 1e-2);
      }
   }
}