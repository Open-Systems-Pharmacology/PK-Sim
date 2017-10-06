using OSPSuite.Utility.Events;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIProgressUpdater : IProgressUpdater
   {
      public void Dispose()
      {
      }

      public void Initialize(int numberOfIterations)
      {
      }

      public void Initialize(int numberOfIterations, string message)
      {
      }

      public void IncrementProgress()
      {
      }

      public void IncrementProgress(string message)
      {
      }

      public void ReportProgress(int iteration)
      {
      }

      public void ReportProgress(int iteration, string message)
      {
      }

      public void ReportStatusMessage(string message)
      {
      }

      public void Terminate()
      {
      }
   }
}