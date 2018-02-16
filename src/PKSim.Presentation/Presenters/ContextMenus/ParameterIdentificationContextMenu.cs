using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ParameterIdentificationContextMenu : ContextMenu<ParameterIdentification>
   {
      public ParameterIdentificationContextMenu(ParameterIdentification parameterIdentification) : base(parameterIdentification)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ParameterIdentification parameterIdentification)
      {
         return ParameterIdentificationContextMenuItems.ContextMenuItemsFor(parameterIdentification);
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