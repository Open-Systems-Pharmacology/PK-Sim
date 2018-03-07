using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class RunParameterIdentificationQualificationStepRunner : QualificationStepRunner<RunParameterIdentificationQualificationStep>
   {
      private readonly IParameterIdentificationEngineFactory _parameterIdentificationEngineFactory;
      private readonly ITransferOptimizedParametersToSimulationsTask _transferOptimizedParametersToSimulationsTask;

      public RunParameterIdentificationQualificationStepRunner(
         IParameterIdentificationEngineFactory parameterIdentificationEngineFactory,
         ITransferOptimizedParametersToSimulationsTask transferOptimizedParametersToSimulationsTask,
         ILogger logger) : base(logger)
      {
         _parameterIdentificationEngineFactory = parameterIdentificationEngineFactory;
         _transferOptimizedParametersToSimulationsTask = transferOptimizedParametersToSimulationsTask;
      }

      public override async Task RunAsync(RunParameterIdentificationQualificationStep qualificationStep)
      {
         var parameterIdentification = qualificationStep.ParameterIdentification;
         using (var engine = _parameterIdentificationEngineFactory.Create())
         {
            await engine.StartAsync(parameterIdentification);

            if (parameterIdentification.HasResults)
            {
               _transferOptimizedParametersToSimulationsTask.TransferParametersFrom(parameterIdentification, parameterIdentification.Results.MinimumBy(x => x.TotalError));
               _logger.AddDebug(PKSimConstants.QualificationSteps.ParameterIdentificationResultsTransferredToSimulations(parameterIdentification.Name));
            }
         }
      }
   }
}