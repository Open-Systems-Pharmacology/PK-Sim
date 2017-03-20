using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IScalingMethodTask
   {
      /// <summary>
      ///    Returns all the scaling methods available for the given parameter scaling
      /// </summary>
      /// <param name="parameterScaling">The parameter scaling for which scaling methods need to be retrieved</param>
      IEnumerable<ScalingMethod> AllMethodsFor(ParameterScaling parameterScaling);

      /// <summary>
      ///    Returns the default scaling method defined for the parameter scaling
      /// </summary>
      /// <param name="parameterScaling">The parameter scaling for which the default scaling method needs to be retrieved</param>
      ScalingMethod DefaultMethodFor(ParameterScaling parameterScaling);
   }

   public class ScalingMethodTask : IScalingMethodTask
   {
      private readonly IRepository<IScalingMethodSpecification> _repository;

      public ScalingMethodTask(IRepository<IScalingMethodSpecification> repository)
      {
         _repository = repository;
      }

      public IEnumerable<ScalingMethod> AllMethodsFor(ParameterScaling parameterScaling)
      {
         return allScalingMethodFor(parameterScaling).Select(x => x.Method).ToList();
      }

      public ScalingMethod DefaultMethodFor(ParameterScaling parameterScaling)
      {
         foreach (var scalingMethodSpecification in allScalingMethodFor(parameterScaling))
         {
            if (scalingMethodSpecification.IsDefaultFor(parameterScaling))
               return scalingMethodSpecification.Method;
         }
         //default not found, return our default scaling method
         return new KeepScalingMethod();
      }

      private IEnumerable<IScalingMethodSpecification> allScalingMethodFor(ParameterScaling parameterScaling)
      {
         return _repository.Where(x => x.IsSatisfiedBy(parameterScaling));
      }
   }
}