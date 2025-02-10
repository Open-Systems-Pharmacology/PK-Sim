using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IModelNeighborhoodQuery
   {
      /// <summary>
      ///    Returns all the neighborhood builders that are defined for the given organism structure and the model properties.
      /// </summary>
      /// <param name="individualNeighborhoods">List of neighborhood already defined in the individual</param>
      /// <param name="modelProperties">Model Properties defining the criteria with which the neighborhoods should be retrieved</param>
      IEnumerable<NeighborhoodBuilder> NeighborhoodsFor(IContainer individualNeighborhoods, ModelProperties modelProperties);
   }
}