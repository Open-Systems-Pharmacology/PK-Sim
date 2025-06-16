using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;


namespace PKSim.Core.Snapshots.Mappers;

public class DataInfoMapper : OSPSuite.Core.Snapshots.Mappers.DataInfoMapper<PKSimProject>
{
   public DataInfoMapper(ExtendedPropertyMapper extendedPropertyMapper, IDimensionRepository dimensionRepository) : base(extendedPropertyMapper, dimensionRepository.DimensionByName(Constants.Dimension.MOLECULAR_WEIGHT))
   {
      
   }


}