using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;

namespace PKSim.Core.Snapshots.Mappers;

public class DataRepositoryMapper : DataRepositoryMapper<PKSimProject, SnapshotContext>
{
   public DataRepositoryMapper(ExtendedPropertyMapper extendedPropertyMapper, DataColumnMapper dataColumnMapper) : base(extendedPropertyMapper, dataColumnMapper)
   {
   }

   protected override SnapshotContextWithDataRepository<PKSimProject> ContextFor(SnapshotContext snapshotContext, ModelDataRepository dataRepository)
   {
      return new SnapshotContextWithDataRepository(dataRepository, snapshotContext);
   }
}