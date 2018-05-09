using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class ParameterAlternativeDTO : DxValidatableDTO<ParameterAlternative>, IWithValueOrigin
   {
      public ParameterAlternative ParameterAlternative { get; }
      public ValueOrigin ValueOrigin { get; }

      public ParameterAlternativeDTO(ParameterAlternative parameterAlternative) : base(parameterAlternative)
      {
         ParameterAlternative = parameterAlternative;
         ValueOrigin = ParameterAlternative.AllParameters(x => !x.IsDefault).FirstOrDefault()?.ValueOrigin ?? new ValueOrigin();
      }

      public string Name
      {
         get => ParameterAlternative.Name;
         set => ParameterAlternative.Name = value;
      }

      public bool IsDefault
      {
         get => ParameterAlternative.IsDefault;
         set => ParameterAlternative.IsDefault = value;
      }

      public void UpdateValueOriginFrom(ValueOrigin sourceValueOrigin)
      {
         ValueOrigin.UpdateFrom(sourceValueOrigin);
      }
   }
}