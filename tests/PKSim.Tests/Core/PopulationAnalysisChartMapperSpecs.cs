using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using PopulationAnalysis = PKSim.Core.Snapshots.PopulationAnalysis;
using PopulationAnalysisChart = PKSim.Core.Snapshots.PopulationAnalysisChart;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisChartMapper : ContextSpecificationAsync<PopulationAnalysisChartMapper>
   {
      protected ChartMapper _chartMapper;
      protected PopulationAnalysisMapper _populationAnalysisMapper;
      protected BoxWhiskerAnalysisChart _bowWiskerChart;
      protected PopulationAnalysis _snapshotPopulationAnalysis;
      protected PopulationAnalysisChart _snapshot;
      protected IPopulationAnalysisChartFactory _populationAnalysisChartFactory;
      protected ObservedDataCollectionMappper _observedDataCollectionMapper;

      protected override Task Context()
      {
         _chartMapper = A.Fake<ChartMapper>();
         _populationAnalysisMapper = A.Fake<PopulationAnalysisMapper>();
         _populationAnalysisChartFactory = A.Fake<IPopulationAnalysisChartFactory>();
         _observedDataCollectionMapper= A.Fake<ObservedDataCollectionMappper>();

         sut = new PopulationAnalysisChartMapper(_chartMapper, _populationAnalysisMapper, _observedDataCollectionMapper, _populationAnalysisChartFactory);

         _bowWiskerChart = new BoxWhiskerAnalysisChart
         {
            PopulationAnalysis = new PopulationBoxWhiskerAnalysis()
         };
         _bowWiskerChart.AddSecondaryAxis(new AxisSettings());
         _bowWiskerChart.AddSecondaryAxis(new AxisSettings());

         _snapshotPopulationAnalysis = new PopulationAnalysis();
         A.CallTo(() => _populationAnalysisMapper.MapToSnapshot(_bowWiskerChart.BasePopulationAnalysis)).Returns(_snapshotPopulationAnalysis);
         return _completed;
      }
   }

   public class When_mapping_a_population_analysis_chart_to_snapshot : concern_for_PopulationAnalysisChartMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_bowWiskerChart);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_analysit_type()
      {
         _snapshot.Type.ShouldBeEqualTo(_bowWiskerChart.AnalysisType);
      }

      [Observation]
      public void should_map_the_basic_chart_properties()
      {
         A.CallTo(() => _chartMapper.MapToSnapshot(_bowWiskerChart, _snapshot)).MustHaveHappened();
      }

      [Observation]
      public void should_map_primary_axis_settings()
      {
         _snapshot.XAxisSettings.ShouldBeEqualTo(_bowWiskerChart.PrimaryXAxisSettings);
         _snapshot.YAxisSettings.ShouldBeEqualTo(_bowWiskerChart.PrimaryYAxisSettings);
      }

      [Observation]
      public void should_map_secondary_axis_settings()
      {
         _snapshot.SecondaryYAxisSettings.ShouldOnlyContainInOrder(_bowWiskerChart.SecondaryYAxisSettings);
      }

      [Observation]
      public void should_map_population_analysis()
      {
         _snapshot.Analysis.ShouldBeEqualTo(_snapshotPopulationAnalysis);
      }
   }

   public class When_mapping_a_population_analysis_snapshot_to_population_analysis_chart : concern_for_PopulationAnalysisChartMapper
   {
      private SimulationAnalysisContext _context;
      private Model.PopulationAnalyses.PopulationAnalysisChart _newPopulationAnalysis;

      protected override async Task Context()
      {
         await base.Context();
         _context = new SimulationAnalysisContext();
         _snapshot = await sut.MapToSnapshot(_bowWiskerChart);
         var boxWiskerAnalysisChart = new BoxWhiskerAnalysisChart {PopulationAnalysis = new PopulationBoxWhiskerAnalysis()};
         A.CallTo(() => _populationAnalysisChartFactory.Create(PopulationAnalysisType.BoxWhisker)).Returns(boxWiskerAnalysisChart);
      }

      protected override async Task Because()
      {
         _newPopulationAnalysis = await sut.MapToModel(_snapshot, _context);
      }

      [Observation]
      public void should_return_a_population_analysis_chart_having_the_expected_type()
      {
         _newPopulationAnalysis.ShouldBeAnInstanceOf<BoxWhiskerAnalysisChart>();
      }

      [Observation]
      public void should_have_updated_the_axis_settings()
      {
         _bowWiskerChart.PrimaryXAxisSettings.ShouldBeEqualTo(_snapshot.XAxisSettings);
         _bowWiskerChart.PrimaryYAxisSettings.ShouldBeEqualTo(_snapshot.YAxisSettings);
         _bowWiskerChart.SecondaryYAxisSettings.ShouldContain(_snapshot.SecondaryYAxisSettings);
      }

      [Observation]
      public void should_map_the_basic_chart_properties()
      {
         A.CallTo(() => _chartMapper.MapToModel(_snapshot, _newPopulationAnalysis)).MustHaveHappened();
      }

      [Observation]
      public void should_map_population_analysis()
      {
         A.CallTo(() => _populationAnalysisMapper.MapToModel(_snapshot.Analysis, _newPopulationAnalysis.BasePopulationAnalysis)).MustHaveHappened();
      }
   }
}