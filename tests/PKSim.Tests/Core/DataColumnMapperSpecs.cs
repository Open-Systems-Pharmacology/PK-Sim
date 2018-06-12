using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using SnapshotDataColumn = PKSim.Core.Snapshots.DataColumn;

namespace PKSim.Core
{
   public abstract class concern_for_DataColumnMapper : ContextSpecificationAsync<DataColumnMapper>
   {
      protected QuantityInfoMapper _quantityInfoMapper;
      protected DataInfoMapper _dataInfoMapper;
      protected DataColumn _dataColumn;
      protected SnapshotDataColumn _snapshot;
      protected DataColumn _relatedColumn;
      protected Snapshots.QuantityInfo _quantityInfoSnapshot;
      protected Snapshots.DataInfo _dataInfoSnapshot;
      private IDimensionRepository _dimensionRepository;
      protected BaseGrid _baseGrid;

      protected override Task Context()
      {
         _quantityInfoMapper = A.Fake<QuantityInfoMapper>();
         _dataInfoMapper = A.Fake<DataInfoMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         sut = new DataColumnMapper(_dataInfoMapper, _quantityInfoMapper, _dimensionRepository);

         var observedData = DomainHelperForSpecs.ObservedData();
         _dataColumn = observedData.ObservationColumns().First();
         _baseGrid = observedData.BaseGrid;
         _relatedColumn = new DataColumn("related", DomainHelperForSpecs.NoDimension(), observedData.BaseGrid)
         {
            Values = new[] { 0f, 0f, 0f },
            DataInfo = { Origin = ColumnOrigins.ObservationAuxiliary }
         };
         _dataColumn.AddRelatedColumn(_relatedColumn);

         _quantityInfoSnapshot = new Snapshots.QuantityInfo();
         _dataInfoSnapshot = new Snapshots.DataInfo();
         A.CallTo(() => _quantityInfoMapper.MapToSnapshot(_dataColumn.QuantityInfo)).Returns(_quantityInfoSnapshot);
         A.CallTo(() => _dataInfoMapper.MapToSnapshot(_dataColumn.DataInfo)).Returns(_dataInfoSnapshot);
         A.CallTo(() => _dimensionRepository.DimensionByName(_dataColumn.Dimension.Name)).Returns(_dataColumn.Dimension);
         A.CallTo(() => _dimensionRepository.DimensionByName(_baseGrid.Dimension.Name)).Returns(_baseGrid.Dimension);

         return Task.FromResult(true);
      }
   }

   public class When_mapping_snapshot_to_base_grid : concern_for_DataColumnMapper
   {
      private DataColumn _result;
      private DataRepository _contextDataRepository;
      private DataInfo _dataInfo;
      private QuantityInfo _quantityInfo;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_baseGrid);
         _contextDataRepository = DomainHelperForSpecs.ObservedData();
         _dataInfo = new DataInfo(ColumnOrigins.BaseGrid);
         _quantityInfo = new QuantityInfo("quantityInfo", new[] { "path" }, QuantityType.Undefined);

         A.CallTo(() => _dataInfoMapper.MapToModel(_snapshot.DataInfo)).Returns(_dataInfo);
         A.CallTo(() => _quantityInfoMapper.MapToModel(_snapshot.QuantityInfo)).Returns(_quantityInfo);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, _contextDataRepository);
      }

      [Observation]
      public void the_column_should_be_base_grid()
      {
         _result.ShouldBeAnInstanceOf<BaseGrid>();
      }
   }

   public class When_mapping_snapshot_to_data_column : concern_for_DataColumnMapper
   {
      private DataColumn _result;
      private DataRepository _contextDataRepository;
      private DataInfo _dataInfo;
      private QuantityInfo _quantityInfo;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_dataColumn);
         _contextDataRepository = DomainHelperForSpecs.ObservedData();
         _dataInfo = new DataInfo(ColumnOrigins.Observation);
         _quantityInfo = new QuantityInfo("quantityInfo", new[] { "path" }, QuantityType.Undefined);

         A.CallTo(() => _dataInfoMapper.MapToModel(_snapshot.DataInfo)).Returns(_dataInfo);
         A.CallTo(() => _quantityInfoMapper.MapToModel(_snapshot.QuantityInfo)).Returns(_quantityInfo);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot, _contextDataRepository);
      }

      [Observation]
      public void the_colum_contains_the_quantity_info()
      {
         _result.QuantityInfo.ShouldBeEqualTo(_quantityInfo);
      }

      [Observation]
      public void the_column_contains_the_data_info()
      {
         _result.DataInfo.ShouldBeEqualTo(_dataInfo);
      }

      [Observation]
      public void the_column_should_be_data_column()
      {
         _result.ShouldBeAnInstanceOf<DataColumn>();
      }

      [Observation]
      public void the_properties_should_be_set_as_in_the_original_column()
      {
         _result.Name.ShouldBeEqualTo(_dataColumn.Name);
         _result.Dimension.ShouldBeEqualTo(_dataColumn.Dimension);
         _result.DisplayUnit.ShouldBeEqualTo(_dataColumn.DisplayUnit);
         _result.Values.ShouldOnlyContainInOrder(_dataColumn.Values);
         _result.RelatedColumns.Count.ShouldBeEqualTo(1);
      }
   }

   public class When_mapping_data_column_to_snapshot : concern_for_DataColumnMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_dataColumn);
      }

      [Observation]
      public void the_snapshot_includes_the_quantity_info_snapshot()
      {
         _snapshot.QuantityInfo.ShouldBeEqualTo(_quantityInfoSnapshot);
      }

      [Observation]
      public void the_snapshot_includes_the_data_info_snapshot()
      {
         _snapshot.DataInfo.ShouldBeEqualTo(_dataInfoSnapshot);
      }

      [Observation]
      public void the_snapshot_has_properties_set_as_expected()
      {
         _snapshot.Name.ShouldBeEqualTo(_dataColumn.Name);
         _snapshot.Values.ShouldOnlyContainInOrder(_dataColumn.Values);
         _snapshot.RelatedColumns.Length.ShouldBeEqualTo(1);
         _snapshot.Dimension.ShouldBeEqualTo(_dataColumn.Dimension.DisplayName);
      }
   }
}
