using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility.Extensions;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
    public class MultipleSensitivityAnalysesContextMenuFactory : MultipleNodeContextMenuFactory<ClassifiableSensitivityAnalysis>
    {
        private readonly IContainer _container;

        public MultipleSensitivityAnalysesContextMenuFactory(IContainer container)
        {
            _container = container;
        }

        protected override IContextMenu CreateFor(IReadOnlyList<ClassifiableSensitivityAnalysis> nodes, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
        {

            var sensitivityAnalysisList = nodes.Select(pi => pi.SensitivityAnalysis).ToList().AsReadOnly();

            return new MultipleSensitivityAnalysisContextMenu(sensitivityAnalysisList, _container);
        }

        public override bool IsSatisfiedBy(IReadOnlyList<ITreeNode> treeNodes, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
        {
            return treeNodes.All(node => node.IsAnImplementationOf<SensitivityAnalysisNode>());
        }
    }

    public class MultipleSensitivityAnalysisContextMenu : ContextMenu<IReadOnlyList<SensitivityAnalysis>>
    {

        public MultipleSensitivityAnalysisContextMenu(IReadOnlyList<SensitivityAnalysis> nodes, IContainer container)
           : base(nodes, container)
        {
        }

        protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<SensitivityAnalysis> nodes)
        {
            yield return ObjectBaseCommonContextMenuItems.AddToJournal(nodes, _container);
            yield return CreateMenuButton.WithCaption(MenuNames.Delete)
               .WithIcon(ApplicationIcons.Delete)
               .WithCommandFor<RemoveMultipleSensitivityAnalysisUICommand, IReadOnlyList<SensitivityAnalysis>>(nodes, _container)
               .AsGroupStarter();
        }
    }
}