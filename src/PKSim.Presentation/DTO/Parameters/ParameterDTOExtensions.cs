using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.DTO.Parameters
{
   public static class ParameterDTOExtensions
   {
      public static bool IsNull(this IParameterDTO parameterDTO)
      {
         return parameterDTO == null || parameterDTO.IsAnImplementationOf<NullParameterDTO>();
      }
   }
}