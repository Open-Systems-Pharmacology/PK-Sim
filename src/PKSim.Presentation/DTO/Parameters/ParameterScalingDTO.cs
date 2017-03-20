using OSPSuite.Utility.Reflection;
using PKSim.Core.Services;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Parameters
{
   public class ParameterScalingDTO : Notifier
   {
      public ParameterScaling ParameterScaling { get; private set; }
      public IParameterDTO SourceParameter { get; set; }
      public IParameterDTO TargetParameter { get; set; }
      public string ParameterFullPathDisplay { get; set; }

      public ParameterScalingDTO(ParameterScaling parameterScaling)
      {
         ParameterScaling = parameterScaling;
      }

      public ScalingMethod ScalingMethod
      {
         get { return ParameterScaling.ScalingMethod; }
         set
         {
            ParameterScaling.ScalingMethod = value;
            OnPropertyChanged(() => ScalingMethod);
            OnPropertyChanged(() => TargetScaledValue);
         }
      }

      public double SourceDefaultValue
      {
         get { return ParameterScaling.SourceDefaultValueInDisplayUnit; }
      }

      public double SourceValue
      {
         get { return ParameterScaling.SourceValueInDisplayUnit; }
      }

      public double TargetDefaultValue
      {
         get { return ParameterScaling.TargetDefaultValueInDisplayUnit; }
      }

      public double TargetScaledValue
      {
         get { return ParameterScaling.TargetScaledValueInDisplayUnit; }
      }
   }
}