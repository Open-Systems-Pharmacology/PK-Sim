using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;

namespace PKSim.BatchTool
{
   public abstract class concern_for_TwoPoresJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("two_pores");
      }
   }

   public class When_loading_the_simulation_defined_in_the_two_pores_json_file : concern_for_TwoPoresJson
   {
      [Observation]
      public void should_return_a_simulation_having_the_two_pores_model_as_configuration()
      {
         _simulation.ModelConfiguration.ModelName.ShouldBeEqualTo(CoreConstants.Model.TwoPores);
      }

      [Observation]
      public void should_return_a_simulation_having_the_rodger_and_roland_model_for_partition_coeff()
      {
         var compooundProperties = _simulation.CompoundPropertiesList.First();
         compooundProperties.CalculationMethodFor(CoreConstants.Category.DistributionCellular).Name
                    .ShouldBeEqualTo(ConverterConstants.CalculationMethod.RR);
      }
   }
}