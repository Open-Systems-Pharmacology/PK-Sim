using System.Threading.Tasks;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class BaseClassifiableMapper<T> : SnapshotMapperBase<T, Classifiable> where T : IClassifiableWrapper, new()
   {
      public override Task<Classifiable> MapToSnapshot(T model)
      {
         return SnapshotFrom(model, snapshot =>
         {
            snapshot.ClassificationPath = model.Parent?.Path;
            snapshot.Name = model.Name;
         });
      }

      public override Task<T> MapToModel(Classifiable snapshot)
      {
         return Task.FromResult(new T());
      }

   }
}