using OSPSuite.Utility;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IGlobalPKAnalysisToGlobalPKAnalysisDTOMapper : IMapper<GlobalPKAnalysis, GlobalPKAnalysisDTO>
   {
   }

   public class GlobalPKAnalysisToGlobalPKAnalysisDTOMapper : IGlobalPKAnalysisToGlobalPKAnalysisDTOMapper
   {
      private readonly IGlobalPKAnalysisToDataTableMapper _dataTableMapper;

      public GlobalPKAnalysisToGlobalPKAnalysisDTOMapper(IGlobalPKAnalysisToDataTableMapper dataTableMapper)
      {
         _dataTableMapper = dataTableMapper;
      }

      public GlobalPKAnalysisDTO MapFrom(GlobalPKAnalysis globalPKAnalysis)
      {
         return new GlobalPKAnalysisDTO(_dataTableMapper.MapFrom(globalPKAnalysis, addMetaData: true));
      }
   }
}