using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services.Charts;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;
using IChartTemplatingTask = PKSim.Presentation.Services.IChartTemplatingTask;

namespace PKSim.Presentation.Presenters.Charts
{
   public interface ISimulationTimeProfileChartPresenter : IChartPresenter<SimulationTimeProfileChart>,
      ISimulationAnalysisPresenter,
      IPKAnalysisWithChartPresenter,
      IListener<RenamedEvent>,
      IListener<ObservedDataAddedToAnalysableEvent>,
      IListener<ObservedDataRemovedFromAnalysableEvent>,
      IListener<SimulationResultsUpdatedEvent>
   {
   }

   public class SimulationTimeProfileChartPresenter : ChartPresenter<SimulationTimeProfileChart, ISimulationTimeProfileChartView, ISimulationTimeProfileChartPresenter>,
      ISimulationTimeProfileChartPresenter,
      ISimulationAnalysisPresenter<IndividualSimulation>

   {
      public SimulationTimeProfileChartPresenter(ISimulationTimeProfileChartView view, ChartPresenterContext chartPresenterContext, IIndividualPKAnalysisPresenter pkAnalysisPresenter, IChartTask chartTask, IObservedDataTask observedDataTask, IChartTemplatingTask chartTemplatingTask, IChartUpdater chartUpdateTask) :
         base(view, chartPresenterContext, chartTemplatingTask, pkAnalysisPresenter, chartTask, observedDataTask, chartUpdateTask)
      {
         PresentationKey = PresenterConstants.PresenterKeys.SimulationTimeProfileChartPresenter;
      }

      protected override void AddObservedData(IReadOnlyList<DataRepository> observedData, bool asResultOfDragAndDrop)
      {
         base.AddObservedData(observedData, asResultOfDragAndDrop);
         if (asResultOfDragAndDrop)
            _observedDataTask.AddObservedDataToAnalysable(observedData, Simulation);
      }

      protected override void NotifyProjectChanged()
      {
         base.NotifyProjectChanged();
         Simulation.HasChanged = true;
      }

      public void UpdateAnalysisBasedOn(IndividualSimulation individualSimulation)
      {
         base.UpdateAnalysisBasedOn(individualSimulation, individualSimulation.DataRepository);
      }

      public void InitializeAnalysis(ISimulationAnalysis simulationAnalysis, IAnalysable analysable)
      {
         base.InitializeAnalysis(simulationAnalysis.DowncastTo<SimulationTimeProfileChart>());
         UpdateAnalysisBasedOn(analysable);
      }

      public void UpdateAnalysisBasedOn(IAnalysable analysable)
      {
         var simulation = analysable.DowncastTo<IndividualSimulation>();
         UpdateAnalysisBasedOn(simulation);
         UpdateTemplatesBasedOn(simulation);

         InitializeFromTemplateIfRequired();
      }

      protected Simulation Simulation => _repositoryCache.First();

      protected DataRepository DataRepository => DataRepositoryFor(Simulation);

      public void Handle(ObservedDataAddedToAnalysableEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         AddObservedData(eventToHandle.ObservedData, eventToHandle.ShowData);
      }

      public void Handle(ObservedDataRemovedFromAnalysableEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         RemoveDataRepositoriesFromEditor(eventToHandle.ObservedData);
         ChartDisplayPresenter.Refresh();
      }

      private bool canHandle(AnalysableEvent analysableEvent)
      {
         return Equals(Simulation, analysableEvent.Analysable);
      }

      public ISimulationAnalysis Analysis => Chart;

      public void Handle(SimulationResultsUpdatedEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         _chartTask.SetOriginTextFor(Simulation.Name, Chart);
      }

      protected override void ConfigureColumns()
      {
         base.ConfigureColumns();
         Column(BrowserColumns.RepositoryName).GroupIndex = 0;
         Column(BrowserColumns.RepositoryName).Visible = true;
         Column(BrowserColumns.RepositoryName).VisibleIndex = 0;

         Column(BrowserColumns.Container).Visible = true;
         Column(BrowserColumns.Container).Caption = PKSimConstants.UI.Organ;
         Column(BrowserColumns.Container).GroupIndex = 1;
         Column(BrowserColumns.Container).VisibleIndex = 1;

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
         if (!Equals(eventToHandle.RenamedObject, Chart))
            return;

         ChartChanged();
      }
   }
}