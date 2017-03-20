using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class ParameterAlternativeDTO : DxValidatableDTO<ParameterAlternative>
   {
      public ParameterAlternative ParameterAlternative { get; private set; }

      public ParameterAlternativeDTO(ParameterAlternative parameterAlternative) : base(parameterAlternative)
      {
         ParameterAlternative = parameterAlternative;
      }

      public string Name
      {
         get { return ParameterAlternative.Name; }
         set { ParameterAlternative.Name = value; }
      }

      public bool IsDefault
      {
         get { return ParameterAlternative.IsDefault; }
         set { ParameterAlternative.IsDefault = value; }
      }

      public string Description
      {
         get { return ParameterAlternative.Description; }
         set { ParameterAlternative.Description = value; }
      }
   }
}