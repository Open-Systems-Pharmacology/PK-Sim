using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SensitivityAnalysisContextMenuTreeNodeContextMenuFactory : NodeContextMenuFactory<ClassifiableSensitivityAnalysis>
   {
      private readonly IContainer _container;

      public SensitivityAnalysisContextMenuTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ClassifiableSensitivityAnalysis classifiableSensitivityAnalysis, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SensitivityAnalysisContextMenu(classifiableSensitivityAnalysis.SensitivityAnalysis, _container);
      }
   }

   public class SensitivityAnalysisContextMenu : ContextMenu<SensitivityAnalysis>
   {
      public SensitivityAnalysisContextMenu(SensitivityAnalysis sensitivityAnalysis, IContainer container) : base(sensitivityAnalysis, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(SensitivityAnalysis sensitivityAnalysis)
      {
         return SensitivityAnalysisContextMenuItems.ContextMenuItemsFor(sensitivityAnalysis, _container);
      }
   }
}
