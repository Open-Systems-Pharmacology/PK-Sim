using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core.Commands;

namespace PKSim.Presentation.Services
{
   public interface ISimulationSubjectTask : IBuildingBlockTask<ISimulationSubject>
   {
   }

   public class SimulationSubjectTask : ISimulationSubjectTask
   {
      private readonly IIndividualTask _individualTask;
      private readonly IPopulationTask _populationTask;
      private readonly IApplicationController _applicationController;
      private readonly IBuildingBlockTask _buildingBlockTask;

      public SimulationSubjectTask(IIndividualTask individualTask, IPopulationTask populationTask,
         IApplicationController applicationController, IBuildingBlockTask buildingBlockTask)
      {
         _individualTask = individualTask;
         _populationTask = populationTask;
         _applicationController = applicationController;
         _buildingBlockTask = buildingBlockTask;
      }

      public ISimulationSubject AddToProject()
      {
         using (var presenter = _applicationController.Start<ISimulationSubjectSelectionPresenter>())
         {
            if (!presenter.ChooseSimulationSubject())
               return null;

            if (presenter.SimulationSubjetType.IsAnImplementationOf<Population>())
               return _populationTask.AddToProject();

            return _individualTask.AddToProject();
         }
      }

      public IPKSimCommand AddToProject(ISimulationSubject buildingBlock, bool editBuildingBlock, bool addToHistory)
      {
         throw new NotSupportedException("Do not call AddToProject for a simulation subject. Use the dedicated method instead");
      }

      public void Edit(ISimulationSubject buildingBlockToEdit)
      {
         throw new NotSupportedException("Do not call Edit for a simulation subject. Use the dedicated method instead");
      }

      public IReadOnlyList<ISimulationSubject> LoadFromTemplate()
      {
         return _buildingBlockTask.LoadFromTemplate<ISimulationSubject>(PKSimBuildingBlockType.SimulationSubject);
      }

      public IReadOnlyList<ISimulationSubject> LoadFromSnapshot()
      {
         throw new NotSupportedException("Do not call LoadFromSnapshot for a simulation subject. Use the dedicated method instead");
      }

      public ISimulationSubject LoadSingleFromTemplate()
      {
         return LoadFromTemplate().FirstOrDefault();
      }

      public void Load(ISimulationSubject buildingBlockToLoad)
      {
         _buildingBlockTask.Load(buildingBlockToLoad);
      }

      public IEnumerable<ISimulationSubject> All()
      {
         return _buildingBlockTask.All<ISimulationSubject>();
      }

      public void SaveAsTemplate(ISimulationSubject buildingBlockToSave)
      {
         throw new NotSupportedException("Do not call SaveAsTemplate for a simulation subject. Use the dedicated method instead");
      }

      public void SaveAsSystemTemplate(ISimulationSubject buildingBlockToSave)
      {
         throw new NotSupportedException("Do not call SaveAsSystemTemplate for a simulation subject. Use the dedicated method instead");
      }
   }
}