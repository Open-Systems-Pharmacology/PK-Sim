using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Compounds
{
   public class ParameterAlternativeDTO : DxValidatableDTO<ParameterAlternative>, IWithValueOrigin
   {
      private ValueOrigin _valueOrigin;
      public ParameterAlternative ParameterAlternative { get; }

      public ParameterAlternativeDTO(ParameterAlternative parameterAlternative) : base(parameterAlternative)
      {
         ParameterAlternative = parameterAlternative;
         _valueOrigin = ParameterAlternative.AllParameters().FirstOrDefault()?.ValueOrigin ?? new ValueOrigin();
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

      public ValueOrigin ValueOrigin => _valueOrigin;
   }
}