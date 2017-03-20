using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IEditSimulationPresenter<TSimulation> : IEditAnalyzablePresenter<TSimulation>, IEditBuildingBockPresenter<TSimulation> where TSimulation : Simulation
   {
   }

   public abstract class EditSimulationPresenter<TView, TPresenter, TSimulation, TSubPresenter> : EditAnalyzablePresenter<TView, TPresenter, TSimulation, TSubPresenter>
      where TSimulation : Simulation
      where TView : IView<TPresenter>, IEditAnalyzableView
      where TPresenter : IPresenter
      where TSubPresenter : IEditSimulationItemPresenter<TSimulation>
   {
      protected EditSimulationPresenter(TView view, ISubPresenterItemManager<TSubPresenter> subPresenterItemManager, IReadOnlyList<ISubPresenterItem> subPresenterItems, 
         ISimulationAnalysisPresenterFactory simulationAnalysisPresenterFactory, ISimulationAnalysisPresenterContextMenuFactory contextMenuFactory, 
         IPresentationSettingsTask presentationSettingsTask, ISimulationAnalysisCreator simulationAnalysisCreator)
         : base(view, subPresenterItemManager, subPresenterItems, simulationAnalysisPresenterFactory, contextMenuFactory, presentationSettingsTask, simulationAnalysisCreator)
      {
      }

      protected override void InitializeSubPresentersWith(TSimulation simulation)
      {
         _subPresenterItemManager.AllSubPresenters.Each(x => x.EditSimulation(simulation));
      }

      protected override bool CanHandle(AnalysableEvent analysableEvent)
      {
         return Equals(analysableEvent.Analysable, Analyzable);
      }
   }
}