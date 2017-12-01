using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services.Charts;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;
using IChartTemplatingTask = PKSim.Presentation.Services.IChartTemplatingTask;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface IIndividualSimulationComparisonPresenter : IChartPresenter<IndividualSimulationComparison>,
      IPKAnalysisWithChartPresenter,
      IListener<SimulationResultsUpdatedEvent>,
      IListener<BuildingBlockRemovedEvent>,
      IListener<SwapSimulationEvent>
   {
      void Edit(object subject);
      void Edit(IndividualSimulationComparison individualSimulationComparison);
      object Subject { get; }
      string ChartName { get; }
      void DragOver(object sender, DragEventArgs e);
      void DragDrop(object sender, DragEventArgs e);
      bool AnyCurves();
      void Clear();
   }

   public class IndividualSimulationComparisonPresenter : ChartPresenter<IndividualSimulationComparison, IIndividualSimulationComparisonView, IIndividualSimulationComparisonPresenter>, IIndividualSimulationComparisonPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;

      public event EventHandler Closing = delegate { };

      public IndividualSimulationComparisonPresenter(IIndividualSimulationComparisonView view, ChartPresenterContext chartPresenterContext, IIndividualPKAnalysisPresenter pkAnalysisPresenter, IChartTask chartTask, IObservedDataTask observedDataTask, ILazyLoadTask lazyLoadTask, IChartTemplatingTask chartTemplatingTask, IChartUpdater chartUpdater) :
         base(view, chartPresenterContext, chartTemplatingTask, pkAnalysisPresenter, chartTask, observedDataTask, chartUpdater)
      {
         _lazyLoadTask = lazyLoadTask;
         PresentationKey = PresenterConstants.PresenterKeys.IndividualSimulationComparisonPresenter;
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

      protected override void AddObservedData(IReadOnlyList<DataRepository> observedData, bool asResultOfDragAndDrop)
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
         UpdateAnalysisBasedOn(simulation, simulation.DataRepository);

         _chartTemplatingTask.UpdateDefaultSettings(ChartEditorPresenter, simulation.DataRepository.ToList(), new[] {simulation}, addCurveIfNoSourceDefined: false);
         InitializeFromTemplateIfRequired();

         showChartView();
      }

      private void showChartView()
      {
         _view.SetChartView(_chartPresenterContext.EditorAndDisplayPresenter.BaseView);
      }

      protected override string NameForColumn(DataColumn dataColumn)
      {
         return _chartPresenterContext.CurveNamer.CurveNameForColumn(SimulationFor(dataColumn), dataColumn);
      }

      private bool containsIndividualSimulationNodes(IEnumerable<ITreeNode> simulationNodes)
      {
         if (simulationNodes == null)
            return false;

         return simulationNodes.OfType<SimulationNode>().Any(x => x.Simulation.IsAnImplementationOf<IndividualSimulation>());
      }

      protected override void ConfigureColumns()
      {
         base.ConfigureColumns();
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

      public object Subject => Chart;

      public string ChartName => Chart.Name;

      public virtual void RestoreSettings(IPresentationSettings settings)
      {
         //per default do nothing
      }

      public void Edit(object subject)
      {
         Edit(subject.DowncastTo<IndividualSimulationComparison>());
      }

      public void Edit(IndividualSimulationComparison individualSimulationComparison)
      {
         editChart(individualSimulationComparison);
      }

      private void editChart(IndividualSimulationComparison chart)
      {
         InitializeAnalysis(chart);
         UpdateTemplatesBasedOn(_chartPresenterContext.ProjectRetriever.CurrentProject);
         BindChartToEditors();
         updateResultsInChart();
         LoadSettingsForSubject(chart);
      }

      private void updateResultsInChart()
      {
         Chart.AllSimulations.Each(s => UpdateAnalysisBasedOn(s, s.DataRepository));
      }

      public void Handle(SimulationResultsUpdatedEvent eventToHandle)
      {
         if (!canHandle(eventToHandle.Simulation as Simulation))
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
         ChartEditorPresenter.RemoveDataRepositories(new []{repo});
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
            UpdateAnalysisBasedOn(newIndividualSimulation, newIndividualSimulation.DataRepository);
         }
         else
            removeSimulation(individualSimulation);
      }
   }
}