using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Presentation
{
   public abstract class concern_for_RunParameterIdentificationQualificationStepRunner : ContextSpecificationAsync<RunParameterIdentificationQualificationStepRunner>
   {
      protected IParameterIdentificationEngineFactory _parameterIdentificationEngineFactory;
      protected ITransferOptimizedParametersToSimulationsTask _transferOptimizedParametersToSimulationsTask;
      protected ILogger _logger;
      protected RunParameterIdentificationQualificationStep _runParameterIdentificationStep;
      protected ParameterIdentification _parameterIdentification;
      protected IParameterIdentificationEngine _parameterIdentificationEngine;

      protected override Task Context()
      {
         _parameterIdentificationEngineFactory = A.Fake<IParameterIdentificationEngineFactory>();
         _transferOptimizedParametersToSimulationsTask = A.Fake<ITransferOptimizedParametersToSimulationsTask>();
         _logger = A.Fake<ILogger>();
         sut = new RunParameterIdentificationQualificationStepRunner(_parameterIdentificationEngineFactory, _transferOptimizedParametersToSimulationsTask, _logger);

         _parameterIdentification = new ParameterIdentification();
         _runParameterIdentificationStep = new RunParameterIdentificationQualificationStep
         {
            ParameterIdentification = _parameterIdentification
         };

         _parameterIdentificationEngine = A.Fake<IParameterIdentificationEngine>();
         A.CallTo(() => _parameterIdentificationEngineFactory.Create()).Returns(_parameterIdentificationEngine);
         return _completed;
      }
   }

   public class When_running_a_parameter_identification_step : concern_for_RunParameterIdentificationQualificationStepRunner
   {
      protected override Task Because()
      {
         return sut.RunAsync((IQualificationStep)_runParameterIdentificationStep);
      }

      [Observation]
      public void should_have_debug_logged_the_fact_that_the_qualification_step_was_starting()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.Information.StartingQualificationStep(_runParameterIdentificationStep.Display), LogLevel.Debug, A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_create_a_new_parameter_identification_engine_and_run_the_parameter_identification_and_dispose_of_the_engine()
      {
         A.CallTo(() => _parameterIdentificationEngine.StartAsync(_parameterIdentification)).MustHaveHappened()
            .Then(A.CallTo(() => _parameterIdentificationEngine.Dispose()).MustHaveHappened());
      }
   }

   public class When_running_a_parameter_identification_step_and_the_parameter_identification_run_was_successful : concern_for_RunParameterIdentificationQualificationStepRunner
   {
      private ParameterIdentificationRunResult _result1;
      private ParameterIdentificationRunResult _result2;

      protected override async Task Context()
      {
         await base.Context();
         _result1 = A.Fake<ParameterIdentificationRunResult>();
         _result2 = A.Fake<ParameterIdentificationRunResult>();
         A.CallTo(() => _result1.TotalError).Returns(10);
         A.CallTo(() => _result2.TotalError).Returns(2);

         A.CallTo(() => _parameterIdentificationEngine.StartAsync(_parameterIdentification)).Invokes(x =>
         {
            _parameterIdentification.AddResult(_result1);
            _parameterIdentification.AddResult(_result2);
         });
      }

      protected override Task Because()
      {
         return sut.RunAsync(_runParameterIdentificationStep);
      }

      [Observation]
      public void should_transfer_the_best_results_to_the_simulation()
      {
         A.CallTo(() => _transferOptimizedParametersToSimulationsTask.TransferParametersFrom(_parameterIdentification, _result2)).MustHaveHappened();
      }

      [Observation]
      public void should_have_debug_logged_the_fact_that_the_results_were_transferred_to_the_simulation()
      {
         A.CallTo(() => _logger.AddToLog(PKSimConstants.QualificationSteps.ParameterIdentificationResultsTransferredToSimulations(_parameterIdentification.Name), LogLevel.Debug, A<string>._)).MustHaveHappened();
      }
   }
}