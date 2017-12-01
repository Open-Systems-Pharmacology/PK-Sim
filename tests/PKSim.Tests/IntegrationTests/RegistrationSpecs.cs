using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Comparison;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Comparison;
using PKSim.Core.Model;
using PKSim.Infrastructure.Reporting.Summary;

namespace PKSim.IntegrationTests
{
   public class When_registering_all_components_for_the_application_start : ContextForIntegration<CoreRegister>
   {
      [Observation]
      public void should_be_able_to_find_a_diff_builder_for_a_simulation()
      {
         var diffBuilderRepository = IoC.Resolve<IDiffBuilderRepository>();
         var simuationDiffBuilder  = diffBuilderRepository.BuilderFor(new IndividualSimulation());
         simuationDiffBuilder.ShouldBeAnInstanceOf<SimulationDiffBuilder>();
      }

      [Observation]
      public void should_be_able_to_find_a_report_builder_for_an_individual()
      {
         var reportBuilderRepository = IoC.Resolve<IReportBuilderRepository>();
         var reportBuilder = reportBuilderRepository.BuilderFor(new Individual());
         reportBuilder.ShouldBeAnInstanceOf<IndividualReportBuilder>();
      }
   }
}