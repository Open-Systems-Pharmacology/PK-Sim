using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IAdvancedParameterCollection : IEntity
   {
      IAdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter);
      IEnumerable<IAdvancedParameter> AdvancedParameters { get; }
      void AddAdvancedParameter(IAdvancedParameter advancedParameter);
      void RemoveAdvancedParameter(IAdvancedParameter advancedParameter);
   }

   public class AdvancedParameterCollection : Container, IAdvancedParameterCollection
   {
      public AdvancedParameterCollection()
      {
         Name = CoreConstants.ContainerName.AdvancedParameterCollection;
      }

      public IEnumerable<IAdvancedParameter> AdvancedParameters
      {
         get { return GetChildren<IAdvancedParameter>(); }
      }

      public void AddAdvancedParameter(IAdvancedParameter advancedParameter)
      {
         if (advancedParameterForPath(advancedParameter.ParameterPath) != null)
            return;

         Add(advancedParameter);
      }

      private IAdvancedParameter advancedParameterForPath(string parameterPath)
      {
         return AdvancedParameters.FirstOrDefault(adv => string.Equals(adv.ParameterPath, parameterPath));
      }

      public IAdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter)
      {
         var parameterPath = entityPathResolver.PathFor(parameter);
         return AdvancedParameters.FirstOrDefault(adv => string.Equals(adv.ParameterPath, parameterPath));
      }

      public void RemoveAdvancedParameter(IAdvancedParameter advancedParameter)
      {
         var advancedParameterToRemove = advancedParameterForPath(advancedParameter.ParameterPath);
         if (advancedParameterToRemove == null) return;
         RemoveChild(advancedParameterToRemove);
      }
   }
}