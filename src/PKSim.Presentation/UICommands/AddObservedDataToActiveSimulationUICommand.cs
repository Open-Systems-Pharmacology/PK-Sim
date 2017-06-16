using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility.Reflection;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class AddObservedDataToActiveSimulationUICommand : ObjectUICommand<Simulation>
   {
      private readonly IObservedDataTask _observedDataTask;
      private WeakRef<DataRepository> _observedDataReference;

      public AddObservedDataToActiveSimulationUICommand(IObservedDataTask observedDataTask)
      {
         _observedDataTask = observedDataTask;
      }

      protected override void PerformExecute()
      {
         _observedDataTask.AddObservedDataToAnalysable(new[] {_observedDataReference.Target}, Subject, showData: true);
      }

      public AddObservedDataToActiveSimulationUICommand For(DataRepository observedData)
      {
         _observedDataReference = new WeakRef<DataRepository>(observedData);
         return this;
      }
   }
}