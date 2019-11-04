using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Mappers;

namespace PKSim.Core.Mappers
{
   public class PKSimDataColumnToPathElementsMapper : DataColumnToPathElementsMapper
   {
      public PKSimDataColumnToPathElementsMapper(IPathToPathElementsMapper pathToPathElementsMapper) : base(pathToPathElementsMapper)
      {
      }

      protected override PathElements ObservedDataPathElementsFor(DataColumn dataColumn, List<string> quantityPath)
      {
         if (quantityPath.Count!=6)
            return base.ObservedDataPathElementsFor(dataColumn, quantityPath);

         return new PathElements
         {
            [PathElementId.Simulation] = new PathElement {DisplayName = quantityPath[0]},
            [PathElementId.TopContainer] = new PathElement {DisplayName = quantityPath[0]},
            [PathElementId.Container] = new PathElement {DisplayName = quantityPath[2]},
            [PathElementId.BottomCompartment] = new PathElement {DisplayName = quantityPath[3]},
            [PathElementId.Molecule] = new PathElement {DisplayName = quantityPath[4]},
            [PathElementId.Name] = new PathElement {DisplayName = quantityPath[5]}
         };
      }
   }
}