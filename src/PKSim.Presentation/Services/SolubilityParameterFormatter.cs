using OSPSuite.Utility.Format;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Services
{
   public class SolubilityParameterFormatter : NumericFormatter<double>
   {
      private readonly SolubilityAlternativeDTO _solubilityAlternativeDTO;

      public SolubilityParameterFormatter(SolubilityAlternativeDTO solubilityAlternativeDTO) : base(NumericFormatterOptions.Instance)
      {
         _solubilityAlternativeDTO = solubilityAlternativeDTO;
      }

      public override string Format(double valueToFormat)
      {
         if (_solubilityAlternativeDTO.IsTable)
            return " ";

         return base.Format(valueToFormat);
      }
   }
}