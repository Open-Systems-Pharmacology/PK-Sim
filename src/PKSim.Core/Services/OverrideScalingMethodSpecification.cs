using System.Linq;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class OverrideScalingMethodSpecification : IScalingMethodSpecification
   {
      private readonly IParameterTask _parameterTask;

      public OverrideScalingMethodSpecification(IParameterTask parameterTask)
      {
         _parameterTask = parameterTask;
      }

      public bool IsSatisfiedBy(ParameterScaling parameterScaling)
      {
         //Satisfied by all parameter scaling
         return true;
      }

      public bool IsDefaultFor(ParameterScaling parameterScaling)
      {
         return parameterScaling.SourceParameter.IsIndividualMoleculeGlobal();
      }

      public ScalingMethod Method => new OverrideScalingMethod(_parameterTask);
   }
}