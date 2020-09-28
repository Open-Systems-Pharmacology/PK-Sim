using System;
using System.Threading.Tasks;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IQualificationStepRunner : IDisposable
   {
      Task RunAsync(IQualificationStep qualificationStep);
   }

   public abstract class QualificationStepRunner<T> : IQualificationStepRunner where T : IQualificationStep
   {
      protected readonly IOSPLogger _logger;

      protected QualificationStepRunner(IOSPLogger logger)
      {
         _logger = logger;
      }

      protected virtual void Cleanup()
      {
      }

      public async Task RunAsync(IQualificationStep qualificationStep)
      {
         _logger.AddDebug(PKSimConstants.Information.StartingQualificationStep(qualificationStep.Display));
         await RunAsync(qualificationStep.DowncastTo<T>());
      }

      public abstract Task RunAsync(T qualificationStep);

      #region Disposable properties

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         Cleanup();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~QualificationStepRunner()
      {
         Cleanup();
      }

      #endregion
   }
}