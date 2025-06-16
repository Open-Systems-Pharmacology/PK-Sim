using PKSim.Core.Repositories;
using System.Threading.Tasks;
using ModelValueOrigin = OSPSuite.Core.Domain.ValueOrigin;
using SnapshotValueOrigin = OSPSuite.Core.Snapshots.ValueOrigin;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ValueOriginMapper : OSPSuite.Core.Snapshots.Mappers.ValueOriginMapper
   {
      private readonly IValueOriginRepository _valueOriginRepository;

      public ValueOriginMapper(IValueOriginRepository valueOriginRepository)
      {
         _valueOriginRepository = valueOriginRepository;
      }

      public override Task<SnapshotValueOrigin> MapToSnapshot(ModelValueOrigin valueOrigin)
      {
         if(valueOriginComesFromDb(valueOrigin))
            return Task.FromResult<SnapshotValueOrigin>(null);

         return base.MapToSnapshot(valueOrigin);
      }

      private bool valueOriginComesFromDb(ModelValueOrigin valueOrigin)
      {
         var dbValueOrigin = _valueOriginRepository.FindBy(valueOrigin.Id);
         return dbValueOrigin.Equals(valueOrigin);
      }
   }
}