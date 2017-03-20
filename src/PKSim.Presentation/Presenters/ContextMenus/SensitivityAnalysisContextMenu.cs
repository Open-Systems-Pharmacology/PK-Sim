using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SensitivityAnalysisContextMenuTreeNodeContextMenuFactory : NodeContextMenuFactory<ClassifiableSensitivityAnalysis>
   {
      public override IContextMenu CreateFor(ClassifiableSensitivityAnalysis classifiableSensitivityAnalysis, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new SensitivityAnalysisContextMenu(classifiableSensitivityAnalysis.SensitivityAnalysis);
      }
   }

   public class SensitivityAnalysisContextMenu : ContextMenu<SensitivityAnalysis>
   {
      public SensitivityAnalysisContextMenu(SensitivityAnalysis sensitivityAnalysis) : base(sensitivityAnalysis)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(SensitivityAnalysis sensitivityAnalysis)
      {
         return SensitivityAnalysisContextMenuItems.ContextMenuItemsFor(sensitivityAnalysis);
      }
   }
}
