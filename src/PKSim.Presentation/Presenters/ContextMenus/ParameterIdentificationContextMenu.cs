using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Assets;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ParameterIdentificationContextMenu : ContextMenu<ParameterIdentification>
   {
      public ParameterIdentificationContextMenu(ParameterIdentification parameterIdentification) : base(parameterIdentification)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ParameterIdentification parameterIdentification)
      {
         return new List<IMenuBarItem>(ParameterIdentificationContextMenuItems.ContextMenuItemsFor(parameterIdentification))
         {
            exportParameterIdentificationToR(parameterIdentification)
         };
      }

      private IMenuBarItem exportParameterIdentificationToR(ParameterIdentification parameterIdentification)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Export to R®"))
            .WithCommandFor<ExportParameterIdentificationToRUICommand, ParameterIdentification>(parameterIdentification)
            .WithIcon(ApplicationIcons.R)
            .AsGroupStarter()
            .ForDeveloper();
      }
   }

   public class ParameterIdentificationContextMenuTreeNodeContextMenuFactory : NodeContextMenuFactory<ClassifiableParameterIdentification>
   {
      public override IContextMenu CreateFor(ClassifiableParameterIdentification classifiableParameterIdentification, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ParameterIdentificationContextMenu(classifiableParameterIdentification.ParameterIdentification);
      }
   }
}