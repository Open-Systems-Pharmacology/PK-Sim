using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Presentation
{
   public abstract class concern_for_QualiticationPlanRunner : ContextSpecificationAsync<IQualiticationPlanRunner>
   {
      protected IQualificationStepRunnerFactory _qualificationStepRunnerFactory;
      protected ILogger _logger;

      protected override Task Context()
      {
         _qualificationStepRunnerFactory = A.Fake<IQualificationStepRunnerFactory>();
         _logger = A.Fake<ILogger>();
         sut = new QualiticationPlanRunner(_qualificationStepRunnerFactory, _logger);

         return _completed;
      }
   }

   public class When_running_a_qualification_plan : concern_for_QualiticationPlanRunner
   {
      private QualificationPlan _qualificationPlan;
      private IQualificationStep _step1;
      private IQualificationStep _step2;
      private IQualificationStepRunner _qualificationStepRunner1;
      private IQualificationStepRunner _qualificationStepRunner2;

      protected override async Task Context()
      {
         await base.Context();
         _step1 = A.Fake<IQualificationStep>();
         _step2 = A.Fake<IQualificationStep>();
         _qualificationPlan = new QualificationPlan {_step1, _step2}.WithName("QP");

         _qualificationStepRunner1 = A.Fake<IQualificationStepRunner>();
         _qualificationStepRunner2 = A.Fake<IQualificationStepRunner>();

         A.CallTo(() => _qualificationStepRunnerFactory.CreateFor(_step1)).Returns(_qualificationStepRunner1);
         A.CallTo(() => _qualificationStepRunnerFactory.CreateFor(_step2)).Returns(_qualificationStepRunner2);
      }

      protected override Task Because()
      {
         return sut.RunAsync(_qualificationPlan);
      }

      [Observation]
      public void should_log_debug_the_fact_that_the_qualification_run_is_starting()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.Information.StartingQualificationPlan(_qualificationPlan.Name), LogLevel.Debug, A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_iterate_through_all_qualification_steps_in_order_and_execute_them()
      {
         A.CallTo(() => _qualificationStepRunner1.RunAsync(_step1)).MustHaveHappened()
            .Then(A.CallTo(() => _qualificationStepRunner2.RunAsync(_step2)).MustHaveHappened());
      }

      [Observation]
      public void should_have_disposed_of_all_qualification_step_runners_created_during_the_run()
      {
         A.CallTo(() => _qualificationStepRunner1.Dispose()).MustHaveHappened();
         A.CallTo(() => _qualificationStepRunner2.Dispose()).MustHaveHappened();
      }
   }
}