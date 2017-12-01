using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Compression;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Chart.Mappers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Charts;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ChartTemplatingTask : ContextSpecification<ChartTemplatingTask>
   {
      protected IChartFromTemplateService _chartFromTemplateService;
      protected IProjectRetriever _projectRetriever;
      protected IChartTemplatePersistor _chartTemplatePersistor;
      protected IStringCompression _stringCompression;
      protected IDialogCreator _dialogCreator;
      protected IPKSimChartFactory _chartFactory;
      protected IQuantityPathToQuantityDisplayPathMapper _quantityDisplayPathMapper;
      protected ICurveChartToCurveChartTemplateMapper _chartTemplateMapper;
      protected IPKSimXmlSerializerRepository _chartEditorXmlSerializerRepository;
      protected IChartTask _chartTask;
      protected ICloneManager _cloneManager;
      protected IApplicationController _applicationController;
      protected IExecutionContext _executionContext;
      private IChartUpdater _chartUpdater;

      protected override void Context()
      {
         _chartFromTemplateService = A.Fake<IChartFromTemplateService>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _chartTemplatePersistor = A.Fake<IChartTemplatePersistor>();
         _stringCompression = A.Fake<IStringCompression>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _chartFactory = A.Fake<IPKSimChartFactory>();
         _quantityDisplayPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         _chartTemplateMapper = A.Fake<ICurveChartToCurveChartTemplateMapper>();
         _chartTask = A.Fake<IChartTask>();
         _cloneManager = A.Fake<ICloneManager>();
         _applicationController = A.Fake<IApplicationController>();
         _executionContext = A.Fake<IExecutionContext>();
         _chartUpdater= A.Fake<IChartUpdater>();
         sut = new ChartTemplatingTask(_chartFromTemplateService, _projectRetriever, _chartTemplatePersistor,_chartUpdater, _dialogCreator, _chartFactory, _quantityDisplayPathMapper, _chartTemplateMapper,
            _executionContext, _applicationController, _cloneManager, _chartTask);
      }
   }

   public class When_updating_default_settings_of_chart_with_observed_data : concern_for_ChartTemplatingTask
   {
      private IChartEditorPresenter _chartEditorPresenter;
      private IReadOnlyCollection<DataColumn> _allAvailableColumns;
      private IReadOnlyCollection<ISimulation> _simulationCollection;
      private ChartWithObservedData _chartWithObservedData;

      protected override void Context()
      {
         base.Context();
         _allAvailableColumns = A.Fake<IReadOnlyCollection<DataColumn>>();
         _chartEditorPresenter = A.Fake<IChartEditorPresenter>();
         var individualSimulation = new IndividualSimulation { SimulationSettings = new SimulationSettings() };
         _simulationCollection = new List<ISimulation> { individualSimulation };
         _chartWithObservedData = A.Fake<ChartWithObservedData>();
         A.CallTo(() => _chartEditorPresenter.Chart).Returns(_chartWithObservedData);
      }

      protected override void Because()
      {
         sut.UpdateDefaultSettings(_chartEditorPresenter, _allAvailableColumns, _simulationCollection);
      }

      [Observation]
      public void the_chart_task_should_update_observed_data()
      {
         A.CallTo(() => _chartTask.UpdateObservedDataInChartFor(A<Simulation>._, _chartWithObservedData)).MustHaveHappened();
      }
   }


   public abstract class When_updating_default_settings : concern_for_ChartTemplatingTask
   {
      protected IReadOnlyCollection<IndividualSimulation> _simulations;
      protected IReadOnlyCollection<DataColumn> _allAvailableColumns;
      protected IChartEditorPresenter _chartEditorPresenter;
      protected Curve _curve;
      protected DataColumn _column;

      protected abstract DataColumn GenerateDataColumn();

      protected override void Context()
      {
         base.Context();
         _curve = new Curve();
         _curve.SetxData(A.Fake<DataColumn>(), A.Fake<IDimensionFactory>());

         _chartEditorPresenter = A.Fake<IChartEditorPresenter>();
         _simulations = A.Fake<IReadOnlyCollection<IndividualSimulation>>();
         var individualSimulation = new IndividualSimulation {SimulationSettings = new SimulationSettings()};
         _simulations = new List<IndividualSimulation> { individualSimulation };
         var simulationConcentrationChart = new SimulationTimeProfileChart();
         individualSimulation.AddAnalysis(simulationConcentrationChart);

         var dataRepository = generateDataRepository();

         individualSimulation.AddUsedObservedData(dataRepository);
         simulationConcentrationChart.AddObservedData(dataRepository);
         simulationConcentrationChart.AddCurve(_curve);

         A.CallTo(() => _projectRetriever.CurrentProject.ObservedDataBy(dataRepository.Id)).Returns(dataRepository);
         _chartEditorPresenter.Edit(new CurveChart());
      }

      private DataRepository generateDataRepository()
      {
         _column = GenerateDataColumn();
         _allAvailableColumns = new List<DataColumn> { _column };
         _curve.SetyData(_column, A.Fake<IDimensionFactory>());
         var dataRepository = new DataRepository { _column };
         return dataRepository;
      }

      protected static BaseGrid GenerateNewBaseGrid()
      {
         return new BaseGrid("baseGridId", new Dimension(new BaseDimensionRepresentation(), "time", "min"));
      }

      protected override void Because()
      {
         sut.UpdateDefaultSettings(_chartEditorPresenter, _allAvailableColumns, _simulations);
      }
   }

   public class When_updating_the_default_settings_for_a_calcualted_output_and_no_template_is_found : When_updating_default_settings
   {
      protected override void Context()
      {
         base.Context();
         var outputSelections = _simulations.First().SimulationSettings.OutputSelections;
         outputSelections.AddOutput(new QuantitySelection("path", QuantityType.Undefined));
      }

      protected override DataColumn GenerateDataColumn()
      {
         return new DataColumn
         {
            BaseGrid = GenerateNewBaseGrid(),
            DataInfo = { Origin = ColumnOrigins.Calculation },
            QuantityInfo = { Path = new[] { "path" } }
         };
      }

      [Observation]
      public void should_add_the_new_curve_to_the_chart_with_the_settings_from_the_simulation_as_defaults()
      {
         A.CallTo(() => _chartEditorPresenter.AddCurveForColumn(_column, _curve.CurveOptions)).MustHaveHappened();
      }
   }

   public class When_updating_the_default_settings_for_a_observed_data_and_no_template_is_found : When_updating_default_settings
   {

      protected override DataColumn GenerateDataColumn()
      {
         return new DataColumn
         {
            BaseGrid = GenerateNewBaseGrid(),
            DataInfo = { Origin = ColumnOrigins.Observation }
         };
      }

      [Observation]
      public void should_add_the_new_curve_to_the_chart_with_the_settings_from_the_simulation_as_defaults()
      {
         A.CallTo(() => _chartEditorPresenter.AddCurveForColumn(_column, _curve.CurveOptions)).MustHaveHappened();
      }
   }
}
