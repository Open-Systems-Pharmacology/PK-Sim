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
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ParameterIdentificationContextMenu : ContextMenu<ParameterIdentification>
   {
      public ParameterIdentificationContextMenu(ParameterIdentification parameterIdentification, IContainer container) : base(parameterIdentification, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ParameterIdentification parameterIdentification)
      {
         return new List<IMenuBarItem>(ParameterIdentificationContextMenuItems.ContextMenuItemsFor(parameterIdentification, _container))
         {
            exportParameterIdentificationToR(parameterIdentification)
         };
      }

      private IMenuBarItem exportParameterIdentificationToR(ParameterIdentification parameterIdentification)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Export for R®"))
            .WithCommandFor<ExportParameterIdentificationToRUICommand, ParameterIdentification>(parameterIdentification, _container)
            .WithIcon(ApplicationIcons.R)
            .AsGroupStarter()
            .ForDeveloper();
      }
   }

   public class ParameterIdentificationContextMenuTreeNodeContextMenuFactory : NodeContextMenuFactory<ClassifiableParameterIdentification>
   {
      private readonly IContainer _container;

      public ParameterIdentificationContextMenuTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ClassifiableParameterIdentification classifiableParameterIdentification, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ParameterIdentificationContextMenu(classifiableParameterIdentification.ParameterIdentification, _container);
      }
   }
}