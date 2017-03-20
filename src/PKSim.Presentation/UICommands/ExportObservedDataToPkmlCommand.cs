using PKSim.Core.Services;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ExportObservedDataToPkmlCommand : ObjectUICommand<DataRepository>
   {
       private readonly IObservedDataTask _observedDataTask;

       public ExportObservedDataToPkmlCommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.ExportToPkml(Subject);
      }
   }
}