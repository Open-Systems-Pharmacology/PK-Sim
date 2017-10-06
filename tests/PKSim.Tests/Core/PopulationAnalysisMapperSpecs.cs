using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
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
      protected PopulationBoxWhiskerAnalysis _populationBoxWiskerAnalysis;
      protected PopulationPivotAnalysis _populationPivotAnalysis;
      protected IPopulationAnalysisField _field1;
      protected IPopulationAnalysisField _field2;
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

         _populationBoxWiskerAnalysis = new PopulationBoxWhiskerAnalysis {ShowOutliers = true};

         _field1 = A.Fake<IPopulationAnalysisField>().WithName("Field1");
         _field2 = A.Fake<IPopulationAnalysisField>().WithName("Field2");
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _populationPivotAnalysis.Add(_field1);
         _populationPivotAnalysis.Add(_field2);
         _populationPivotAnalysis.SetPosition(_field1, PivotArea.FilterArea, 1);
         _populationPivotAnalysis.SetPosition(_field2, PivotArea.RowArea, 2);
         _snapshotField1 = new PopulationAnalysisField().WithName(_field1.Name);
         _snapshotField2 = new PopulationAnalysisField().WithName(_field2.Name);

         A.CallTo(() => _fieldMapper.MapToSnapshot(_field1)).Returns(_snapshotField1);
         A.CallTo(() => _fieldMapper.MapToSnapshot(_field2)).Returns(_snapshotField2);
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

      [Observation]
      public void should_have_updated_the_fields_position()
      {
         _snapshotField1.Area.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field1).Area);
         _snapshotField1.Index.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field1).Index);

         _snapshotField2.Area.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field2).Area);
         _snapshotField2.Index.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field2).Index);
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
         _snapshot = await sut.MapToSnapshot(_populationBoxWiskerAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_flag_set()
      {
         _snapshot.ShowOutliers.ShouldBeEqualTo(_populationBoxWiskerAnalysis.ShowOutliers);
      }

      [Observation]
      public void should_set_statistical_aggreation_selection_to_null()
      {
         _snapshot.Statistics.ShouldBeNull();
      }
   }

   public class When_updating_a_pivot_population_analysis_from_snapshot : concern_for_PopulationAnalysisMapper
   {
      private PopulationPivotAnalysis _newPivotAnalysis;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_populationPivotAnalysis);
         _newPivotAnalysis = new PopulationPivotAnalysis();

         A.CallTo(() => _fieldMapper.MapToModel(_snapshotField1)).Returns(_field1);
         A.CallTo(() => _fieldMapper.MapToModel(_snapshotField2)).Returns(_field2);
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, _newPivotAnalysis);
      }

      [Observation]
      public void should_have_added_one_field_for_each_snapshot_fields()
      {
         _newPivotAnalysis.AllFields.ShouldOnlyContain(_field1, _field2);
      }

      [Observation]
      public void should_have_update_the_field_position_as_expected()
      {
         _newPivotAnalysis.GetPosition(_field1).Area.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field1).Area);
         _newPivotAnalysis.GetPosition(_field1).Index.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field1).Index);

         _newPivotAnalysis.GetPosition(_field2).Area.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field2).Area);
         _newPivotAnalysis.GetPosition(_field2).Index.ShouldBeEqualTo(_populationPivotAnalysis.GetPosition(_field2).Index);
      }
   }

   public class When_updating_a_population_statistical_analysis_from_snapshot : concern_for_PopulationAnalysisMapper
   {
      private PopulationStatisticalAnalysis _newStatisticalAnalysis;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_populationStatisticalAnalysis);
         _newStatisticalAnalysis = new PopulationStatisticalAnalysis();
         _selectedStatisticalAnalysis.Selected = false;
         _selectedStatisticalAnalysis.LineStyle = LineStyles.DashDot;
         _newStatisticalAnalysis.AddStatistic(_selectedStatisticalAnalysis);
         _newStatisticalAnalysis.AddStatistic(new PredefinedStatisticalAggregation {Method = StatisticalAggregationType.Median});
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, _newStatisticalAnalysis);
      }

      [Observation]
      public void should_return_a_snapshot_containing_only_the_selected_statistical_aggreation()
      {
         _newStatisticalAnalysis.SelectedStatistics.ShouldOnlyContain(_selectedStatisticalAnalysis);
         _selectedStatisticalAnalysis.LineStyle.ShouldBeEqualTo(_snapshot.Statistics[0].LineStyle);
      }
   }

   public class When_updating_a_population_box_whisker_analysis_from_snapshot : concern_for_PopulationAnalysisMapper
   {
      private PopulationBoxWhiskerAnalysis _newBoxWhiskerAnalysis;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_populationBoxWiskerAnalysis);
         _newBoxWhiskerAnalysis = new PopulationBoxWhiskerAnalysis();
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, _newBoxWhiskerAnalysis);
      }

      [Observation]
      public void should_have_update_the_show_outliers_flag()
      {
         _newBoxWhiskerAnalysis.ShowOutliers.ShouldBeEqualTo(_populationBoxWiskerAnalysis.ShowOutliers);
      }
   }
}