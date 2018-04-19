using OSPSuite.Utility;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Matlab.Mappers
{
   public interface IMatlabOriginDataToOriginDataMapper : IMapper<OriginData, Core.Model.OriginData>
   {
   }

   public class MatlabOriginDataToOriginDataMapper : IMatlabOriginDataToOriginDataMapper
   {
      private readonly OriginDataMapper _originDataMapper;

      public MatlabOriginDataToOriginDataMapper(OriginDataMapper originDataMapper)
      {
         _originDataMapper = originDataMapper;
      }

      public Core.Model.OriginData MapFrom(OriginData snapshotOriginData)
      {
         return _originDataMapper.MapToModel(snapshotOriginData).Result;
      }
   }
}