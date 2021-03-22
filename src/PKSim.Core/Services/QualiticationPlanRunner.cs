using System.Threading.Tasks;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IQualiticationPlanRunner
   {
      Task RunAsync(QualificationPlan qualificationPlan);
   }

   public class QualiticationPlanRunner : IQualiticationPlanRunner
   {
      private readonly IQualificationStepRunnerFactory _qualificationStepRunnerFactory;
      private readonly IOSPSuiteLogger _logger;

      public QualiticationPlanRunner(IQualificationStepRunnerFactory qualificationStepRunnerFactory, IOSPSuiteLogger logger)
      {
         _qualificationStepRunnerFactory = qualificationStepRunnerFactory;
         _logger = logger;
      }

      public async Task RunAsync(QualificationPlan qualificationPlan)
      {
         _logger.AddDebug(PKSimConstants.Information.StartingQualificationPlan(qualificationPlan.Name));

         //this needs to be run in order. Await EACH run
         foreach (var qualificationStep in qualificationPlan.Steps)
         {
            using (var runner = _qualificationStepRunnerFactory.CreateFor(qualificationStep))
            {
               await runner.RunAsync(qualificationStep);
            }
         }
      }
   }
}