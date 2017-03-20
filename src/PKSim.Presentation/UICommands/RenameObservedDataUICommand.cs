using OSPSuite.Core.Domain.Data;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class RenameObservedDataUICommand : ObjectUICommand<DataRepository>
   {
      private readonly IObservedDataTask _observedDataTask;

      public RenameObservedDataUICommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.Rename(Subject);
      }
   }
}