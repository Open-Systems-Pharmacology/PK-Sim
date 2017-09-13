using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using SnapshotDataRepository = PKSim.Core.Snapshots.DataRepository;

namespace PKSim.Core
{
   public abstract class concern_for_ObservedDataMapper : ContextSpecification<ObservedDataMapper>
   {
      protected DataColumnMapper _dataColumnMapper;
      protected ExtendedPropertiesMapper _extendedPropertiesMapper;

      protected override void Context()
      {
         _dataColumnMapper = A.Fake<DataColumnMapper>();
         _extendedPropertiesMapper = A.Fake<ExtendedPropertiesMapper>();
         sut = new ObservedDataMapper(_extendedPropertiesMapper, _dataColumnMapper);
      }
   }

   public class When_mapping_observed_data_to_snapshot : concern_for_ObservedDataMapper
   {
      private DataRepository _dataRepository;
      private SnapshotDataRepository _snapshot;
      private DataColumn _dataColumn;
      private DataColumn _relatedColumn;

      protected override void Context()
      {
         base.Context();
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

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_dataRepository);
      }

      [Observation]
      public void the_snapshot_properties_should_be_set_as_expected()
      {
         _snapshot.Name.ShouldBeEqualTo(_dataRepository.Name);
         _snapshot.Description.ShouldBeEqualTo(_dataRepository.Description);
      }
   }
}
