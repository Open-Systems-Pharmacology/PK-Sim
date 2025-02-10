using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.DiseaseStates
{
   public class DiseaseStateDTO
   {
      public DiseaseState Value { get; set; }

      /// <summary>
      /// For now, we assume only one disease state parameter. We can make the view more generic if this is ever required
      /// </summary>
      public IParameterDTO Parameter { get; set; }

   }
}
