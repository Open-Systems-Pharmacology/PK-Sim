using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.Mappers;

public class PKSimDiffItemToDiffItemDTOMapper : DiffItemToDiffItemDTOMapper
{
   private readonly IPathAndValueEntityToPathElementsMapper _pathAndValueEntityToPathElementsMapper;

   public PKSimDiffItemToDiffItemDTOMapper(IPathToPathElementsMapper pathElementsMapper, IDisplayNameProvider displayNameProvider, IPathAndValueEntityToPathElementsMapper pathAndValueEntityToPathElementsMapper)
      : base(pathElementsMapper, displayNameProvider, pathAndValueEntityToPathElementsMapper)
   {
      _pathAndValueEntityToPathElementsMapper = pathAndValueEntityToPathElementsMapper;
   }

   public override DiffItemDTO MapFrom(DiffItem diffItem)
   {
      var diffItemDTO = base.MapFrom(diffItem);
      if (diffItem is MismatchDiffItem || diffItem.CommonAncestor is not OverwriteParameterSet overwriteParameterSet)
         return diffItemDTO;

      if (missingParameterValueIn(diffItem) is { } missingParameterValue)
      {
         diffItemDTO.PathElements = _pathAndValueEntityToPathElementsMapper.MapFrom(missingParameterValue);
         diffItemDTO.ObjectName = missingParameterValue.Name;
      }

      diffItemDTO.ObjectName = PKSimConstants.UI.ObjectInOverwriteParameterSet(overwriteParameterSet.Name, diffItemDTO.ObjectName);
      return diffItemDTO;
   }

   private static ParameterValue missingParameterValueIn(DiffItem diffItem) =>
      diffItem is MissingDiffItem missingDiffItem ? (missingDiffItem.MissingObject1 ?? missingDiffItem.MissingObject2) as ParameterValue : null;
}
