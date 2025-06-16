using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using SnapshotDataColumn = OSPSuite.Core.Snapshots.DataColumn;

namespace PKSim.Core.Snapshots.Mappers;

public class SnapshotContextWithDataRepository : SnapshotContextWithDataRepository<PKSimProject>
{
   public SnapshotContextWithDataRepository(DataRepository dataRepository, SnapshotContext<PKSimProject> baseContext) : base(dataRepository, baseContext, ProjectVersions.Current)
   {
   }
}

public class DataColumnMapper : DataColumnMapper<PKSimProject>
{
   private readonly IDimensionRepository _dimensionRepository;

   public DataColumnMapper(DataInfoMapper dataInfoMapper, QuantityInfoMapper quantityInfoMapper, IDimensionRepository dimensionRepository) : base(dataInfoMapper, quantityInfoMapper)
   {
      _dimensionRepository = dimensionRepository;
   }

   protected override IDimension DimensionFrom(SnapshotDataColumn snapshot)
   {
      return _dimensionRepository.DimensionByName(snapshot.Dimension);
   }
}
   