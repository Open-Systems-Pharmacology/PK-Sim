using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.Binders;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services.Charts;
using IChartTemplatingTask = PKSim.Presentation.Services.IChartTemplatingTask;
using ItemChangedEventArgs = OSPSuite.Core.Chart.ItemChangedEventArgs;

namespace PKSim.Presentation.Presenters.Charts
{
   public enum ChartDisplayMode
   {
      Chart,
      PKAnalysis
   }

   public abstract class ChartPresenter<TChart, TView, TPresenter> : OSPSuite.Presentation.Presenters.Charts.ChartPresenter<TChart, TView, TPresenter>, IPKAnalysisWithChartPresenter
      where TChart : class, IChartWithObservedData
      where TView : IChartView<TPresenter>
      where TPresenter : IPresenter
   {
      protected const string _chartDisplayModeSetting = "chartDisplayMode";

      protected readonly IIndividualPKAnalysisPresenter _pkAnalysisPresenter;
      protected readonly IChartTask _chartTask;
      protected readonly IObservedDataTask _observedDataTask;
      private readonly IUserSettings _userSettings;

      protected ChartDisplayMode _chartDisplayMode;
      protected readonly ICache<DataRepository, IndividualSimulation> _repositoryCache;
      private readonly ObservedDataDragDropBinder _observedDataDragDropBinder;
      private readonly string _visiblePropertyName;
      protected readonly IChartTemplatingTask _chartTemplatingTask;

      protected ChartPresenter(TView view, ChartPresenterContext chartPresenterContext, IChartTemplatingTask chartTemplatingTask, IIndividualPKAnalysisPresenter pkAnalysisPresenter, IChartTask chartTask, IObservedDataTask observedDataTask, IUserSettings userSettings)
         : base(view, chartPresenterContext)
      {
         _chartTask = chartTask;
         _observedDataTask = observedDataTask;
         _userSettings = userSettings;
         _view.SetChartView(chartPresenterContext.ChartEditorAndDisplayPresenter.BaseView);
         _pkAnalysisPresenter = pkAnalysisPresenter;
         _view.SetPKAnalysisView(_pkAnalysisPresenter.View);
         AddSubPresenters(_pkAnalysisPresenter);
         _chartTemplatingTask = chartTemplatingTask;
         _repositoryCache = new Cache<DataRepository, IndividualSimulation> {OnMissingKey = noDataForSimulation};

         ChartEditorPresenter.SetShowDataColumnInDataBrowserDefinition(IsColumnVisibleInDataBrowser);
         ChartDisplayPresenter.DragDrop += OnDragDrop;
         ChartDisplayPresenter.DragOver += OnDragOver;
         ChartDisplayPresenter.ExportToPDF = () => _chartTask.ExportToPDF(Chart);
         AddAllButtons();
         _chartDisplayMode = ChartDisplayMode.Chart;
         _observedDataDragDropBinder = new ObservedDataDragDropBinder();
         _visiblePropertyName = ReflectionHelper.PropertyFor<ICurve, bool>(x => x.Visible).Name;
      }

      protected override void InitEditorLayout()
      {
         if (!string.IsNullOrEmpty(_userSettings.ChartEditorLayout))
            loadEditorLayoutFromUserSettings();
         else
            setDefaultEditorLayout();
      }

      private void loadEditorLayoutFromUserSettings()
      {
         _chartPresenterContext.EditorLayoutTask.InitEditorLayout(_chartPresenterContext.ChartEditorAndDisplayPresenter, _userSettings.ChartEditorLayout, loadColumnSettings: true);
      }

      protected bool IsColumnVisibleInDataBrowser(DataColumn dataColumn)
      {
         return _chartTask.IsColumnVisibleInDataBrowser(dataColumn);
      }

      private IndividualSimulation noDataForSimulation(DataRepository dataRepository)
      {
         return null;
      }

      protected override ISimulation SimulationFor(DataColumn dataColumn)
      {
         return _repositoryCache[dataColumn.Repository];
      }

      protected DataRepository DataRepositoryFor(Simulation simulation)
      {
         return _repositoryCache.KeyValues.Where(keyValue => Equals(keyValue.Value, simulation))
            .Select(keyValue => keyValue.Key)
            .FirstOrDefault();
      }

      protected virtual void UpdateAnalysisBasedOn(IndividualSimulation simulation, DataRepository dataRepository)
      {
         BindChartToEditors();

         if (_repositoryCache.Contains(dataRepository))
         {
            ChartEditorPresenter.RefreshDataRepository(dataRepository);
            ChartDisplayPresenter.Refresh();

            //after refresh, some data might not be available anymore=>in that case init chart from template
            InitializeFromTemplateIfRequired();

            RefreshPKAnalysisIfVisible();
         }
         else
         {
            _repositoryCache[dataRepository] = simulation;
            ChartEditorPresenter.AddDataRepository(dataRepository);

            //has to be done here since colums are added dynamically!?!?
            ChartEditorPresenter.ApplyColumnSettings();
         }
      }

      protected void InitializeFromTemplate()
      {
         InitializeFromTemplate(_repositoryCache.Keys.SelectMany(x => x.Columns).ToList(), _repositoryCache);
      }

      protected virtual void InitializeFromTemplate(IReadOnlyCollection<DataColumn> allColumns, IReadOnlyCollection<IndividualSimulation> simulations)
      {
         _chartTemplatingTask.InitFromTemplate(Chart, _chartPresenterContext.ChartEditorAndDisplayPresenter, allColumns, simulations, NameForColumn, DefaultChartTemplate);
      }

      protected abstract void ConfigureEditor();

      public override void Clear()
      {
         base.Clear();
         _repositoryCache.Clear();
      }

      protected override void AddChartEventHandlers()
      {
         base.AddChartEventHandlers();
         if (Chart == null) return;
         Chart.Curves.CollectionChanged += curveSelectionChanged;
         Chart.Curves.ItemPropertyChanged += curvePropertyChanged;
      }

      protected override void RemoveChartEventHandlers()
      {
         base.RemoveChartEventHandlers();
         if (Chart == null) return;
         Chart.Curves.CollectionChanged -= curveSelectionChanged;
         Chart.Curves.ItemPropertyChanged -= curvePropertyChanged;
      }

      private void curvePropertyChanged(object sender, ItemChangedEventArgs args)
      {
         var curve = args.Item as ICurve;
         if (curve == null) return;

         if (args.PropertyName.IsOneOf(_visiblePropertyName, _nameProperty))
            RefreshPKAnalysisIfVisible();
      }

      public void SwitchPKAnalysisPlot()
      {
         if (_chartDisplayMode == ChartDisplayMode.Chart)
            ShowAnalysis();
         else
            ShowChart();

         _settings.SetSetting(_chartDisplayModeSetting, _chartDisplayMode);
      }

      public override void LoadSettingsForSubject(IWithId subject)
      {
         base.LoadSettingsForSubject(subject);
         _pkAnalysisPresenter.LoadSettingsForSubject(subject);

         if (_settings.IsEqual(_chartDisplayModeSetting, ChartDisplayMode.PKAnalysis))
            ShowAnalysis();
         else
            ShowChart();
      }

      protected virtual void ShowChart()
      {
         _chartDisplayMode = ChartDisplayMode.Chart;
         _view.ShowChartView();
      }

      protected virtual void ShowAnalysis()
      {
         _chartDisplayMode = ChartDisplayMode.PKAnalysis;
         CalculatePKAnalysis();
         _view.ShowPKAnalysisView();
      }

      protected void RefreshPKAnalysisIfVisible()
      {
         if (_chartDisplayMode != ChartDisplayMode.PKAnalysis)
            return;
         CalculatePKAnalysis();
      }

      protected void CalculatePKAnalysis()
      {
         _pkAnalysisPresenter.ShowPKAnalysis(_repositoryCache, Chart.Curves.Where(curve => curve.Visible));
      }

      private void curveSelectionChanged(object sender, NotifyCollectionChangedEventArgs e)
      {
         RefreshPKAnalysisIfVisible();
      }

      public override void AddObservedData(DataRepository observedData, bool asResultOfDragAndDrop)
      {
         AddDataRepositoryToEditor(observedData);

         //make curve visibles
         if (!asResultOfDragAndDrop) return;

         observedData.Where(c => c.DataInfo.Origin == ColumnOrigins.Observation)
            .Each(c => ChartEditorPresenter.AddCurveForColumn(c.Id));
      }

      protected virtual void OnDragOver(object sender, DragEventArgs e)
      {
         _observedDataDragDropBinder.PrepareDrag(e);
      }

      protected virtual void OnDragDrop(object sender, DragEventArgs e)
      {
         var droppedObservedData = _observedDataDragDropBinder.DroppedObservedDataFrom(e);
         droppedObservedData.Each(data => AddObservedData(data, asResultOfDragAndDrop: true));
      }

      protected override void NotifyProjectChanged()
      {
         _chartTask.ProjectChanged();
      }

      private void setDefaultEditorLayout()
      {
         ResetEditor();

         Column(AxisOptionsColumns.UnitName).Caption = PKSimConstants.UI.Unit;


         ConfigureEditor();
         ChartEditorPresenter.ApplyColumnSettings();
      }

      protected void InitializeFromTemplateIfRequired()
      {
         if (Chart != null && !Chart.Curves.Any())
            InitializeFromTemplate();
      }
   }
}