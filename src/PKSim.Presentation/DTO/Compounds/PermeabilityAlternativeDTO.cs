using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class PermeabilityAlternativeDTO : ParameterAlternativeDTO
   {
      public PermeabilityAlternativeDTO(ParameterAlternative parameterAlternative, ValueOrigin valueOrigin) : base(parameterAlternative, valueOrigin)
      {
      }

      public IParameterDTO PermeabilityParameter { get; set; }

      public double Permeability
      {
         get => PermeabilityParameter.Value;
         set => PermeabilityParameter.Value = value;
      }
   }
}