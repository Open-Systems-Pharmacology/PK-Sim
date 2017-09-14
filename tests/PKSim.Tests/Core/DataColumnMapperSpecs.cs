using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using SnapshotDataColumn = PKSim.Core.Snapshots.DataColumn;

namespace PKSim.Core
{
   public abstract class concern_for_DataColumnMapper : ContextSpecificationAsync<DataColumnMapper>
   {
      protected QuantityInfoMapper _quantityInfoMapper;
      protected DataInfoMapper _dataInfoMapper;

      protected override Task Context()
      {
         _quantityInfoMapper = A.Fake<QuantityInfoMapper>();
         _dataInfoMapper = A.Fake<DataInfoMapper>();
         sut = new DataColumnMapper(_dataInfoMapper, _quantityInfoMapper);
         return Task.FromResult(true);
      }
   }

   public class When_mapping_data_column_to_snapshot : concern_for_DataColumnMapper
   {
      private DataColumn _dataColumn;
      private SnapshotDataColumn _snapshot;
      private DataColumn _relatedColumn;

      protected override async Task Context()
      {
         await base.Context();
         var observedData = DomainHelperForSpecs.ObservedData();
         _dataColumn = observedData.ObservationColumns().First();
         _relatedColumn = new DataColumn("related", DomainHelperForSpecs.NoDimension(), observedData.BaseGrid)
         {
            Values = new[] { 0f, 0f, 0f },
            DataInfo = { Origin = ColumnOrigins.ObservationAuxiliary}
         };
         _dataColumn.AddRelatedColumn(_relatedColumn);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_dataColumn);
      }

      [Observation]
      public void the_quantity_info_mapper_is_used_to_map_quantity_info()
      {
         A.CallTo(() => _quantityInfoMapper.MapToSnapshot(_dataColumn.QuantityInfo)).MustHaveHappened();
      }

      [Observation]
      public void the_data_info_mapper_is_used_to_map_data_info()
      {
         A.CallTo(() => _dataInfoMapper.MapToSnapshot(_dataColumn.DataInfo)).MustHaveHappened();
      }

      [Observation]
      public void the_snapshot_has_properties_set_as_expected()
      {
         _snapshot.Name.ShouldBeEqualTo(_dataColumn.Name);
         _snapshot.Unit.ShouldBeEqualTo(_dataColumn.DisplayUnit.ToString());
         _snapshot.Values.ShouldOnlyContainInOrder(_dataColumn.Values);
         _snapshot.RelatedColumns.Length.ShouldBeEqualTo(1);
      }
   }
}
