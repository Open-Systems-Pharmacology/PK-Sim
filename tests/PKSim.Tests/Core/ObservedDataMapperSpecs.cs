using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using SnapshotDataRepository = PKSim.Core.Snapshots.DataRepository;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataMapper : ContextSpecificationAsync<ObservedDataMapper>
   {
      protected DataColumnMapper _dataColumnMapper;
      protected ExtendedPropertiesMapper _extendedPropertiesMapper;

      protected override Task Context()
      {
         _dataColumnMapper = A.Fake<DataColumnMapper>();
         _extendedPropertiesMapper = A.Fake<ExtendedPropertiesMapper>();
         sut = new ObservedDataMapper(_extendedPropertiesMapper, _dataColumnMapper);
         return Task.FromResult(true);
      }
   }

   public class When_mapping_observed_data_to_snapshot : concern_for_ObservedDataMapper
   {
      private DataRepository _dataRepository;
      private SnapshotDataRepository _snapshot;
      private DataColumn _dataColumn;
      private DataColumn _relatedColumn;

      protected override async Task Context()
      {
         await base.Context();
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
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_dataRepository);
      }

      [Observation]
      public void the_snapshot_properties_should_be_set_as_expected()
      {
         _snapshot.Name.ShouldBeNull();
         _snapshot.Description.ShouldBeEqualTo(_dataRepository.Description);
      }
   }
}
