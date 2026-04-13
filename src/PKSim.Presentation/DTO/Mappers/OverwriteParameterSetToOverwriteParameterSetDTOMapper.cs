using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.DTO.Mappers;

public interface IOverwriteParameterSetToOverwriteParameterSetDTOMapper : IMapper<OverwriteParameterSet, OverwriteParameterSetDTO>
{
}

public class OverwriteParameterSetToOverwriteParameterSetDTOMapper : IOverwriteParameterSetToOverwriteParameterSetDTOMapper
{
   public OverwriteParameterSetDTO MapFrom(OverwriteParameterSet overwriteParameterSet)
   {
      return new OverwriteParameterSetDTO(overwriteParameterSet);
   }
}