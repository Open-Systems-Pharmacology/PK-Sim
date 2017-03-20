using System.Collections.Generic;
using OSPSuite.Utility;
using PKSim.Core.Mappers;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IPopulationPKAnalysisToPKAnalysisDTOMapper : IMapper<IReadOnlyList<PopulationPKAnalysis>, PKAnalysisDTO>
   {
   }

   public class PopulationPKAnalysisToPKAnalysisDTOMapper : IPopulationPKAnalysisToPKAnalysisDTOMapper
   {
      private readonly IPopulationPKAnalysisToDataTableMapper _dataTableMapper;

      public PopulationPKAnalysisToPKAnalysisDTOMapper(IPopulationPKAnalysisToDataTableMapper dataTableMapper)
      {
         _dataTableMapper = dataTableMapper;
      }

      public PKAnalysisDTO MapFrom(IReadOnlyList<PopulationPKAnalysis> analyses)
      {
         return new PKAnalysisDTO(_dataTableMapper.MapFrom(analyses, addMetaData: true));
      }
   }
}