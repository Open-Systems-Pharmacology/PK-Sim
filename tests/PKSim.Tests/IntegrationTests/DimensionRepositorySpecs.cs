using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DimensionRepository : ContextForIntegration<IDimensionRepository>
   {
       
   }

   public class When_retrieving_the_hard_coded_dimension_in_pksim : concern_for_DimensionRepository
   {
      [Observation]
      public void should_be_able_to_find_them()
      {
         foreach (var dimension in CoreConstants.Dimension.AllHardCoded)
         {
            sut.DimensionByName(dimension).ShouldNotBeNull();
         }  
      }
   }
}