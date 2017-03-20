using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadObservedDataFromTemplateUICommand : IUICommand
   {
      private readonly IObservedDataTask _observedDataTask;

      public LoadObservedDataFromTemplateUICommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      public void Execute()
      {
         _observedDataTask.LoadObservedDataFromTemplate();
      }
   }
}