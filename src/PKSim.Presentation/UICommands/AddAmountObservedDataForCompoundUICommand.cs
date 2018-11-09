using OSPSuite.Presentation.UICommands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class AddAmountObservedDataForCompoundUICommand : ObjectUICommand<Compound>
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public AddAmountObservedDataForCompoundUICommand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.AddAmountDataToProjectForCompound(Subject);
      }
   }
}