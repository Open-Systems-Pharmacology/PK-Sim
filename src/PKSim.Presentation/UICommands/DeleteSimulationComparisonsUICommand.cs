using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class DeleteSimulationComparisonsUICommand : ObjectUICommand<IReadOnlyList<ISimulationComparison>>
   {
      private readonly IApplicationController _applicationController;
      private readonly IWorkspace _workspace;
      private readonly IEventPublisher _eventPublisher;
      private readonly IDialogCreator _dialogCreator;
      private readonly IRegistrationTask _registrationTask;

      public DeleteSimulationComparisonsUICommand(IApplicationController applicationController, IWorkspace workspace, IEventPublisher eventPublisher,
         IDialogCreator dialogCreator, IRegistrationTask registrationTask)
      {
         _applicationController = applicationController;
         _workspace = workspace;
         _eventPublisher = eventPublisher;
         _dialogCreator = dialogCreator;
         _registrationTask = registrationTask;
      }

      protected override void PerformExecute()
      {
         var res = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(Subject.AllNames()));
         if (res == ViewResult.No) return;
         Subject.Each(deleteSimulationComparison);
      }

      private void deleteSimulationComparison(ISimulationComparison simulationComparison)
      {
         _applicationController.Close(simulationComparison);
         _workspace.Project.RemoveSimulationComparison(simulationComparison);
         _registrationTask.Unregister(simulationComparison);
         _eventPublisher.PublishEvent(new SimulationComparisonDeletedEvent(_workspace.Project, simulationComparison));
      }
   }
}