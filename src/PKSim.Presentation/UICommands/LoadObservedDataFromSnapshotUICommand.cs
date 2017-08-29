using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadObservedDataFromSnapshotUICommand : IUICommand
   {
      private readonly IObservedDataTask _observedDataTask;

      public LoadObservedDataFromSnapshotUICommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }
      public async void Execute()
      {
         await _observedDataTask.SecureAwait(x => x.LoadFromSnapshot());
      }
   }
}