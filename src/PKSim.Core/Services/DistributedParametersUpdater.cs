using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IDistributedParametersUpdater
   {
      /// <summary>
      ///    Update the distributions defined in the distributed parameters according to the given origin data
      /// </summary>
      void UpdateDistributedParameter(PathCache<IDistributedParameter> allDistributedParameter, PathCache<IDistributedParameter> allBaseDistributedParameters, OriginData originData);
   }

   public class DistributedParametersUpdater : IDistributedParametersUpdater
   {
      private readonly IParameterQuery _parameterQuery;
      private readonly IDistributionFormulaFactory _distributionFactory;

      public DistributedParametersUpdater(IParameterQuery parameterQuery, IDistributionFormulaFactory distributionFactory)
      {
         _parameterQuery = parameterQuery;
         _distributionFactory = distributionFactory;
      }

      public void UpdateDistributedParameter(PathCache<IDistributedParameter> allDistributedParameter, PathCache<IDistributedParameter> allBaseDistributedParameters, OriginData originData)
      {
         foreach (var distributionsForOneContainer in _parameterQuery.AllParameterDistributionsFor(originData).GroupBy(dist => dist.ParentContainerPath))
         {
            foreach (var distributions in distributionsForOneContainer.GroupBy(x => x.ParameterName))
            {
               var parameterPath = distributionsForOneContainer.Key + ObjectPath.PATH_DELIMITER + distributions.Key;
               var currentParameter = allDistributedParameter[parameterPath];
               var baseParameter = allBaseDistributedParameters[parameterPath];
               if (currentParameter == null || baseParameter==null) 
                  continue;

               _distributionFactory.UpdateDistributionBasedOn(distributions, currentParameter, baseParameter,originData);
            }
         }
      }
   }
}