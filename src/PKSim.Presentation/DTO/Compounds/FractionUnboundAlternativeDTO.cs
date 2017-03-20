using PKSim.Core.Model;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class FractionUnboundAlternativeDTO : ParameterAlternativeDTO
   {
      public new ParameterAlternativeWithSpecies ParameterAlternative { get; private set; }

      public FractionUnboundAlternativeDTO(ParameterAlternativeWithSpecies parameterAlternative) : base(parameterAlternative)
      {
         ParameterAlternative = parameterAlternative;
      }

      public IParameterDTO FractionUnboundParameter { get; set; }

      public Species Species
      {
         get { return ParameterAlternative.Species; }
         set { ParameterAlternative.Species = value; }
      }

      public double FractionUnbound
      {
         get { return FractionUnboundParameter.Value; }
         set {/*nothing to do here*/}
      }
   }
}