using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation.DTO.Compounds;

public class OverwriteParameterValueDTO
{
   private readonly ParameterValue _parameterValue;

   public OverwriteParameterValueDTO(ParameterValue parameterValue)
   {
      _parameterValue = parameterValue;
   }

   public string Path => _parameterValue.Path.ToString();
   public double? Value => _parameterValue.Value;
   public string Unit => _parameterValue.DisplayUnit.Name;
   public string ValueOrigin => _parameterValue.ValueOrigin.Display;
}