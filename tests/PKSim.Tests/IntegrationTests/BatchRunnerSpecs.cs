using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_BatchRunnerSpecs<T> : ContextForIntegration<IBatchRunner<T>>
   {
      [Observation]
      public void should_be_able_to_instantiate_the_instance_of_the_batch_runner()
      {
         sut.ShouldNotBeNull();
      }
   }

   public class Should_be_able_to_create_a_batch_runner_for_json_run : concern_for_BatchRunnerSpecs<JsonRunOptions>
   {
   }

   public class Should_be_able_to_create_a_batch_runner_for_qualification_run : concern_for_BatchRunnerSpecs<QualificationRunOptions>
   {
   }

   public class Should_be_able_to_create_a_batch_runner_for_snapshot_run : concern_for_BatchRunnerSpecs<SnapshotRunOptions>
   {

   }

   public class Should_be_able_to_create_a_batch_runner_for_export_run : concern_for_BatchRunnerSpecs<ExportRunOptions>
   {
   }

}