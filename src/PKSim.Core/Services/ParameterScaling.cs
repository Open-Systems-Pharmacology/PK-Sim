using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class ParameterScaling
   {
      private ScalingMethod _scalingMethod;

      //default value of scaled parameter
      protected double _targetValueBeforeScaling;

      /// <summary>
      ///    Origin parameter for the scaling (e.g. Adult parameter)
      /// </summary>
      public virtual IParameter SourceParameter { get; private set; }

      /// <summary>
      ///    Target parameter for the scaling (e.g. child parameter)
      /// </summary>
      public virtual IParameter TargetParameter { get; private set; }

      public ParameterScaling(IParameter sourceParameter, IParameter targetParameter)
      {
         SourceParameter = sourceParameter;
         TargetParameter = targetParameter;
         _targetValueBeforeScaling = targetParameter.Value;
      }

      /// <summary>
      ///    Method after which the scaling should be performed
      /// </summary>
      public virtual ScalingMethod ScalingMethod
      {
         get { return _scalingMethod; }
         set
         {
            _scalingMethod = value;
            TargetParameter.Value = ScaledValue;
         }
      }

      /// <summary>
      ///    Returns the scaled value in base unit according to the scaling method
      /// </summary>
      public virtual double ScaledValue
      {
         get { return ScalingMethod.ScaledValueFor(this); }
      }

      /// <summary>
      ///    Returns the scaled value in Display Unit
      /// </summary>
      public virtual double TargetScaledValueInDisplayUnit
      {
         get { return TargetParameter.ValueInDisplayUnit; }
      }

      /// <summary>
      ///    Returns the source value in Display Unit
      /// </summary>
      public virtual double SourceDefaultValueInDisplayUnit
      {
         get { return SourceParameter.ConvertToDisplayUnit(SourceParameter.DefaultValue); }
      }

      /// <summary>
      ///    Returns the source value in Display Unit
      /// </summary>
      public virtual double SourceValueInDisplayUnit
      {
         get { return SourceParameter.ValueInDisplayUnit; }
      }

      /// <summary>
      ///    Returns the target default in Display Unit
      /// </summary>
      public virtual double TargetDefaultValueInDisplayUnit
      {
         get { return TargetParameter.ConvertToDisplayUnit(_targetValueBeforeScaling); }
      }

      /// <summary>
      ///    Returns the target default in core unot
      /// </summary>
      public virtual double TargetValue
      {
         get { return _targetValueBeforeScaling; }
      }

      /// <summary>
      ///    Scales the target parameter and returns the actual scale command that was used to perform the scaling
      /// </summary>
      public virtual ICommand Scale()
      {
         return ScalingMethod.Scale(this);
      }

      /// <summary>
      ///    Returns true if both source and target parameters are distributed
      /// </summary>
      public virtual bool IsDistributedScaling
      {
         get
         {
            if (SourceParameter == null || TargetParameter == null) return false;
            return SourceParameter.IsDistributed() && TargetParameter.IsDistributed();
         }
      }

      public void ResetTargetParameter()
      {
         TargetParameter.Value = _targetValueBeforeScaling;
      }
   }
}