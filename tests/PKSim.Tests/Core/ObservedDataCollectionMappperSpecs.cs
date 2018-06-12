using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ObservedDataCurveOptions = PKSim.Core.Model.PopulationAnalyses.ObservedDataCurveOptions;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataCollectionMappper : ContextSpecificationAsync<ObservedDataCollectionMappper>
   {
      private CurveOptionsMapper _curveOptionsMapper;
      protected ObservedDataCollection _snapshot;
      protected Model.PopulationAnalyses.ObservedDataCollection _observedDataCollection;
      protected DataRepository _observedDataRepository;
      protected ObservedDataCurveOptions _observedDataCurveOptions;
      protected CurveOptions _snapshotCurveOptions;
      protected DataColumn _firstObservedDataColumn;

      protected override Task Context()
      {
         _curveOptionsMapper = A.Fake<CurveOptionsMapper>();
         sut = new ObservedDataCollectionMappper(_curveOptionsMapper);

         _observedDataRepository = DomainHelperForSpecs.ObservedData("ID").WithName("ObsData");
         _observedDataCollection = new Model.PopulationAnalyses.ObservedDataCollection();
         _observedDataCollection.ApplyGroupingToObservedData = true;
         _observedDataCollection.AddObservedData(_observedDataRepository);
         _firstObservedDataColumn = _observedDataRepository.ObservationColumns().First();
         _observedDataCurveOptions = new ObservedDataCurveOptions
         {
            Caption = "Obs Data Caption",
            ColumnId = _firstObservedDataColumn.Id
         };

         _observedDataCollection.AddCurveOptions(_observedDataCurveOptions);
         _snapshotCurveOptions = new CurveOptions();
         A.CallTo(() => _curveOptionsMapper.MapToSnapshot(_observedDataCurveOptions.CurveOptions)).Returns(_snapshotCurveOptions);
         return _completed;
      }
   }

   public class When_mapping_an_empty_observed_data_collection_to_snapshot : concern_for_ObservedDataCollectionMappper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(new Model.PopulationAnalyses.ObservedDataCollection());
      }

      [Observation]
      public void should_return_null()
      {
         _snapshot.ShouldBeNull();
      }
   }

   public class When_mapping_observed_data_collection_to_snapshot : concern_for_ObservedDataCollectionMappper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_observedDataCollection);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_list_of_all_observed_data_used()
      {
         _snapshot.ObservedData.ShouldContain(_observedDataRepository.Name);
      }

      [Observation]
      public void should_have_saved_the_observed_data_collection_properties()
      {
         _snapshot.ApplyGrouping.ShouldBeEqualTo(_observedDataCollection.ApplyGroupingToObservedData);
      }

      [Observation]
      public void should_have_saved_the_observed_data_curve_options()
      {
         _snapshot.CurveOptions.Length.ShouldBeEqualTo(1);
         _snapshot.CurveOptions[0].CurveOptions.ShouldBeEqualTo(_snapshotCurveOptions);
         _snapshot.CurveOptions[0].Caption.ShouldBeEqualTo(_observedDataCurveOptions.Caption);
      }
   }

   public class When_mapping_null_observed_data_collection_snapshot_to_observed_data_collection : concern_for_ObservedDataCollectionMappper
   {
      private Model.PopulationAnalyses.ObservedDataCollection _result;

      protected override async Task Because()
      {
         _result =  await sut.MapToModel(null, new SimulationAnalysisContext());
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_mapping_observed_data_collection_snapshot_to_observed_data_collection : concern_for_ObservedDataCollectionMappper
   {
      private Model.PopulationAnalyses.ObservedDataCollection _newObservedDataCollection;
      private SimulationAnalysisContext _simulationAnalysisContext;

      protected override async Task Context()
      {
         await base.Context();
         _simulationAnalysisContext = new SimulationAnalysisContext(new []{_observedDataRepository, });
         _snapshot = await sut.MapToSnapshot(_observedDataCollection);
      }

      protected override async Task Because()
      {
         _newObservedDataCollection = await sut.MapToModel(_snapshot, _simulationAnalysisContext);
      }

      [Observation]
      public void should_return_an_observed_data_collection_with_the_expected_properties()
      {
         _newObservedDataCollection.ApplyGroupingToObservedData.ShouldBeEqualTo(_observedDataCollection.ApplyGroupingToObservedData);
         _newObservedDataCollection.AllObservedData().ShouldOnlyContain(_observedDataCollection.AllObservedData());
         _newObservedDataCollection.ObservedDataCurveOptions().Count().ShouldBeEqualTo(_observedDataCollection.ObservedDataCurveOptions().Count());
         _newObservedDataCollection.ObservedDataCurveOptionsFor(_firstObservedDataColumn).Caption.ShouldBeEqualTo(_observedDataCurveOptions.Caption);
      }
   }
}