using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using PopulationAnalysis = PKSim.Core.Snapshots.PopulationAnalysis;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisChartMapper : ContextSpecificationAsync<PopulationAnalysisChartMapper>
   {
      protected ChartMapper _chartMapper;
      protected PopulationAnalysisMapper _populationAnalysisMapper;
      protected BoxWhiskerAnalysisChart _bowWiskerChart;
      protected PopulationAnalysis _snapshotPopulationAnalysis;

      protected override Task Context()
      {
         _chartMapper = A.Fake<ChartMapper>();
         _populationAnalysisMapper = A.Fake<PopulationAnalysisMapper>();

         sut = new PopulationAnalysisChartMapper(_chartMapper, _populationAnalysisMapper);

         _bowWiskerChart = new BoxWhiskerAnalysisChart();
         _bowWiskerChart.PopulationAnalysis = new PopulationBoxWhiskerAnalysis();
         _bowWiskerChart.AddSecondaryAxis(new AxisSettings());
         _bowWiskerChart.AddSecondaryAxis(new AxisSettings());

         _snapshotPopulationAnalysis =new PopulationAnalysis();
         A.CallTo(() => _populationAnalysisMapper.MapToSnapshot(_bowWiskerChart.BasePopulationAnalysis)).ReturnsAsync(_snapshotPopulationAnalysis);
         return _completed;
      }
   }

   public class When_mapping_a_population_analysis_chart_to_snapshot : concern_for_PopulationAnalysisChartMapper
   {
      private Snapshots.PopulationAnalysisChart _snapshot;

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
}