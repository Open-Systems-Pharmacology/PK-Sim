using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class AdvancedParameterCollection : Container
   {
      public AdvancedParameterCollection()
      {
         Name = CoreConstants.ContainerName.AdvancedParameterCollection;
      }

      public IEnumerable<AdvancedParameter> AdvancedParameters => GetChildren<AdvancedParameter>();

      public void AddAdvancedParameter(AdvancedParameter advancedParameter)
      {
         if (advancedParameterForPath(advancedParameter.ParameterPath) != null)
            return;

         Add(advancedParameter);
      }

      private AdvancedParameter advancedParameterForPath(string parameterPath)
      {
         return AdvancedParameters.FirstOrDefault(adv => string.Equals(adv.ParameterPath, parameterPath));
      }

      public AdvancedParameter AdvancedParameterFor(IEntityPathResolver entityPathResolver, IParameter parameter)
      {
         return advancedParameterForPath(entityPathResolver.PathFor(parameter));
      }

      public void RemoveAdvancedParameter(AdvancedParameter advancedParameter)
      {
         var advancedParameterToRemove = advancedParameterForPath(advancedParameter.ParameterPath);
         if (advancedParameterToRemove == null) return;
         RemoveChild(advancedParameterToRemove);
      }

      public void Clear()
      {
         var allAdvancedParametrs = AdvancedParameters.ToList();
         allAdvancedParametrs.Each(RemoveChild);
      }
   }
}