using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.UICommands;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class DeleteUsedObservedDataUICommand : ObjectUICommand<IReadOnlyList<UsedObservedData>>
   {
      private readonly IObservedDataTask _observedDataTask;

      public DeleteUsedObservedDataUICommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.RemoveUsedObservedDataFromSimulation(Subject);
      }

      public DeleteUsedObservedDataUICommand For(UsedObservedData usedObservedData)
      {
         For(new[] {usedObservedData});
         return this;
      }
   }
}