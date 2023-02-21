using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;

namespace PKSim.Core.Mappers
{
   public class PKSimQuantityPathToQuantityDisplayPathMapper : QuantityPathToQuantityDisplayPathMapper
   {
      public PKSimQuantityPathToQuantityDisplayPathMapper(ObjectPathFactory objectPathFactory, IPathToPathElementsMapper pathToPathElementsMapper, IDataColumnToPathElementsMapper dataColumnToPathElementsMapper) : base(objectPathFactory, pathToPathElementsMapper, dataColumnToPathElementsMapper)
      {
      }

      protected override IEnumerable<PathElementId> DefaultPathElementsToUse(bool addSimulationName, PathElements pathElements)
      {
         if(addSimulationName)
            yield return PathElementId.Simulation;

         yield return PathElementId.Molecule;

         //Container is defined? no need to use TopContainer
         if(!pathElements.Contains(PathElementId.Container))
            yield return PathElementId.TopContainer;

         yield return PathElementId.Container;
         yield return PathElementId.BottomCompartment;
         yield return PathElementId.Name;
      }
   }
}