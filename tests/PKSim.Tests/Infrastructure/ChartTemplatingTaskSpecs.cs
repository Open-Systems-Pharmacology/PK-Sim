using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Compression;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
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
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Presentation.Services;

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
         sut = new ChartTemplatingTask(
            _chartFromTemplateService, 
            _projectRetriever, 
            _chartTemplatePersistor, 
            _stringCompression, 
            _dialogCreator, 
            _chartFactory, 
            _quantityDisplayPathMapper, 
            _chartTemplateMapper,
            A.Fake<IExecutionContext>(),
            A.Fake<IApplicationController>(),
            A.Fake<ICloneManager>());
      }
   }

   public abstract class When_updating_default_settings_base_class : concern_for_ChartTemplatingTask
   {
      protected IReadOnlyCollection<IndividualSimulation> _simulations;
      protected IReadOnlyCollection<DataColumn> _allAvailableColumns;
      protected IChartEditorPresenter _chartEditorPresenter;
      protected ICurve _curve;
      protected DataColumn _column;

      protected abstract DataColumn GenerateDataColumn();

      protected override void Context()
      {
         base.Context();
         _curve = new Curve();
         _curve.SetxData(A.Fake<DataColumn>(), A.Fake<IDimensionFactory>());

         _chartEditorPresenter = A.Fake<IChartEditorPresenter>();
         _simulations = A.Fake<IReadOnlyCollection<IndividualSimulation>>();
         var individualSimulation = new IndividualSimulation();
         individualSimulation.SimulationSettings = new SimulationSettings();
         _simulations = new List<IndividualSimulation> {individualSimulation};
         var simulationConcentrationChart = new SimulationTimeProfileChart();
         individualSimulation.AddAnalysis(simulationConcentrationChart);

         var dataRepository = generateDataRepository();

         individualSimulation.AddUsedObservedData(dataRepository);
         simulationConcentrationChart.AddObservedData(dataRepository);
         simulationConcentrationChart.AddCurve(_curve);

         A.CallTo(() => _projectRetriever.CurrentProject.ObservedDataBy(dataRepository.Id)).Returns(dataRepository);
         _chartEditorPresenter.DataSource = new CurveChart();
      }

      private DataRepository generateDataRepository()
      {
         _column = GenerateDataColumn();
         _allAvailableColumns = new List<DataColumn> {_column};
         _curve.SetyData(_column, A.Fake<IDimensionFactory>());
         var dataRepository = new DataRepository {_column};
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

   public class When_updating_the_default_settings_for_a_calcualted_output_and_no_template_is_found : When_updating_default_settings_base_class
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
            DataInfo = {Origin = ColumnOrigins.Calculation},
            QuantityInfo = {Path = new[] {"path"}}
         };
      }

      [Observation]
      public void should_add_the_new_curve_to_the_chart_with_the_settings_from_the_simulation_as_defaults()
      {
         A.CallTo(() => _chartEditorPresenter.AddCurveForColumn(_column.Id, _curve.CurveOptions)).MustHaveHappened();
      }
   }

   public class When_updating_the_default_settings_for_a_observed_data_and_no_template_is_found : When_updating_default_settings_base_class
   {

      protected override DataColumn GenerateDataColumn()
      {
         return new DataColumn
         {
            BaseGrid = GenerateNewBaseGrid(),
            DataInfo = {Origin = ColumnOrigins.Observation}
         };
      }

      [Observation]
      public void should_add_the_new_curve_to_the_chart_with_the_settings_from_the_simulation_as_defaults()
      {
         A.CallTo(() => _chartEditorPresenter.AddCurveForColumn(_column.Id, _curve.CurveOptions)).MustHaveHappened();
      }
   }
}
