using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Snapshots.Mappers;
using SnapshotQuantityInfo = PKSim.Core.Snapshots.QuantityInfo;

namespace PKSim.Core
{
   public abstract class concern_for_QuantityInfoMapper : ContextSpecificationAsync<QuantityInfoMapper>
   {
      protected QuantityInfo _quantityInfo;

      protected override Task Context()
      {
         sut = new QuantityInfoMapper();
         _quantityInfo = new QuantityInfo("name", new[] { "the", "path" }, QuantityType.Time) { OrderIndex = 9 };
         return Task.FromResult(true);
      }
   }

   public class When_mapping_snapshot_to_quantity_info : concern_for_QuantityInfoMapper
   {
      private SnapshotQuantityInfo _snapshot;
      private QuantityInfo _result;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_quantityInfo);
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void the_quantity_info_should_have_values_as_in_the_original()
      {
         _result.OrderIndex.ShouldBeEqualTo(_quantityInfo.OrderIndex);
         _result.Path.ShouldOnlyContainInOrder(_quantityInfo.Path);
         _result.Type.ShouldBeEqualTo(_quantityInfo.Type);
         _result.Name.ShouldBeEqualTo(_quantityInfo.Name);
      }
   }

   public class When_mapping_quantity_info_to_snapshot : concern_for_QuantityInfoMapper
   {
      private SnapshotQuantityInfo _snapshot;

      protected override async Task Context()
      {
         await base.Context();
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_quantityInfo);
      }

      [Observation]
      public void the_snapshot_should_have_properties_as_expected()
      {
         _snapshot.OrderIndex.ShouldBeEqualTo(_quantityInfo.OrderIndex);
         _snapshot.Path.ShouldBeEqualTo(_quantityInfo.PathAsString);
         _snapshot.Type.ShouldBeEqualTo(_quantityInfo.Type);
         _snapshot.Name.ShouldBeEqualTo(_quantityInfo.Name);
      }
   }
}
