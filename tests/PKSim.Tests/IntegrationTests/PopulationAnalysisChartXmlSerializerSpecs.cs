using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_box_whisker_analysis_chart : ContextForSerialization<BoxWhiskerAnalysisChart>
   {
      private BoxWhiskerAnalysisChart _populationAnalysisChart;
      private BoxWhiskerAnalysisChart _deserialized;

      protected override void Context()
      {
         base.Context();
         _populationAnalysisChart = new BoxWhiskerAnalysisChart
         {
            PopulationAnalysis = new PopulationBoxWhiskerAnalysis{ShowOutliers = true},
            Id = "TOTO",
            Name = "Results",
            Description = "This is the description"
         };
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_populationAnalysisChart);
      }

      [Observation]
      public void should_be_able_to_load_the_serialized_chart()
      {
         _deserialized.PopulationAnalysis.ShouldNotBeNull();
         _deserialized.PopulationAnalysis.ShowOutliers.ShouldBeTrue();
         _deserialized.ChartSettings.ShouldNotBeNull();
         _deserialized.Id.ShouldBeEqualTo(_populationAnalysisChart.Id);
         _deserialized.Name.ShouldBeEqualTo(_populationAnalysisChart.Name);
         _deserialized.Description.ShouldBeEqualTo(_populationAnalysisChart.Description);
      }
   }

   public class When_serializing_a_time_profile_chart : ContextForSerialization<TimeProfileAnalysisChart>
   {
      private TimeProfileAnalysisChart _populationAnalysisChart;
      private TimeProfileAnalysisChart _deserialized;
      private DataRepository _dataRepository;
      private IWithIdRepository _idRepository;
      private DataColumn _dataColumn;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _idRepository = IoC.Resolve<IWithIdRepository>();
         var baseGrid=new BaseGrid("base",Constants.Dimension.NO_DIMENSION);
         _dataColumn = new DataColumn("Col",Constants.Dimension.NO_DIMENSION,baseGrid);
         _dataColumn.DataInfo.Origin = ColumnOrigins.Observation;
         _dataRepository = new DataRepository();
         _dataRepository.Add(_dataColumn);
         _idRepository.Register(_dataRepository);
      }

      protected override void Context()
      {
         base.Context();
         _populationAnalysisChart = new TimeProfileAnalysisChart
         {
            PopulationAnalysis = new PopulationStatisticalAnalysis(),
            Id = "TOTO",
            Name = "Results",
            Description = "This is the description"
         };
         _populationAnalysisChart.AddObservedData(_dataRepository);
         _populationAnalysisChart.CurveOptionsFor(_dataColumn).Color = Color.Bisque;
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_populationAnalysisChart);
      }

      [Observation]
      public void should_be_able_to_load_the_serialized_chart()
      {
         _deserialized.PopulationAnalysis.ShouldNotBeNull();
         _deserialized.ChartSettings.ShouldNotBeNull();
         _deserialized.Id.ShouldBeEqualTo(_populationAnalysisChart.Id);
         _deserialized.Name.ShouldBeEqualTo(_populationAnalysisChart.Name);
         _deserialized.Description.ShouldBeEqualTo(_populationAnalysisChart.Description);
      }

      [Observation]
      public void should_have_deserialized_the_referenced_observed_data()
      {
         _deserialized.UsesObservedData(_dataRepository).ShouldBeTrue();
      }

      [Observation]
      public void should_have_deserialized_the_curve_settings()
      {
         _deserialized.CurveOptionsFor(_dataColumn).ShouldNotBeNull();
         _deserialized.CurveOptionsFor(_dataColumn).Color.ShouldBeEqualTo(Color.Bisque);
         _deserialized.ObservedDataCollection.ObservedDataCurveOptions().Count().ShouldBeEqualTo(1);
      }
   }
}