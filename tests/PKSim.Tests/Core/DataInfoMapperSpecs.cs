using System;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using DataInfo = OSPSuite.Core.Domain.Data.DataInfo;
using ExtendedProperties = PKSim.Core.Snapshots.ExtendedProperties;
using SnapshotDataInfo = PKSim.Core.Snapshots.DataInfo;


namespace PKSim.Core
{
   public abstract class concern_for_DataInfoMapper : ContextSpecificationAsync<DataInfoMapper>
   {
      protected ExtendedPropertiesMapper _extendedPropertiesMapper;
      protected ExtendedProperties _extendedPropertiesSnapshot;
      protected DataInfo _dataInfo;
      protected DateTime _dateTime;

      protected override Task Context()
      {
         _extendedPropertiesMapper = A.Fake<ExtendedPropertiesMapper>();
         sut = new DataInfoMapper(_extendedPropertiesMapper);

         _extendedPropertiesSnapshot = new ExtendedProperties();
         _dateTime = DateTime.Parse("January 1, 2017");
         _dataInfo = new DataInfo(ColumnOrigins.Observation, AuxiliaryType.GeometricStdDev, "unitName", _dateTime, "source", "category", 2.3) { LLOQ = 0.4f };

         A.CallTo(() => _extendedPropertiesMapper.MapToSnapshot(_dataInfo.ExtendedProperties)).ReturnsAsync(_extendedPropertiesSnapshot);

         return Task.FromResult(true);
      }
   }

   public class When_mapping_snapshot_to_data_info : concern_for_DataInfoMapper
   {
      private SnapshotDataInfo _snapshot;
      private DataInfo _result;
      private OSPSuite.Core.Domain.ExtendedProperties _extendedProperties;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_dataInfo);
         _extendedProperties = new OSPSuite.Core.Domain.ExtendedProperties {new ExtendedProperty<string> {Name = "key"}};
         A.CallTo(() => _extendedPropertiesMapper.MapToModel(_snapshot.ExtendedProperties)).ReturnsAsync(_extendedProperties);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void the_extended_properties_should_match()
      {
         _result.ExtendedProperties.ShouldOnlyContain(_extendedProperties);
      }

      [Observation]
      public void the_data_info_should_have_properties_as_in_original()
      {
         _result.AuxiliaryType.ShouldBeEqualTo(_dataInfo.AuxiliaryType);
         _result.Category.ShouldBeEqualTo(_dataInfo.Category);
         _result.ComparisonThreshold.ShouldBeEqualTo(_dataInfo.ComparisonThreshold);
         _result.Date.ShouldBeEqualTo(_dataInfo.Date);
         _result.LLOQ.ShouldBeEqualTo(_dataInfo.LLOQ);
         _result.MolWeight.ShouldBeEqualTo(_dataInfo.MolWeight);
         _result.Origin.ShouldBeEqualTo(_dataInfo.Origin);
         _result.Source.ShouldBeEqualTo(_dataInfo.Source);
      }
   }

   public class When_mapping_data_info_to_snapshot : concern_for_DataInfoMapper
   {
      private SnapshotDataInfo _snapshot;

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_dataInfo);
      }

      [Observation]
      public void the_snapshot_includes_the_extended_properties_snapshot()
      {
         _snapshot.ExtendedProperties.ShouldBeEqualTo(_extendedPropertiesSnapshot);
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
