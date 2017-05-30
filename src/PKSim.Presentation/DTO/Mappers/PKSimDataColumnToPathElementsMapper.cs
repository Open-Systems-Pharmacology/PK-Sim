using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
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
            [PathElement.Simulation] = new PathElementDTO {DisplayName = quantityPath[0]},
            [PathElement.TopContainer] = new PathElementDTO {DisplayName = quantityPath[0]},
            [PathElement.Container] = new PathElementDTO {DisplayName = quantityPath[2]},
            [PathElement.BottomCompartment] = new PathElementDTO {DisplayName = quantityPath[3]},
            [PathElement.Molecule] = new PathElementDTO {DisplayName = quantityPath[4]},
            [PathElement.Name] = new PathElementDTO {DisplayName = quantityPath[5]}
         };
      }
   }
}