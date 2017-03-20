using System.Data;

namespace PKSim.Presentation.DTO.Simulations
{
   public class GlobalPKAnalysisDTO : PKAnalysisDTO
   {
      public GlobalPKAnalysisDTO(DataTable dataTable) : base(dataTable)
      {
      }
   }
}