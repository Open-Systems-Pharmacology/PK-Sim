using System;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Presentation.Regions;
using OSPSuite.Core;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Presenters.Commands;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using IWorkspace = PKSim.Presentation.Core.IWorkspace;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IHistoryPresenter : IMainViewItemPresenter,
      IListener<ProjectCreatedEvent>,
      IListener<ProjectClosedEvent>,
      IListener<ProjectLoadedEvent>,
      IListener<SimulationRunFinishedEvent>,
      IListener<ObjectBaseConvertedEvent>
   {
   }

   public class HistoryPresenter : IHistoryPresenter
   {
      private readonly IHistoryBrowserPresenter _historyBrowserPresenter;
      private readonly IWorkspace _workspace;
      private readonly IExecutionContext _executionContext;
      private readonly IApplicationConfiguration _applicationConfiguration;
      private readonly IRegion _region;
      private bool _initialized;
      public event EventHandler StatusChanged = delegate { };

      public HistoryPresenter(IHistoryBrowserPresenter historyBrowserPresenter, IWorkspace workspace, IRegionResolver regionResolver, IExecutionContext executionContext, IApplicationConfiguration applicationConfiguration)
      {
         _historyBrowserPresenter = historyBrowserPresenter;
         _workspace = workspace;
         _executionContext = executionContext;
         _applicationConfiguration = applicationConfiguration;
         _initialized = false;
         _region = regionResolver.RegionWithName(RegionNames.History);
         _region.Add(_historyBrowserPresenter.View);
      }

      public void Initialize()
      {
         if (_initialized) return;

         //initialize view layout
         int position = 0;
         HistoryColumns.State.Position = position++;
         HistoryColumns.ColumnByName(Constants.Command.BUILDING_BLOCK_TYPE).Position = position++;
         HistoryColumns.ColumnByName(Constants.Command.BUILDING_BLOCK_NAME).Position = position++;
         HistoryColumns.CommandType.Position = position++;
         HistoryColumns.ObjectType.Position = position++;
         HistoryColumns.Description.Position = position++;
         HistoryColumns.User.Position = position++;
         HistoryColumns.Time.Position = position++;

         _historyBrowserPresenter.Initialize();
         refreshHistory();
         _initialized = true;
      }

      public void ToggleVisibility()
      {
         if (!_region.IsVisible)
            _historyBrowserPresenter.UpdateHistory();
         _region.ToggleVisibility();
      }

      public void Handle(ProjectCreatedEvent eventToHandle)
      {
         refreshHistory();
      }

      public void Handle(ProjectClosedEvent eventToHandle)
      {
         refreshHistory();
      }

      private void refreshHistory()
      {
         _historyBrowserPresenter.HistoryManager = _workspace.HistoryManager;
         _historyBrowserPresenter.UpdateHistory();
      }

      public void Handle(ProjectLoadedEvent eventToHandle)
      {
         refreshHistory();
      }

      public bool CanClose => true;

      public void ViewChanged()
      {
      }

      public IView BaseView => null;

      public void Handle(SimulationRunFinishedEvent eventToHandle)
      {
         var command = new OSPSuiteInfoCommand
         {
            Description = PKSimConstants.Command.RunSimulationDescription(eventToHandle.Simulation.Name, eventToHandle.ExecutionTime.ToDisplay()),
            BuildingBlockName = eventToHandle.Simulation.Name,
            BuildingBlockType = PKSimConstants.ObjectTypes.Simulation,
            ObjectType = PKSimConstants.ObjectTypes.Simulation
         };

         executeCommandAndAddToWorkspace(command);
      }

      public void Handle(ObjectBaseConvertedEvent eventToHandle)
      {
         var buildingBlock = eventToHandle.ConvertedObject;
         var buildingBlockType = _executionContext.TypeFor(buildingBlock);
         var fromVersion = eventToHandle.FromVersion;

         var command = new OSPSuiteInfoCommand
         {
            Description = PKSimConstants.Command.ObjectConvertedDescription(buildingBlock.Name, buildingBlockType, fromVersion.VersionDisplay, _applicationConfiguration.Version),
            BuildingBlockName = buildingBlock.Name,
            BuildingBlockType = buildingBlockType,
            ObjectType = buildingBlockType
         };

         executeCommandAndAddToWorkspace(command);
      }

      private void executeCommandAndAddToWorkspace(OSPSuiteInfoCommand command)
      {
         command.Run(_executionContext);
         _workspace.AddCommand(command);
      }

      public void ReleaseFrom(IEventPublisher eventPublisher)
      {
         eventPublisher.RemoveListener(this);
      }
   }
}