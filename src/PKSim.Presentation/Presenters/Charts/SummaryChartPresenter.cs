using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BTS.UI.TreeList;
using BTS.Utility.Events;
using BTS.Utility.Extensions;
using BTS.Utility.Licensing;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;
using PKSim.Resources;
using SBSuite.Core.Domain.Data;
using SBSuite.Core.Events;
using SBSuite.Presentation.Chart.Presenters;
using SBSuite.Presentation.Chart.Services;
using SBSuite.Presentation.Core;
using SBSuite.Presentation.Extensions;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface ISummaryChartPresenter : IChartPresenter<SummaryChart>,
      IListener<SimulationResultsUpdatedEvent>,
      IListener<BuildingBlockRemovedEvent>,
      IListener<SwapSimulationEvent>
   {
      void Edit(object subject);
      void Edit(SummaryChart chart);
      object Subject { get; }
      string ChartName { get; }
      void DragOver(object sender, DragEventArgs e);
      void DragDrop(object sender, DragEventArgs e);
      bool AnyCurves();
      void Clear();
   }

   public class SummaryChartPresenter : ChartPresenter<SummaryChart, ISummaryChartView, ISummaryChartPresenter>, ISummaryChartPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IChartTemplatingTask _chartTemplatingTask;
      public event EventHandler Closing = delegate { };

      public SummaryChartPresenter(
         ISummaryChartView view, 
         IChartEditorAndDisplayPresenter chartEditorAndDisplayPresenter,
         IIndividualPKAnalysisPresenter pkAnalysisPresenter, 
         IDataColumnToPathElementsMapper dataColumnToPathElementsMapper, 
         IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper,
         IChartTask chartTask, 
         IObservedDataTask observedDataTask, 
         ILazyLoadTask lazyLoadTask, 
         ILicense license, 
         IChartEditorLayoutTask chartEditorLayoutTask,
         IChartTemplatingTask chartTemplatingTask) :
            base(view, chartEditorAndDisplayPresenter, pkAnalysisPresenter, dataColumnToPathElementsMapper, quantityDisplayPathMapper, chartTask, observedDataTask, license, chartEditorLayoutTask, chartTemplatingTask)
      {
         _lazyLoadTask = lazyLoadTask;
         _chartTemplatingTask = chartTemplatingTask;
      }

      protected override void InitializeButtons()
      {
         AddSaveSettingsButton();
         base.InitializeButtons();
      }

      public void DragDrop(object sender, DragEventArgs e)
      {
         OnDragDrop(sender, e);
      }

      public bool AnyCurves()
      {
         return _repositoryCache != null && _repositoryCache.Any();
      }

      protected override void OnDragDrop(object sender, DragEventArgs e)
      {
         var droppedNodes = e.Data<IReadOnlyList<ITreeNode>>();
         if (containsIndividualSimulationNodes(droppedNodes))
            individualSimulationsFrom(droppedNodes).Each(addSimulationToChart);
         else
            base.OnDragDrop(sender, e);
      }

      public void DragOver(object sender, DragEventArgs e)
      {
         OnDragOver(sender, e);
      }

      protected override void OnDragOver(object sender, DragEventArgs e)
      {
         var draggedNodes = e.Data<IReadOnlyList<ITreeNode>>();
         if (containsIndividualSimulationNodes(draggedNodes))
            e.Effect = DragDropEffects.Move;
         else
            base.OnDragOver(sender, e);
      }

      private static IEnumerable<IndividualSimulation> individualSimulationsFrom(IReadOnlyList<ITreeNode> treeNodes)
      {
         return treeNodes.OfType<SimulationNode>().Select(x => x.Simulation).OfType<IndividualSimulation>();
      }

      public override void AddObservedData(DataRepository observedData, bool asResultOfDragAndDrop)
      {
         base.AddObservedData(observedData, asResultOfDragAndDrop);
         showChartView();
      }

      private void addSimulationToChart(IndividualSimulation simulation)
      {
         _lazyLoadTask.Load(simulation);
         if (!simulation.HasResults)
            throw new PKSimException(PKSimConstants.Error.SimulationHasNoResultsAndCannotBeUsedInSummaryChart(simulation.Name));

         Chart.AddSimulation(simulation);
         SetDataSource(simulation, simulation.DataRepository);
         
         if (!Chart.Curves.Any())
            InitializeFromTemplate();
         else
            _chartTemplatingTask.UpdateDefaultSettings(_chartEditorPresenter, simulation.DataRepository.ToList(), new[] {simulation});

         showChartView();
      }

      private void showChartView()
      {
         _view.SetChartView(_chartEditorAndDisplayPresenter.Control);
      }

      protected override string NameForColumns(DataColumn dataColumn)
      {
         return _quantityDisplayPathMapper.DisplayNameFor(SimulationFor(dataColumn), dataColumn, true);
      }

      private bool containsIndividualSimulationNodes(IEnumerable<ITreeNode> simulationNodes)
      {
         if (simulationNodes == null)
            return false;

         return simulationNodes.OfType<SimulationNode>().Any(x => x.Simulation.IsAnImplementationOf<IndividualSimulation>());
      }

      protected override void ConfigureEditor()
      {

         Column(BrowserColumns.RepositoryName).Visible = true;
         Column(BrowserColumns.RepositoryName).VisibleIndex = 1;
         Column(BrowserColumns.RepositoryName).GroupIndex = -1;

         Column(BrowserColumns.Container).Visible = true;
         Column(BrowserColumns.Container).Caption = PKSimConstants.UI.Organ;
         Column(BrowserColumns.Container).GroupIndex = 0;
         Column(BrowserColumns.Container).VisibleIndex = 0;

         Column(BrowserColumns.Molecule).Visible = true;
         Column(BrowserColumns.Molecule).Caption = PKSimConstants.UI.Molecule;
         Column(BrowserColumns.Molecule).VisibleIndex = 2;

         Column(BrowserColumns.BottomCompartment).Visible = true;
         Column(BrowserColumns.BottomCompartment).Caption = PKSimConstants.UI.Compartment;
         Column(BrowserColumns.BottomCompartment).VisibleIndex = 3;
         Column(BrowserColumns.BottomCompartment).SortColumnName = BrowserColumns.OrderIndex.ToString();

         Column(BrowserColumns.Name).Visible = true;
         Column(BrowserColumns.Name).VisibleIndex = 4;
         Column(BrowserColumns.Name).Caption = PKSimConstants.UI.Name;

         Column(BrowserColumns.Used).Visible = true;
         Column(BrowserColumns.Used).VisibleIndex = 5;
      }

      public void Handle(RenamedEvent eventToHandle)
      {
         //nothing to do here as renamed is handle automatically with change event
      }

      public virtual void OnFormClosed()
      {
         Closing(this, EventArgs.Empty);
         Clear();
      }

      public object Subject
      {
         get { return Chart; }
      }

      public string ChartName
      {
         get { return Chart.Name; }
      }


      public virtual void RestoreSettings(IPresenterSettings settings)
      {
         //per default do nothing
      }

      public void Edit(object subject)
      {
         Edit(subject.DowncastTo<SummaryChart>());
      }

      public void Edit(SummaryChart chart)
      {
         editChart(chart);
      }

      private void editChart(SummaryChart chart)
      {
         InitializeChart(chart);
         updateResultsInChart();
      }

      private void updateResultsInChart()
      {
         Chart.AllSimulations().Each(s => SetDataSource(s, s.DataRepository));
      }

      public void Handle(SimulationResultsUpdatedEvent eventToHandle)
      {
         if (!canHandle(eventToHandle.Simulation))
            return;

         updateResultsInChart();
      }

      private bool canHandle(Simulation simulation)
      {
         if (simulation == null)
            return false;
         return _repositoryCache.Contains(simulation);
      }

      public void Handle(BuildingBlockRemovedEvent removedEvent)
      {
         if (removedEvent.DueToSwap) return;

         var simulation = removedEvent.BuildingBlock as IndividualSimulation;
         if (!canHandle(simulation))
            return;

         removeSimulation(simulation);
      }

      private void removeSimulation(IndividualSimulation simulation)
      {
         var repo = DataRepositoryFor(simulation);
         if (repo == null) return;
         _repositoryCache.Remove(repo);
         _chartEditorPresenter.RemoveDataRepository(repo);
         Chart.RemoveSimulation(simulation);
      }

      public void Handle(SwapSimulationEvent eventToHandle)
      {
         if (!canHandle(eventToHandle.OldSimulation))
            return;

         var individualSimulation = eventToHandle.OldSimulation.DowncastTo<IndividualSimulation>();
         var newIndividualSimulation = eventToHandle.NewSimulation as IndividualSimulation;
         if (newIndividualSimulation != null && newIndividualSimulation.HasResults)
         {
            //simply replace old simulation with new simulation
            _repositoryCache.Remove(individualSimulation.DataRepository);
            SetDataSource(newIndividualSimulation, newIndividualSimulation.DataRepository);
         }
         else
            removeSimulation(individualSimulation);
      }
   }
}