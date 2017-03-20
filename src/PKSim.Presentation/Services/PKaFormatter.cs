using PKSim.Assets;
using OSPSuite.Utility.Format;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Services
{
   public class PKaFormatter : NumericFormatter<double>
   {
      private readonly TypePKaDTO _typePkaDTO;

      public PKaFormatter(TypePKaDTO typePkaDTO) : base(NumericFormatterOptions.Instance)
      {
         _typePkaDTO = typePkaDTO;
      }

      public override string Format(double valueToFormat)
      {
         if (_typePkaDTO.CompoundType == CompoundType.Neutral)
            return PKSimConstants.UI.None;
         return base.Format(valueToFormat);
      }
   }
}