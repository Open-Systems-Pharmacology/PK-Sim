using OSPSuite.Utility;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.PopulationAnalyses;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper : IMapper<IPopulationAnalysisField, PopulationAnalysisFieldDTO>
   {
   }

   public class PopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper : IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper
   {
      private readonly IDimensionRepository _dimensionRepository;

      public PopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper(IDimensionRepository dimensionRepository)
      {
         _dimensionRepository = dimensionRepository;
      }

      public PopulationAnalysisFieldDTO MapFrom(IPopulationAnalysisField field)
      {
         return new PopulationAnalysisFieldDTO(field)
         {
            Dimension = _dimensionRepository.MergedDimensionFor(field)
         };
      }
   }
}