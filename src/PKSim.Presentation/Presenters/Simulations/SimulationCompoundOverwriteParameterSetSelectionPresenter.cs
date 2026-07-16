using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundOverwriteParameterSetSelectionPresenter : IEditSimulationCompoundPresenter, IPresenter<ISimulationCompoundOverwriteParameterSetSelectionView>
   {
   }

   public class SimulationCompoundOverwriteParameterSetSelectionPresenter : AbstractSubPresenter<ISimulationCompoundOverwriteParameterSetSelectionView, ISimulationCompoundOverwriteParameterSetSelectionPresenter>,
      ISimulationCompoundOverwriteParameterSetSelectionPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly OverwriteParameterSet _noOverwriteParameterSet;
      private Simulation _simulation;
      private Compound _compound;
      private SimulationCompoundOverwriteParameterSetSelectionDTO _dto;

      public SimulationCompoundOverwriteParameterSetSelectionPresenter(
         ISimulationCompoundOverwriteParameterSetSelectionView view,
         ILazyLoadTask lazyLoadTask) : base(view)
      {
         _lazyLoadTask = lazyLoadTask;
         _noOverwriteParameterSet = new OverwriteParameterSet { Name = PKSimConstants.UI.None };
      }

      public void EditSimulation(Simulation simulation, Compound compound)
      {
         _simulation = simulation;
         _compound = compound;
         _lazyLoadTask.Load(_compound);

         var availableSets = new List<OverwriteParameterSet> { _noOverwriteParameterSet };
         availableSets.AddRange(_compound.OverwriteParameterSets);

         var selected = currentSelectionFor(simulation, compound);
         _dto = new SimulationCompoundOverwriteParameterSetSelectionDTO(availableSets, selected);
         _view.BindTo(_dto);
      }

      public void SaveConfiguration()
      {
         if (_dto == null) return;

         if (isNoSelection(_dto.SelectedOverwriteParameterSet))
            _simulation.RemoveOverwriteParameterSetSelection(_compound.Name);
         else
            _simulation.AddOverwriteParameterSetSelection(_compound.Name, _dto.SelectedOverwriteParameterSet);
      }

      private OverwriteParameterSet currentSelectionFor(Simulation simulation, Compound compound)
      {
         var existing = simulation.OverwriteParameterSetSelections.SelectedSetFor(compound.Name);
         if (existing != null)
            return existing;

         var defaultSet = compound.OverwriteParameterSets.FirstOrDefault(x => x.IsDefault);
         return defaultSet ?? _noOverwriteParameterSet;
      }

      private bool isNoSelection(OverwriteParameterSet overwriteParameterSet) =>
         overwriteParameterSet == null || ReferenceEquals(overwriteParameterSet, _noOverwriteParameterSet);
   }
}
