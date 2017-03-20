using OSPSuite.Core.Domain.Data;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class DeleteObservedDataUICommand : ObjectUICommand<DataRepository>
   {
      private readonly IObservedDataTask _observedDataTask;

      public DeleteObservedDataUICommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.Delete(Subject);
      }
   }
}