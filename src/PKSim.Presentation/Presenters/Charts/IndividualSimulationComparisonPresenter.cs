using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services.Charts;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Simulations;
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
      void DragOver(object sender, IDragEvent e);
      void DragDrop(object sender, IDragEvent e);
      bool AnyCurves();
      void Clear();
   }

   public class IndividualSimulationComparisonPresenter : ChartPresenter<IndividualSimulationComparison, IIndividualSimulationComparisonView, IIndividualSimulationComparisonPresenter>, IIndividualSimulationComparisonPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IDialogCreator _dialogCreator;

      public event EventHandler Closing = delegate { };

      public IndividualSimulationComparisonPresenter(
         IIndividualSimulationComparisonView view,
         ChartPresenterContext chartPresenterContext,
         IIndividualPKAnalysisPresenter pkAnalysisPresenter,
         IChartTask chartTask,
         IObservedDataTask observedDataTask,
         ILazyLoadTask lazyLoadTask,
         IChartTemplatingTask chartTemplatingTask,
         IChartUpdater chartUpdater,
         IUserSettings userSettings,
         IDialogCreator dialogCreator) :
         base(view, chartPresenterContext, chartTemplatingTask, pkAnalysisPresenter, chartTask, observedDataTask, chartUpdater, useSimulationNameToCreateCurveName: true, userSettings)
      {
         _lazyLoadTask = lazyLoadTask;
         _dialogCreator = dialogCreator;
         PresentationKey = PresenterConstants.PresenterKeys.IndividualSimulationComparisonPresenter;
         ChartEditorPresenter.SetLinkSimDataMenuItemVisibility(true);
      }

      public void DragDrop(object sender, IDragEvent e)
      {
         OnDragDrop(sender, e);
      }

      public bool AnyCurves()
      {
         return _repositoryCache != null && _repositoryCache.Any();
      }

      protected override void OnDragDrop(object sender, IDragEvent e)
      {
         var droppedNodes = e.Data<IReadOnlyList<ITreeNode>>();
         var messages = new List<string>();
         if (!containsIndividualSimulationNodes(droppedNodes))
         {
            base.OnDragDrop(sender, e);
            return;
         }

         individualSimulationsFrom(droppedNodes).Each(s =>
         {
            addSimulationToChart(s);
            if (!s.HasResults)
               messages.Add(PKSimConstants.Error.SimulationHasNoResultsAndCannotBeUsedInComparison(s.Name));
         });

         if (messages.Any())
            _dialogCreator.MessageBoxInfo(messages.ToString("\n"));
      }

      public void DragOver(object sender, IDragEvent e)
      {
         OnDragOver(sender, e);
      }

      protected override void OnDragOver(object sender, IDragEvent e)
      {
         var draggedNodes = e.Data<IReadOnlyList<ITreeNode>>();
         if (containsIndividualSimulationNodes(draggedNodes))
            e.Effect = DragEffect.Move;
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
            return;

         Chart.AddSimulation(simulation);
         ChartEditorPresenter.AddOutputMappings(simulation.OutputMappings);
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
         Column(BrowserColumns.Container).GroupIndex = -1;
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
         var isEmptyChart = !Chart.Curves.Any();
         Chart.AllSimulations.Each(s =>
         {
            UpdateAnalysisBasedOn(s, s.DataRepository);
            //if there are no curves in the chart, we are probably just creating it programatically
            if (isEmptyChart)
               _chartTemplatingTask.UpdateDefaultSettings(ChartEditorPresenter, s.DataRepository.ToList(), new[] {s}, addCurveIfNoSourceDefined: false);
         });
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
         ChartEditorPresenter.RemoveDataRepositories(new[] {repo});
         ChartEditorPresenter.RemoveOutputMappings(simulation.OutputMappings);
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