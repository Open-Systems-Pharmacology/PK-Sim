using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using SnapshotDataRepository = PKSim.Core.Snapshots.DataRepository;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataMapper : ContextSpecificationAsync<DataRepositoryMapper>
   {
      protected DataColumnMapper _dataColumnMapper;
      protected ExtendedPropertyMapper _extendedPropertyMapper;
      protected Snapshots.DataColumn _dataColumnSnapshot;
      protected DataRepository _dataRepository;
      
      protected DataColumn _dataColumn;
      protected DataColumn _relatedColumn;
      protected Snapshots.ExtendedProperty _extendedPropertySnapshot;
      protected Snapshots.DataColumn _baseGridSnapshot;
      protected IExtendedProperty _extendedProperty;

      protected override Task Context()
      {
         _dataColumnMapper = A.Fake<DataColumnMapper>();
         _extendedPropertyMapper = A.Fake<ExtendedPropertyMapper>();
         sut = new DataRepositoryMapper(_extendedPropertyMapper, _dataColumnMapper);

         _dataRepository = DomainHelperForSpecs.ObservedData();
         _dataColumn = _dataRepository.ObservationColumns().First();
         _relatedColumn = new DataColumn("related", DomainHelperForSpecs.NoDimension(), _dataRepository.BaseGrid)
         {
            Values = new[] { 0f, 0f, 0f },
            DataInfo = { Origin = ColumnOrigins.ObservationAuxiliary }
         };
         _dataColumn.AddRelatedColumn(_relatedColumn);
         _dataRepository.Add(_relatedColumn);
         _dataRepository.Description = "description";
         _dataRepository.Name = "thename";
         _extendedProperty = new ExtendedProperty<string>{Name = "Name"};
         _dataRepository.ExtendedProperties.Add(_extendedProperty);
         _dataColumnSnapshot = new Snapshots.DataColumn();
         _extendedPropertySnapshot = new Snapshots.ExtendedProperty();
         _baseGridSnapshot = new Snapshots.DataColumn();
         A.CallTo(() => _dataColumnMapper.MapToSnapshot(_dataColumn)).Returns(_dataColumnSnapshot);
         A.CallTo(() => _extendedPropertyMapper.MapToSnapshot(_extendedProperty)).Returns(_extendedPropertySnapshot);
         A.CallTo(() => _extendedPropertyMapper.MapToModel(_extendedPropertySnapshot, A<SnapshotContext>._)).Returns(_extendedProperty);
         A.CallTo(() => _dataColumnMapper.MapToSnapshot(_dataRepository.BaseGrid)).Returns(_baseGridSnapshot);
         
         return Task.FromResult(true);
      }
   }

   public class When_mapping_snapshot_to_observed_data : concern_for_ObservedDataMapper
   {
      private SnapshotDataRepository _snapshot;
      private DataRepository _result;
      private BaseGrid _baseGrid;
      private DataColumn _mappedDataColumn;
      private DataColumn _mappedRelatedColumn;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_dataRepository);

         _baseGrid = new BaseGrid("baseGrid", DomainHelperForSpecs.TimeDimensionForSpecs());
         _mappedDataColumn = new DataColumn("DataColumn", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid);
         _mappedRelatedColumn = new DataColumn("RelatedColumn", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid);
         _mappedDataColumn.AddRelatedColumn(_mappedRelatedColumn);

         A.CallTo(() => _dataColumnMapper.MapToModel(_snapshot.BaseGrid, A<SnapshotContextWithDataRepository>._)).Returns(_baseGrid);
         A.CallTo(() => _dataColumnMapper.MapToModel(_snapshot.Columns.First(), A<SnapshotContextWithDataRepository>._)).Returns(_mappedDataColumn);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, new SnapshotContext());
      }

      [Observation]
      public void the_result_should_have_properties_set_as_in_original()
      {
         _result.Name.ShouldBeEqualTo(_dataRepository.Name);
         _result.Description.ShouldBeEqualTo(_dataRepository.Description);
         _result.BaseGrid.ShouldBeEqualTo(_baseGrid);
         _result.Columns.ShouldOnlyContain(_mappedDataColumn, _baseGrid, _mappedRelatedColumn);
         _result.ExtendedProperties.ShouldContain(_extendedProperty);
      }
   }

   public class When_mapping_observed_data_to_snapshot : concern_for_ObservedDataMapper
   {
      protected SnapshotDataRepository _snapshot;
      protected override async Task Context()
      {
         await base.Context();
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_dataRepository);
      }

      [Observation]
      public void the_snapshot_properties_should_be_set_as_expected()
      {
         _snapshot.Name.ShouldBeEqualTo(_dataRepository.Name);
         _snapshot.Description.ShouldBeEqualTo(_dataRepository.Description);
      }

      [Observation]
      public void should_not_include_related_or_basegrid_columns()
      {
         _snapshot.BaseGrid.ShouldBeEqualTo(_baseGridSnapshot);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_underlying_models()
      {
         //should not contain the basegrid or related column
         _snapshot.Columns.ShouldOnlyContain(_dataColumnSnapshot);
         _snapshot.ExtendedProperties.ShouldContain(_extendedPropertySnapshot);
      }
   }
}
