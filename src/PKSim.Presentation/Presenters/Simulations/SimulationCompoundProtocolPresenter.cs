using System;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundProtocolPresenter : IEditSimulationCompoundPresenter
   {
      void ProtocolSelectionChanged(Protocol selectedProtocol);
      bool ProtocolChanged { get; }
      bool FormulationChanged { get; }
      Protocol SelectedProtocol { get; }
      Compound Compound { get; }
      bool AllowEmptyProtocolSelection { get; set; }
      void UpdateSelectedFormulation(Formulation templateFormulation);
   }

   public class SimulationCompoundProtocolPresenter : AbstractSubPresenter<ISimulationCompoundProtocolView, ISimulationCompoundProtocolPresenter>, ISimulationCompoundProtocolPresenter
   {
      private readonly ISimulationCompoundProtocolFormulationPresenter _simulationCompoundProtocolFormulationPresenter;
      private readonly ISimulationCompoundProtocolEventPresenter _simulationCompoundProtocolEventPresenter;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private Simulation _simulation;
      private ProtocolSelectionDTO _protocolSelectionDTO;
      private ProtocolProperties _protocolProperties;
      public Compound Compound { get; private set; }
      public bool FormulationChanged { get; private set; }
      public bool EventChanged { get; private set; }
      public bool ProtocolChanged { get; private set; }

      public SimulationCompoundProtocolPresenter(
         ISimulationCompoundProtocolView view,
         ISimulationCompoundProtocolFormulationPresenter simulationCompoundProtocolFormulationPresenter,
         ISimulationCompoundProtocolEventPresenter simulationCompoundProtocolEventPresenter,
         ILazyLoadTask lazyLoadTask,
         IBuildingBlockInProjectManager buildingBlockInProjectManager)
         : base(view)
      {
         _simulationCompoundProtocolFormulationPresenter = simulationCompoundProtocolFormulationPresenter;
         _simulationCompoundProtocolEventPresenter = simulationCompoundProtocolEventPresenter;
         _lazyLoadTask = lazyLoadTask;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _view.AddFormulationMappingView(_simulationCompoundProtocolFormulationPresenter.View);
         _view.AddEventMappingView(_simulationCompoundProtocolEventPresenter.View);
         _simulationCompoundProtocolFormulationPresenter.StatusChanged += onFormulationChanged;
         _simulationCompoundProtocolEventPresenter.StatusChanged += onEventChanged;
      }

      private void onFormulationChanged(object sender, EventArgs e)
      {
         FormulationChanged = true;
         OnStatusChanged();
      }

      private void onEventChanged(object sender, EventArgs e)
      {
         EventChanged = true;
         OnStatusChanged();
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         _simulation = simulation;
         Compound = compound;
         _protocolProperties = simulation.CompoundPropertiesFor(compound).ProtocolProperties;
         var templateProtocol = _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy(_simulation,_protocolProperties.Protocol);
         _protocolSelectionDTO = new ProtocolSelectionDTO { BuildingBlock = templateProtocol };
         _view.BindTo(_protocolSelectionDTO);
       //  _view.AdjustHeight();
         updateActiveProtocol();
      }

      private void updateActiveProtocol()
      {
         _lazyLoadTask.Load(SelectedProtocol);
         _protocolProperties.Protocol = SelectedProtocol;
         _simulationCompoundProtocolFormulationPresenter.EditSimulation(_simulation, Compound);
         _simulationCompoundProtocolEventPresenter.EditSimulation(_simulation, Compound);
         _view.EventDescriptionVisible = _simulationCompoundProtocolEventPresenter.EventVisible;
      }

      public void UpdateSelectedFormulation(Formulation templateFormulation)
      {
         _simulationCompoundProtocolFormulationPresenter.UpdateSelectedFormulation(templateFormulation);
      }

      public void ProtocolSelectionChanged(Protocol selectedProtocol)
      {
         if (Equals(SelectedProtocol, selectedProtocol))
            return;

         _protocolSelectionDTO.BuildingBlock = selectedProtocol;
         ProtocolChanged = true;
         updateActiveProtocol();
         ViewChanged();
      }

      public void SaveConfiguration()
      {
         _simulationCompoundProtocolFormulationPresenter.SaveConfiguration();
         _simulationCompoundProtocolEventPresenter.SaveConfiguration();
      }

      public override bool CanClose => base.CanClose && _simulationCompoundProtocolFormulationPresenter.CanClose && _simulationCompoundProtocolEventPresenter.CanClose;

      public Protocol SelectedProtocol => _protocolSelectionDTO.BuildingBlock;

      public bool AllowEmptyProtocolSelection
      {
         set => _view.AllowEmptyProtocolSelection = value;
         get => _view.AllowEmptyProtocolSelection;
      }

  
   }
}