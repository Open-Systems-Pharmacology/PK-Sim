using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class CompoundProcessContextMenu<TCompoundProcess> : ContextMenu<TCompoundProcess, ICompoundProcessesPresenter> where TCompoundProcess : PKSim.Core.Model.CompoundProcess
   {
      protected CompoundProcessContextMenu(TCompoundProcess compoundProcess, ICompoundProcessesPresenter presenter)
         : base(compoundProcess, presenter)
      {
      }


      protected IMenuBarItem DeleteMenuFor(TCompoundProcess compoundProcess, ICompoundProcessesPresenter presenter)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithActionCommand(() => presenter.RemoveProcess(compoundProcess))
            .WithIcon(ApplicationIcons.Delete);
      }
   }

   public abstract class CompoundProcessTreeNodeContextMenuFactory<TCompoundProcess> : NodeContextMenuFactory<TCompoundProcess> where TCompoundProcess : PKSim.Core.Model.CompoundProcess
   {
      public override IContextMenu CreateFor(TCompoundProcess compoundProcess, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return CreateFor(compoundProcess, presenter.DowncastTo<ICompoundProcessesPresenter>());
      }

      protected abstract IContextMenu CreateFor(TCompoundProcess compoundProcess, ICompoundProcessesPresenter compoundProcessesPresenter);

      public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return base.IsSatisfiedBy(treeNode, presenter) && presenter.IsAnImplementationOf<ICompoundProcessesPresenter>();
      }
   }
}