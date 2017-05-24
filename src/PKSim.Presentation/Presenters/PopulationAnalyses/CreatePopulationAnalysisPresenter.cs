﻿using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   internal enum RefreshMode
   {
      RefreshDataAndPlot,
      RefreshPlot
   }

   public interface ICreatePopulationAnalysisPresenter : IWizardPresenter,
      IListener<FieldAddedToPopulationAnalysisEvent>,
      IListener<FieldRemovedFromPopulationAnalysisEvent>,
      IListener<FieldRenamedInPopulationAnalysisEvent>,
      IListener<FieldUnitChangedInPopulationAnalysisEvent>,
      IListener<PopulationAnalysisChartSettingsChangedEvent>
   {
      void SaveAnalysis();
      void LoadAnalysis();

      /// <summary>
      ///    Edits the given population Analysis chart. Returns true if the edit was confirmed otherwise false
      /// </summary>
      bool Edit(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart populationAnalysisChart);

      PopulationAnalysisChart Create(IPopulationDataCollector populationDataCollector);
   }

   public abstract class CreatePopulationAnalysisPresenter<TPopulationAnalysis, TPopulationAnalysisChart> : PKSimWizardPresenter<ICreatePopulationAnalysisView, ICreatePopulationAnalysisPresenter, IPopulationAnalysisItemPresenter>,
      ICreatePopulationAnalysisPresenter
      where TPopulationAnalysis : PopulationAnalysis, new()
      where TPopulationAnalysisChart : PopulationAnalysisChart<TPopulationAnalysis>, new()
   {
      private readonly IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      private readonly IPopulationAnalysisTask _populationAnalysisTask;
      private bool _shouldRefreshData;
      protected bool ShouldRefreshChart { get; set; }

      private IPopulationDataCollector _populationDataCollector;

      public PopulationAnalysisChart<TPopulationAnalysis> PopulationAnalysisChart { get; private set; }
      protected abstract string AnalysisType { get; }

      protected CreatePopulationAnalysisPresenter(ICreatePopulationAnalysisView view, ISubPresenterItemManager<IPopulationAnalysisItemPresenter> subPresenterItemManager,
         IReadOnlyList<ISubPresenterItem> subPresenterItems, IDialogCreator dialogCreator, IPopulationAnalysisTemplateTask populationAnalysisTemplateTask,
         IPopulationAnalysisChartFactory populationAnalysisChartFactory, IPopulationAnalysisTask populationAnalysisTask)
         : base(view, subPresenterItemManager, subPresenterItems, dialogCreator)
      {
         _populationAnalysisTemplateTask = populationAnalysisTemplateTask;
         _populationAnalysisTask = populationAnalysisTask;
         PopulationAnalysisChart = populationAnalysisChartFactory.Create<TPopulationAnalysis, TPopulationAnalysisChart>();
      }

      public PopulationAnalysisChart Create(IPopulationDataCollector populationDataCollector)
      {
         View.Caption = PKSimConstants.UI.CreateAnalysis(AnalysisType);
         Edit(populationDataCollector, PopulationAnalysisChart);
         updateOriginText(populationDataCollector);
         return PopulationAnalysisChart;
      }

      private void updateOriginText(IPopulationDataCollector populationDataCollector)
      {
         if (PopulationAnalysisChart == null) return;
         _populationAnalysisTask.SetOriginText(PopulationAnalysisChart, populationDataCollector.Name);
      }

      protected bool ShouldRefreshData
      {
         get => _shouldRefreshData;
         set
         {
            _shouldRefreshData = value;
            ShouldRefreshChart = value;
         }
      }

      protected TPopulationAnalysis PopulationAnalysis => PopulationAnalysisChart.PopulationAnalysis;

      public bool Edit(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart populationAnalysisChart)
      {
         View.Caption = PKSimConstants.UI.EditAnalysis(AnalysisType);
         return Edit(populationDataCollector, populationAnalysisChart.DowncastTo<PopulationAnalysisChart<TPopulationAnalysis>>());
      }

      protected virtual bool Edit(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart<TPopulationAnalysis> populationAnalysisChart)
      {
         _populationDataCollector = populationDataCollector;
         PopulationAnalysisChart = populationAnalysisChart;
         PopulationAnalysisChart.Analysable = _populationDataCollector;

         SetWizardButtonEnabled(_subPresenterItems.First());

         InitializeSubPresentersForAnalysis();

         View.Display();
         if (_view.Canceled)
         {
            PopulationAnalysisChart = null;
            return false;
         }
         return true;
      }

      protected override void UpdateControls(int currentIndex)
      {
         View.OkEnabled = CanClose;
         View.NextEnabled = true;

         //only refresh if selected item is pivot data
         if (currentIndex != ResultsPresenterItem.Index)
            return;

         if (ShouldRefreshData)
         {
            ResultsPresenter.RefreshAnalysis();
            ShouldRefreshData = false;
         }

         if (ShouldRefreshChart)
         {
            ResultsPresenter.RefreshChart();
            ShouldRefreshChart = false;
         }
      }

      protected abstract ISubPresenterItem<IPopulationAnalysisResultsPresenter> ResultsPresenterItem { get; }

      protected IPopulationAnalysisResultsPresenter ResultsPresenter => PresenterAt(ResultsPresenterItem);

      protected virtual void InitializeSubPresentersForAnalysis()
      {
         ShouldRefreshData = true;
         _subPresenterItemManager.AllSubPresenters.Each(p => p.StartAnalysis(_populationDataCollector, PopulationAnalysis));
         ResultsPresenter.Chart = PopulationAnalysisChart;
      }

      public void SaveAnalysis()
      {
         _populationAnalysisTemplateTask.SavePopulationAnalysis(PopulationAnalysis);
      }

      public void LoadAnalysis()
      {
         var toLoad = _populationAnalysisTemplateTask.LoadPopulationAnalysisFor<TPopulationAnalysis>(_populationDataCollector);
         if (toLoad == null) return;
         PopulationAnalysisChart.PopulationAnalysis = toLoad;
         InitializeSubPresentersForAnalysis();
      }

      private void handle(PopulationAnalysisEvent eventToHandle, RefreshMode refreshMode)
      {
         if (!canHandle(eventToHandle))
            return;

         if (refreshMode == RefreshMode.RefreshDataAndPlot)
            ShouldRefreshData = true;
         else
            ShouldRefreshChart = true;
      }

      private bool canHandle(PopulationAnalysisEvent eventToHandle)
      {
         return Equals(eventToHandle.PopulationAnalysis, PopulationAnalysis);
      }

      public void Handle(FieldRemovedFromPopulationAnalysisEvent eventToHandle)
      {
         handle(eventToHandle, RefreshMode.RefreshDataAndPlot);
      }

      public void Handle(FieldAddedToPopulationAnalysisEvent eventToHandle)
      {
         handle(eventToHandle, RefreshMode.RefreshDataAndPlot);
      }

      public void Handle(FieldRenamedInPopulationAnalysisEvent eventToHandle)
      {
         handle(eventToHandle, RefreshMode.RefreshDataAndPlot);
      }

      public void Handle(FieldUnitChangedInPopulationAnalysisEvent eventToHandle)
      {
         handle(eventToHandle, RefreshMode.RefreshPlot);
      }

      public void Handle(PopulationAnalysisChartSettingsChangedEvent eventToHandle)
      {
         handle(eventToHandle, RefreshMode.RefreshPlot);
      }

      public void Handle(PopulationAnalysisDataSelectionChangedEvent eventToHandle)
      {
         handle(eventToHandle, RefreshMode.RefreshPlot);
         //trigger refresh of chart itself when selected
         UpdateControls();
      }
   }
}