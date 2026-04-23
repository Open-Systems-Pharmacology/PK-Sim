using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation.DTO.Compounds;

public class OverwriteParameterValueDTO
{
   public OverwriteParameterValueDTO(ParameterValue parameterValue)
   {
      ParameterValue = parameterValue;
   }

   public ParameterValue ParameterValue { get; }

   public string Path => ParameterValue.Path.ToString();
   public double? Value => ParameterValue.Value;
   public string Unit => ParameterValue.DisplayUnit.Name;
   public string ValueOrigin => ParameterValue.ValueOrigin.Display;
}