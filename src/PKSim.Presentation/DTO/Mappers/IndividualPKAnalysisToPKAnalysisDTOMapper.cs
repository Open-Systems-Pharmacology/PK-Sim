using System.Collections.Generic;
using System.Data;
using PKSim.Core.Mappers;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Chart;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualPKAnalysisToPKAnalysisDTOMapper
   {
      PKAnalysisDTO MapFrom(IReadOnlyList<IndividualPKAnalysis> analyses, IEnumerable<Curve> curves);
   }

   public class IndividualPKAnalysisToPKAnalysisDTOMapper : IIndividualPKAnalysisToPKAnalysisDTOMapper
   {
      private readonly IIndividualPKAnalysisToDataTableMapper _dataTableMapper;

      public IndividualPKAnalysisToPKAnalysisDTOMapper(IIndividualPKAnalysisToDataTableMapper dataTableMapper)
      {
         _dataTableMapper = dataTableMapper;
      }

      public PKAnalysisDTO MapFrom(IReadOnlyList<IndividualPKAnalysis> analyses, IEnumerable<Curve> curves)
      {
         return new PKAnalysisDTO(_dataTableMapper.MapFrom(analyses, curves, addMetaData: true));
      }
   }
}