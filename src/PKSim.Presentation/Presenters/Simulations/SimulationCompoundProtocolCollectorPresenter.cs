using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundProtocolCollectorPresenter : ISimulationItemPresenter
   {
      void UpdateSelectedProtocol(Protocol templateProtocol);
      void UpdateSelectedFormulation(Formulation templateFormulation);
   }

   public class SimulationCompoundProtocolCollectorPresenter : SimulationCompoundCollectorPresenterBase<ISimulationCompoundProtocolCollectorView, ISimulationCompoundProtocolPresenter>, ISimulationCompoundProtocolCollectorPresenter
   {
      private readonly IProtocolChartPresenter _protocolChartPresenter;
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private bool _hasWarnings;

      public SimulationCompoundProtocolCollectorPresenter(ISimulationCompoundProtocolCollectorView view, IApplicationController applicationController,
         IConfigurableLayoutPresenter configurableLayoutPresenter, IEventPublisher eventPublisher, IProtocolChartPresenter protocolChartPresenter,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater) : base(view, applicationController, configurableLayoutPresenter, eventPublisher)
      {
         view.ApplicationIcon = ApplicationIcons.Administration;
         view.Caption = PKSimConstants.UI.SimulationApplicationConfiguration;
         view.AddProtocolChart(protocolChartPresenter.BaseView);
         _protocolChartPresenter = protocolChartPresenter;
         AddSubPresenters(_protocolChartPresenter);
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _hasWarnings = false;
      }

      public override void SaveConfiguration()
      {
         base.SaveConfiguration();
         //once the configuration is saved,needs to add all building blocks to the simulation

         _simulationBuildingBlockUpdater.UpdateProtocolsInSimulation(_simulation);
         _simulationBuildingBlockUpdater.UpdateFormulationsInSimulation(_simulation);
      }

      public void UpdateSelectedProtocol(Protocol templateProtocol)
      {
         //find presenter using a protocol with the same name and update the selection
         var presenter = allSubPresentersWithDefinedProtocol.FirstOrDefault(x => string.Equals(x.SelectedProtocol.Name, templateProtocol.Name));
         presenter?.ProtocolSelectionChanged(templateProtocol);
      }

      public void UpdateSelectedFormulation(Formulation templateFormulation)
      {
         allSubPresentersWithDefinedProtocol.Each(x=>x.UpdateSelectedFormulation(templateFormulation));
      }

      private IReadOnlyList<Protocol> selectedProtocols => selectedProtocolsByCompounds.ToList();

      private ICache<Compound, Protocol> selectedProtocolsByCompounds
      {
         get
         {
            var cache = new Cache<Compound, Protocol>();
            allSubPresentersWithDefinedProtocol.Each(p => cache[p.Compound] = p.SelectedProtocol);
            return cache;
         }
      }

      private IEnumerable<ISimulationCompoundProtocolPresenter> allSubPresentersWithDefinedProtocol
      {
         get { return AllSubCompoundPresenters().Where(x => x.SelectedProtocol != null); }
      }

      public override void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         base.EditSimulation(simulation, creationMode);
         bool emptyProtocolSelectionAllowed = simulation.Compounds.Count > 1;
         AllSubCompoundPresenters().Each(p => p.AllowEmptyProtocolSelection = emptyProtocolSelectionAllowed);
         refreshView();
      }

      protected override void OnStatusChanged(object sender, EventArgs e)
      {
         //this needs to be done before propagating the StatusChanged event
         refreshView();
         base.OnStatusChanged(sender, e);
      }

      private void refreshView()
      {
         updateWarnings();
         updateCharts();
      }

      private void updateCharts()
      {
         _protocolChartPresenter.PlotProtocols(selectedProtocolsByCompounds);
      }

      private void updateWarnings()
      {
         var protocols = selectedProtocols;
         if (!protocols.Any())
         {
            _hasWarnings = true;
            _view.Warning = PKSimConstants.Error.AtLeastOneProtocolRequiredToCreateSimulation;
         }
         else if (protocols.Distinct().Count() != protocols.Count)
         {
            _hasWarnings = true;
            _view.Warning = PKSimConstants.Error.AProtocolCanOnlyBeUsedOnceInASimulation;
         }
         else
         {
            _hasWarnings = false;
            _view.Warning = string.Empty;
         }
         _view.WarningVisible = _hasWarnings;
      }

      public override bool CanClose => base.CanClose && !_hasWarnings;
   }
}