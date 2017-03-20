using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class SimulationAnalysisPresenterContextMenu : ContextMenu<ISimulationAnalysisPresenter, IEditAnalyzablePresenter>
   {
      public SimulationAnalysisPresenterContextMenu(ISimulationAnalysisPresenter simulationAnalysisPresenter, IEditAnalyzablePresenter editAnalyzablePresenter)
         : base(simulationAnalysisPresenter, editAnalyzablePresenter)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ISimulationAnalysisPresenter simulationAnalysisPresenter, IEditAnalyzablePresenter editAnalyzablePresenter)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.Clone)
            .WithActionCommand(() => editAnalyzablePresenter.CloneAnalysis(simulationAnalysisPresenter.Analysis))
            .WithIcon(ApplicationIcons.Clone);
         
         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.Remove)
            .WithActionCommand(() => editAnalyzablePresenter.RemoveAnalysis(simulationAnalysisPresenter))
            .WithIcon(ApplicationIcons.Close);

         yield return CreateMenuButton.WithCaption(PKSimConstants.UI.RemoveAll)
            .WithActionCommand(editAnalyzablePresenter.RemoveAllAnalyses);

         yield return GenericMenu.RenameMenuFor(simulationAnalysisPresenter.Analysis)
            .AsGroupStarter();
      }
   }

   public class SimulationAnalysisPresenterInEditAnalyzablePresenterContextMenuFactory : IContextMenuSpecificationFactory<ISimulationAnalysisPresenter>
   {
      public IContextMenu CreateFor(ISimulationAnalysisPresenter simulationAnalysisPresenter, IPresenterWithContextMenu<ISimulationAnalysisPresenter> presenter)
      {
         return new SimulationAnalysisPresenterContextMenu(simulationAnalysisPresenter, presenter.DowncastTo<IEditAnalyzablePresenter>());
      }

      public bool IsSatisfiedBy(ISimulationAnalysisPresenter simulationAnalysisPresenter, IPresenterWithContextMenu<ISimulationAnalysisPresenter> presenter)
      {
         return presenter.IsAnImplementationOf<IEditAnalyzablePresenter>();
      }
   }
}