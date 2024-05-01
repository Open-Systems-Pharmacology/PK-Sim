using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleParameterIdentificationContextMenuFactory : MultipleNodeContextMenuFactory<ClassifiableParameterIdentification>
   {

      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleParameterIdentificationContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      public override bool IsSatisfiedBy(IReadOnlyList<ITreeNode> treeNodes, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return treeNodes.All(node => node.IsAnImplementationOf<ParameterIdentificationNode>());
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ClassifiableParameterIdentification> parameterIdentifications, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         var paramIdentificationList = parameterIdentifications.Select(pi => pi.ParameterIdentification).ToList().AsReadOnly();
         return new MultipleParameterIdentificationContextMenu(paramIdentificationList, _executionContext, _container);
      }

      public class MultipleParameterIdentificationContextMenu : ContextMenu<IReadOnlyList<ParameterIdentification>, IExecutionContext>
      {
         public MultipleParameterIdentificationContextMenu(IReadOnlyList<ParameterIdentification> parameterIdentifications, IExecutionContext context, IContainer container)
            : base(parameterIdentifications, context, container)
         {
         }

         protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<ParameterIdentification> parameterIdentifications, IExecutionContext context)
         {
            yield return ObjectBaseCommonContextMenuItems.AddToJournal(parameterIdentifications, _container);

            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
               .WithCommandFor<RemoveMultipleParameterIdentificationsUICommand, IReadOnlyList<ParameterIdentification>>(parameterIdentifications, _container)
               .WithIcon(ApplicationIcons.Delete)
               .AsGroupStarter();
         }
      }
   }
}