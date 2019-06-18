using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationObserversConfigurationPresenter : ISimulationItemPresenter
   {
      IEnumerable<ObserverSet> AllUnmappedObserverSets(ObserverSetMappingDTO currentObserverSetMappingDTO = null);
      void AddObserverSet();
      void RemoveObserverSetMapping(ObserverSetMappingDTO observerSetMappingDTO);
      void LoadObserverSetFor(ObserverSetMappingDTO observerSetMappingDTO);
      void CreateObserverFor(ObserverSetMappingDTO observerSetMappingDTO);
      IEnumerable<ToolTipPart> ToolTipFor(ObserverSetMappingDTO observerSetMappingDTO);
      string DisplayNameFor(ObserverSet observerSet);
   }

   public class SimulationObserversConfigurationPresenter : AbstractSubPresenter<ISimulationObserversConfigurationView, ISimulationObserversConfigurationPresenter>, ISimulationObserversConfigurationPresenter
   {
      private readonly IObserverSetMappingToObserverSetMappingDTOMapper _observerSetMappingDTOMapper;
      private readonly IObserverSetTask _observerSetTask;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      private Simulation _simulation;
      private ObserverSetProperties _observerSetProperties;
      private readonly INotifyList<ObserverSetMappingDTO> _allObserverSetMappingDTOs = new NotifyList<ObserverSetMappingDTO>();

      public SimulationObserversConfigurationPresenter(
         ISimulationObserversConfigurationView view,
         IObserverSetMappingToObserverSetMappingDTOMapper observerSetMappingDTOMapper,
         IObserverSetTask observerSetTask,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater,
         IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer
      ) : base(view)
      {
         _observerSetMappingDTOMapper = observerSetMappingDTOMapper;
         _observerSetTask = observerSetTask;
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _buildingBlockSelectionDisplayer = buildingBlockSelectionDisplayer;
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         _simulation = simulation;
         _observerSetProperties = simulation.ObserverSetProperties;
         _allObserverSetMappingDTOs.Clear();
         _observerSetProperties.ObserverSetMappings.Each(addObserverSetMapping);
         _view.BindTo(_allObserverSetMappingDTOs);
      }

      public void SaveConfiguration()
      {
         _observerSetProperties.ClearObserverSetMappings();

         _allObserverSetMappingDTOs.Each(x =>
         {
            _observerSetTask.Load(x.ObserverSet);
            _observerSetProperties.AddObserverSetMapping(x.ObserverSetMapping);
         });

         _simulationBuildingBlockUpdater.UpdateMultipleUsedBuildingBlockInSimulationFromTemplate(_simulation, _allObserverSetMappingDTOs.Select(x => x.ObserverSet), PKSimBuildingBlockType.ObserverSet);
      }

      public IEnumerable<ObserverSet> AllUnmappedObserverSets(ObserverSetMappingDTO currentObserverSetMappingDTO = null)
      {
         var allAvailableObserverSets = _observerSetTask.All().Except(_allObserverSetMappingDTOs.Select(x => x.ObserverSet)).ToList();
         if (currentObserverSetMappingDTO?.ObserverSet != null)
            allAvailableObserverSets.Insert(0, currentObserverSetMappingDTO.ObserverSet);

         return allAvailableObserverSets.OrderBy(x => x.Name);
      }

      public void AddObserverSet()
      {
         var observerSetMapping = _observerSetTask.CreateObserverSetMapping(AllUnmappedObserverSets().FirstOrDefault());
         addObserverSetMapping(observerSetMapping);
         OnStatusChanged();
      }

      public void RemoveObserverSetMapping(ObserverSetMappingDTO observerSetMappingDTO)
      {
         _allObserverSetMappingDTOs.Remove(observerSetMappingDTO);
         OnStatusChanged();
      }

      public void LoadObserverSetFor(ObserverSetMappingDTO observerSetMappingDTO)
      {
         updateObserverSetInMapping(observerSetMappingDTO, _observerSetTask.LoadSingleFromTemplate());
      }

      public void CreateObserverFor(ObserverSetMappingDTO observerSetMappingDTO)
      {
         updateObserverSetInMapping(observerSetMappingDTO, _observerSetTask.AddToProject());
      }

      public IEnumerable<ToolTipPart> ToolTipFor(ObserverSetMappingDTO observerSetMappingDTO)
      {
         return _buildingBlockSelectionDisplayer.ToolTipFor(observerSetMappingDTO.ObserverSet);
      }

      public string DisplayNameFor(ObserverSet observerSet) => _buildingBlockSelectionDisplayer.DisplayNameFor(observerSet);

      private void updateObserverSetInMapping(ObserverSetMappingDTO observerSetMappingDTO, ObserverSet observerSet)
      {
         if (observerSet == null)
            return;

         observerSetMappingDTO.ObserverSet = observerSet;
        _view.RefreshData();
         OnStatusChanged();
      }

      private void addObserverSetMapping(ObserverSetMapping observerSetMapping)
      {
         _allObserverSetMappingDTOs.Add(_observerSetMappingDTOMapper.MapFrom(observerSetMapping, _simulation));
      }

      public override bool CanClose => base.CanClose && noDoubleSeletionOfObsererSet();

      private bool noDoubleSeletionOfObsererSet()
      {
         var allNames = _allObserverSetMappingDTOs.Select(x => x.ObserverSet?.Name).Where(x => !string.IsNullOrEmpty(x)).Distinct();
         return allNames.Count() == _allObserverSetMappingDTOs.Count;
      }
   }
}