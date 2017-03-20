using OSPSuite.Core.Domain.Data;

namespace PKSim.Presentation.DTO.ObservedData
{
   public class ObservedDataToCompoundMappingDTO
   {
      public PKSim.Core.Model.Compound Compound { get; set; }
      public DataRepository ObservedData { get; set; }
   }
}