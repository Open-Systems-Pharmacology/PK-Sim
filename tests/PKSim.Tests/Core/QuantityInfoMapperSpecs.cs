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
      protected override Task Context()
      {
         sut = new QuantityInfoMapper();
         return Task.FromResult(true);
      }
   }

   public class When_mapping_quantity_info_to_snapshot : concern_for_QuantityInfoMapper
   {
      private QuantityInfo _quantityInfo;
      private SnapshotQuantityInfo _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _quantityInfo = new QuantityInfo("name", new[] { "the", "path" }, QuantityType.Time) { OrderIndex = 9 };
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
         _snapshot.Type.ShouldBeEqualTo(_quantityInfo.Type.ToString());
         _snapshot.Name.ShouldBeEqualTo(_quantityInfo.Name);
      }
   }
}
