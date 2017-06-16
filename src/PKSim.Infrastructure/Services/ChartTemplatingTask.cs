using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Chart.Mappers;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure.Services
{
   public class ChartTemplatingTask : OSPSuite.Presentation.Services.Charts.ChartTemplatingTask, IChartTemplatingTask
   {
      private readonly IChartFromTemplateService _chartFromTemplateService;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IPKSimChartFactory _chartFactory;
      private readonly IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      private readonly ICurveChartToCurveChartTemplateMapper _chartTemplateMapper;
      private readonly IExecutionContext _executionContext;
      private readonly IChartTask _chartTask;

      public ChartTemplatingTask(IChartFromTemplateService chartFromTemplateService, IProjectRetriever projectRetriever, IChartTemplatePersistor chartTemplatePersistor, IChartUpdater chartUpdater, IDialogCreator dialogCreator,
         IPKSimChartFactory chartFactory, IQuantityPathToQuantityDisplayPathMapper quantityDisplayPathMapper, ICurveChartToCurveChartTemplateMapper chartTemplateMapper,
         IExecutionContext executionContext, IApplicationController applicationController, ICloneManager cloneManager, IChartTask chartTask)
         : base(applicationController, chartTemplatePersistor, cloneManager, chartTemplateMapper, chartFromTemplateService, chartUpdater, dialogCreator)
      {
         _chartFromTemplateService = chartFromTemplateService;
         _projectRetriever = projectRetriever;
         _chartFactory = chartFactory;
         _quantityDisplayPathMapper = quantityDisplayPathMapper;
         _chartTemplateMapper = chartTemplateMapper;
         _executionContext = executionContext;
         _chartTask = chartTask;
      }

      public void InitFromTemplate(CurveChart chart, IChartEditorAndDisplayPresenter chartEditorPresenter,
         IReadOnlyCollection<DataColumn> allAvailableColumns, IReadOnlyCollection<IndividualSimulation> simulations, Func<DataColumn, string> nameForColumn, CurveChartTemplate defaultChartTemplate = null)
      {
         if (defaultChartTemplate == null)
         {
            UpdateDefaultSettings(chartEditorPresenter.EditorPresenter, allAvailableColumns, simulations);
            return;
         }

         _chartFromTemplateService.InitializeChartFromTemplate(chart, allAvailableColumns, defaultChartTemplate, nameForColumn);

         //this can happen if template does not have any matching curve
         if (!chart.Curves.Any())
            UpdateDefaultSettings(chartEditorPresenter.EditorPresenter, allAvailableColumns, simulations);
      }

      public void UpdateDefaultSettings(IChartEditorPresenter chartEditorPresenter, IReadOnlyCollection<DataColumn> allAvailableColumns, IReadOnlyCollection<ISimulation> simulations, bool addCurveIfNoSourceDefined = true)
      {
         UpdateDefaultSettings(chartEditorPresenter, allAvailableColumns, simulations, addCurveIfNoSourceDefined, () =>
         {
            addObservedDataToChart(chartEditorPresenter, simulations.OfType<IndividualSimulation>());
         });
      }

      private void addObservedDataToChart(IChartEditorPresenter chartEditorPresenter, IEnumerable<IndividualSimulation> simulations)
      {
         simulations.Each(simulation => addObservedDataToChart(chartEditorPresenter, simulation));
      }

      private void addObservedDataToChart(IChartEditorPresenter chartEditorPresenter, IndividualSimulation simulation)
      {
         var chartWithObservedData = chartEditorPresenter.Chart as ChartWithObservedData;
         if (chartWithObservedData != null)
            _chartTask.UpdateObservedDataInChartFor(simulation, chartWithObservedData);

         allObservedDataIn(simulation)
            .Each(observedData =>
            {
               var allObservationColumns = observedData.ObservationColumns();
               allObservationColumns.Each(observationColumn =>
               {
                  var sourceCurve = CurvePlotting(simulation, observationColumn);
                  addRepositoryToChartEditorWithDefaultCurveOptions(chartEditorPresenter, observedData, observationColumn, sourceCurve);
               });
            });
      }

      private static void addRepositoryToChartEditorWithDefaultCurveOptions(IChartEditorPresenter chartEditorPresenter, DataRepository dataRepository, DataColumn observationColumn, Curve sourceCurve)
      {
         if (sourceCurve == null) return;
         chartEditorPresenter.AddDataRepository(dataRepository);
         AddCurveForColumnWithOptionsFromSourceCurve(chartEditorPresenter, observationColumn, sourceCurve);
      }

      public SimulationTimeProfileChart CloneChart(SimulationTimeProfileChart originalChart, IndividualSimulation simulation)
      {
         var clonedChart = _chartFactory.Create(originalChart.GetType()).WithName(originalChart.Name);
         clonedChart.UpdateFrom(originalChart);

         initializeFromTemplate(originalChart, clonedChart, simulation);

         return clonedChart.DowncastTo<SimulationTimeProfileChart>();
      }

      private void initializeFromTemplate(CurveChart originalChart, CurveChart clonedChart, IndividualSimulation simulation)
      {
         var allAvailableColumns = new List<DataColumn>();
         addSimulationResults(simulation, allAvailableColumns);
         addObservedDataColumns(simulation, allAvailableColumns);
         Func<DataColumn, string> curveNameDefinition = c => _quantityDisplayPathMapper.DisplayPathAsStringFor(simulation, c);
         _chartFromTemplateService.InitializeChartFromTemplate(clonedChart, allAvailableColumns, _chartTemplateMapper.MapFrom(originalChart), curveNameDefinition);
      }

      private void addSimulationResults(IndividualSimulation simulation, List<DataColumn> allAvailableColumns)
      {
         allAvailableColumns.AddRange(simulation.DataRepository);
      }

      private void addObservedDataColumns(IndividualSimulation simulation, List<DataColumn> allAvailableColumns)
      {
         allObservedDataIn(simulation).Each(data => allAvailableColumns.AddRange(data.AllButBaseGrid()));
      }

      private IEnumerable<DataRepository> allObservedDataIn(Simulation simulation)
      {
         return simulation.UsedObservedData
            .Select(x => _projectRetriever.CurrentProject.ObservedDataBy(x.Id))
            .Where(x => !x.IsNull());
      }

      public void LoadCurves(CurveChart chart, IndividualSimulation simulation)
      {
         initializeFromTemplate(chart, chart, simulation);
      }

      public void LoadCurves(IndividualSimulation simulation)
      {
         simulation?.Charts.Each(c => LoadCurves(c, simulation));
      }

   
      protected override ICommand ReplaceTemplatesCommand(IWithChartTemplates withChartTemplates, IEnumerable<CurveChartTemplate> curveChartTemplates)
      {
         return updateChartTemplates(withChartTemplates, x =>
         {
            withChartTemplates.ChartTemplates.ToList().Each(t => withChartTemplates.RemoveChartTemplate(t.Name));
            curveChartTemplates.Each(withChartTemplates.AddChartTemplate);
         });
      }

      public override ICommand AddChartTemplateCommand(CurveChartTemplate template, IWithChartTemplates withChartTemplates)
      {
         return updateChartTemplates(withChartTemplates, x => x.AddChartTemplate(template));
      }

      public override ICommand UpdateChartTemplateCommand(CurveChartTemplate template, IWithChartTemplates withChartTemplates, string templateName)
      {
         return updateChartTemplates(withChartTemplates, x =>
         {
            withChartTemplates.RemoveChartTemplate(templateName);
            template.Name = templateName;
            withChartTemplates.AddChartTemplate(template);
         });
      }

      private ICommand updateChartTemplates(IWithChartTemplates withChartTemplates, Action<IWithChartTemplates> action)
      {
         //no command called in PKSim for chart templates
         action(withChartTemplates);
         _executionContext.PublishEvent(new ChartTemplatesChangedEvent(withChartTemplates));
         return new PKSimEmptyCommand();
      }
   }
}