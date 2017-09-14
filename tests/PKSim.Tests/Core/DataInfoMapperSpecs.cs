using System;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using SnapshotDataInfo = PKSim.Core.Snapshots.DataInfo;


namespace PKSim.Core
{
   public abstract class concern_for_DataInfoMapper : ContextSpecificationAsync<DataInfoMapper>
   {
      protected ExtendedPropertiesMapper _extendedPropertiesMapper;

      protected override Task Context()
      {
         _extendedPropertiesMapper = A.Fake<ExtendedPropertiesMapper>();
         sut = new DataInfoMapper(_extendedPropertiesMapper);
         return Task.FromResult(true);
      }
   }

   public class When_mapping_data_info_to_snapshot : concern_for_DataInfoMapper
   {
      private DataInfo _dataInfo;
      private SnapshotDataInfo _snapshot;
      private DateTime _dateTime;

      protected override async Task Context()
      {
         await base.Context();
         _dateTime = DateTime.Parse("January 1, 2017");
         _dataInfo = new DataInfo(ColumnOrigins.Observation, AuxiliaryType.GeometricStdDev, "unitName", _dateTime, "source", "category", 2.3) {LLOQ = 0.4f};
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_dataInfo);
      }

      [Observation]
      public void the_extended_property_mapper_is_used_to_map_extended_properties()
      {
         A.CallTo(() => _extendedPropertiesMapper.MapToSnapshot(_dataInfo.ExtendedProperties)).MustHaveHappened();
      }

      [Observation]
      public void the_snapshot_properties_are_set_as_expected()
      {
         _snapshot.AuxiliaryType.ShouldBeEqualTo(_dataInfo.AuxiliaryType.ToString());
         _snapshot.Category.ShouldBeEqualTo(_dataInfo.Category);
         _snapshot.ComparisonThreshold.ShouldBeEqualTo(_dataInfo.ComparisonThreshold);
         _snapshot.Date.ShouldBeEqualTo(_dataInfo.Date);
         _snapshot.LLOQ.ShouldBeEqualTo(_dataInfo.LLOQ);
         _snapshot.MolWeight.ShouldBeEqualTo(_dataInfo.MolWeight);
         _snapshot.Origin.ShouldBeEqualTo(_dataInfo.Origin.ToString());
         _snapshot.Source.ShouldBeEqualTo(_dataInfo.Source);
      }
   }
}
