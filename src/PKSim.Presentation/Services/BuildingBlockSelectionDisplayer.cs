using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Services
{
   public interface IBuildingBlockSelectionDisplayer
   {
      /// <summary>
      ///    Returns the name to be displayed for the given <paramref name="buildingBlock" /> in the context of a building block
      ///    selection.
      ///    If the <paramref name="emptySelection" /> is not null, it will be used to identify a non existant selection
      /// </summary>
      string DisplayNameFor(IPKSimBuildingBlock buildingBlock, IPKSimBuildingBlock emptySelection = null);

      /// <summary>
      ///    Returns the icon to be displayed for the given <paramref name="buildingBlock" /> in the context of a building block
      ///    selection.
      ///    If the <paramref name="emptySelection" /> is not null, it will be used to identify a non existant selection
      /// </summary>
      ApplicationIcon IconFor(IPKSimBuildingBlock buildingBlock, IPKSimBuildingBlock emptySelection = null);

      /// <summary>
      ///    Returns the tooltip for the given <paramref name="buildingBlock" />
      /// </summary>
      IEnumerable<ToolTipPart> ToolTipFor(IPKSimBuildingBlock buildingBlock);
   }

   public class BuildingBlockSelectionDisplayer : IBuildingBlockSelectionDisplayer
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IToolTipPartCreator _toolTipPartCreator;

      public BuildingBlockSelectionDisplayer(IBuildingBlockRepository buildingBlockRepository, IToolTipPartCreator toolTipPartCreator)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _toolTipPartCreator = toolTipPartCreator;
      }

      public string DisplayNameFor(IPKSimBuildingBlock buildingBlock, IPKSimBuildingBlock emptySelection = null)
      {
         if (buildingBlock == null)
            return string.Empty;

         return isTemplate(buildingBlock, emptySelection) ? buildingBlock.Name : nonTemplateDisplayFor(buildingBlock);
      }

      public ApplicationIcon IconFor(IPKSimBuildingBlock buildingBlock, IPKSimBuildingBlock emptySelection = null)
      {
         if (buildingBlock == null)
            return ApplicationIcons.EmptyIcon;

         var buildingBlockIcon = ApplicationIcons.IconByName(buildingBlock.Icon);
         return isTemplate(buildingBlock, emptySelection) ? buildingBlockIcon : ApplicationIcons.RedOverlayFor(buildingBlockIcon);
      }

      public IEnumerable<ToolTipPart> ToolTipFor(IPKSimBuildingBlock buildingBlock)
      {
         if (buildingBlock == null)
            return Enumerable.Empty<ToolTipPart>();

         return _toolTipPartCreator.ToolTipFor(buildingBlock);
      }

      private static string nonTemplateDisplayFor(IPKSimBuildingBlock buildingBlock)
      {
         return $"{buildingBlock.Name} - <color=red><B>{PKSimConstants.Warning.ThisItNotATemplateBuildingBlock}</B></color>";
      }

      private bool isTemplate(IPKSimBuildingBlock buildingBlock, IPKSimBuildingBlock emptySelection)
      {
         return Equals(buildingBlock, emptySelection)
                || _buildingBlockRepository.ById(buildingBlock.Id) != null;
      }
   }
}