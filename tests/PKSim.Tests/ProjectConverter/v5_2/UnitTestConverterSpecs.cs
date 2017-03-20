using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public class When_converting_the_515_UnitTest_project : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("515_UnitTest");
      }

      [Observation]
      public void should_have_converted_the_dimension_of_SA_factor_and_surface_volume_ratio()
      {
         var organism = First<Individual>().Organism;
         organism.Parameter(ConverterConstants.Parameter.PARAM_k_SA)
            .Dimension.Name.ShouldBeEqualTo(CoreConstants.Dimension.InversedLength);

         organism.Parameter(ConverterConstants.Parameter.A_to_V_bc)
            .Dimension.Name.ShouldBeEqualTo(CoreConstants.Dimension.InversedLength);
      }
   }
}