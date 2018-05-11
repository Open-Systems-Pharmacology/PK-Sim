using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class LipophilictyAlternativeDTO : ParameterAlternativeDTO
   {
      public LipophilictyAlternativeDTO(ParameterAlternative parameterAlternative, ValueOrigin valueOrigin) : base(parameterAlternative, valueOrigin)
      {
      }

      public IParameterDTO LipophilictyParameter { get; set; }

      public double Lipophilicty
      {
         get => LipophilictyParameter.Value;
         set
         {
            /*nothing to do here*/
         }
      }
   }
}