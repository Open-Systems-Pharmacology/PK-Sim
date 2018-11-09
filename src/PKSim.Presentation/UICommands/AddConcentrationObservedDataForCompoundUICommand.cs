using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class AddConcentrationObservedDataForCompoundUICommand : ObjectUICommand<Compound>
   {
      private readonly IImportObservedDataTask _observedDataTask;

      public AddConcentrationObservedDataForCompoundUICommand(IImportObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.AddConcentrationDataToProjectForCompound(Subject);
      }
   }
}