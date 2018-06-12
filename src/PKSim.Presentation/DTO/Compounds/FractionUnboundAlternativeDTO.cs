using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class FractionUnboundAlternativeDTO : ParameterAlternativeDTO
   {
      public new ParameterAlternativeWithSpecies ParameterAlternative { get; }

      public FractionUnboundAlternativeDTO(ParameterAlternativeWithSpecies parameterAlternative, ValueOrigin valueOrigin) : base(parameterAlternative, valueOrigin)
      {
         ParameterAlternative = parameterAlternative;
      }

      public IParameterDTO FractionUnboundParameter { get; set; }

      public Species Species
      {
         get => ParameterAlternative.Species;
         set => ParameterAlternative.Species = value;
      }

      public double FractionUnbound
      {
         get => FractionUnboundParameter.Value;
         set
         {
            /*nothing to do here*/
         }
      }
   }
}