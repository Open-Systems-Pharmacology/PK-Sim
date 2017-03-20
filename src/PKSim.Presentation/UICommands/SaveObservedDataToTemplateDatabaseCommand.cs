using System;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class SaveObservedDataToTemplateDatabaseCommand : ObjectUICommand<DataRepository>
   {
      private readonly IObservedDataTask _observedDataTask;

      public SaveObservedDataToTemplateDatabaseCommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.SaveToTemplate(Subject);
      }
   }
}