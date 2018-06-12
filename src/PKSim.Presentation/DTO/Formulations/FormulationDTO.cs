using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.Formulations
{
   public class FormulationDTO
   {
      public FormulationTypeDTO Type { get; set; }
      public string Description { get; set; }
      public IReadOnlyList<IParameter> Parameters { get; }

      public FormulationDTO(IReadOnlyList<IParameter> parameters)
      {
         Parameters = parameters;
      }
   }
}