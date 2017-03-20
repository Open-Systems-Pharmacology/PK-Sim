using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.Mappers
{
   public class PKSimQuantityPathToQuantityDisplayPathMapper : QuantityPathToQuantityDisplayPathMapper
   {
      public PKSimQuantityPathToQuantityDisplayPathMapper(IObjectPathFactory objectPathFactory, IPathToPathElementsMapper pathToPathElementsMapper, IDataColumnToPathElementsMapper dataColumnToPathElementsMapper) : base(objectPathFactory, pathToPathElementsMapper, dataColumnToPathElementsMapper)
      {
      }

      protected override IEnumerable<PathElement> DefaultPathElementsToUse(bool addSimulationName, PathElements pathElements)
      {
         if(addSimulationName)
            yield return PathElement.Simulation;

         yield return PathElement.Molecule;

         //Container is defined? no need to use TopContainer
         if(!pathElements.Contains(PathElement.Container))
            yield return PathElement.TopContainer;

         yield return PathElement.Container;
         yield return PathElement.BottomCompartment;
         yield return PathElement.Name;
      }
   }
}