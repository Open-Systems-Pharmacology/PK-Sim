using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Mappers;

public abstract class ObjectBaseSnapshotMapperBase<TModel, TSnapshot> : ObjectBaseSnapshotMapperBase<TModel, TSnapshot, SnapshotContext>
   where TModel : IWithName, IWithDescription
   where TSnapshot : IWithName, IWithDescription, new()
{
}