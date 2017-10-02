using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using PopulationAnalysis = PKSim.Core.Snapshots.PopulationAnalysis;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationAnalysisMapper : ContextSpecificationAsync<PopulationAnalysisMapper>
   {
      protected PopulationAnalysisFieldMapper _fieldMapper;
      protected PopulationStatisticalAnalysis _populationStatisticalAnalysis;
      protected PopulationAnalysis _snapshot;
      protected PredefinedStatisticalAggregation _selectedStatisticalAnalysis;
      protected PopulationBoxWhiskerAnalysis _boxWiskerAnalysis;
      protected PopulationPivotAnalysis _populationPivotAnalysis;
      private IPopulationAnalysisField _field1;
      private IPopulationAnalysisField _field2;
      protected PopulationAnalysisField _snapshotField1;
      protected PopulationAnalysisField _snapshotField2;

      protected override Task Context()
      {
         _fieldMapper = A.Fake<PopulationAnalysisFieldMapper>();
         sut = new PopulationAnalysisMapper(_fieldMapper);

         _populationStatisticalAnalysis = new PopulationStatisticalAnalysis();
         _selectedStatisticalAnalysis = new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Max, Selected = true, LineStyle = LineStyles.Dash};
         _populationStatisticalAnalysis.AddStatistic(_selectedStatisticalAnalysis);
         _populationStatisticalAnalysis.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Min, Selected = false});

         _boxWiskerAnalysis = new PopulationBoxWhiskerAnalysis {ShowOutliers = true};

         _field1 = A.Fake<IPopulationAnalysisField>();
         _field2 = A.Fake<IPopulationAnalysisField>();
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _populationPivotAnalysis.Add(_field1);
         _populationPivotAnalysis.Add(_field2);

         _snapshotField1 = new PopulationAnalysisField();
         _snapshotField2 = new PopulationAnalysisField();

         A.CallTo(() => _fieldMapper.MapToSnapshots(_populationPivotAnalysis.AllFields, _populationPivotAnalysis)).ReturnsAsync(new[] {_snapshotField1, _snapshotField2});
         return _completed;
      }
   }

   public class When_mapping_a_population_pivot_analysis_to_snapshot : concern_for_PopulationAnalysisMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_populationPivotAnalysis);
      }

      [Observation]
      public void should_have_mapped_the_analaysis_fields()
      {
         _snapshot.Fields.ShouldOnlyContain(_snapshotField1, _snapshotField2);
      }
   }

   public class When_mapping_a_population_statistical_analysis_to_snapshot : concern_for_PopulationAnalysisMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_populationStatisticalAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_containing_only_the_selected_statistical_aggreation()
      {
         _snapshot.Statistics.Length.ShouldBeEqualTo(1);
         _snapshot.Statistics[0].Id.ShouldBeEqualTo(_selectedStatisticalAnalysis.Id);
         _snapshot.Statistics[0].LineStyle.ShouldBeEqualTo(_selectedStatisticalAnalysis.LineStyle);
      }

      [Observation]
      public void should_set_the_box_wisker_info_to_null()
      {
         _snapshot.ShowOutliers.ShouldBeNull();
      }
   }

   public class When_mapping_a_population_box_whisker_analysis_to_snapshot : concern_for_PopulationAnalysisMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_boxWiskerAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_flag_set()
      {
         _snapshot.ShowOutliers.ShouldBeEqualTo(_boxWiskerAnalysis.ShowOutliers);
      }

      [Observation]
      public void should_set_statistical_aggreation_selection_to_null()
      {
         _snapshot.Statistics.ShouldBeNull();
      }
   }
}