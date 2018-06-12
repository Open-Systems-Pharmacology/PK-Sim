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

      public void Execute()
      {
         _observedDataTask.LoadFromSnapshot();
      }
   }
}