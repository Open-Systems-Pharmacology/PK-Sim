using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ISimulationSelectionForComparisonPresenter : IPresenter<ISimulationSelectionForComparisonView>, IDisposablePresenter
   {
      IEnumerable<Simulation> AvailableSimulations();
      IEnumerable<Symbols> AllSymbols();
      void ReferenceSimulationChanged();
      bool ReferenceSimulationIs(ISimulation simulation);
      bool Edit(PopulationSimulationComparison simulationComparison);
   }

   public class SimulationSelectionForComparisonPresenter : AbstractDisposablePresenter<ISimulationSelectionForComparisonView, ISimulationSelectionForComparisonPresenter>, ISimulationSelectionForComparisonPresenter
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly SimulationComparisonSelectionDTO _selectionDTO;
      private readonly NullSimulation _nullSimulation;

      public SimulationSelectionForComparisonPresenter(ISimulationSelectionForComparisonView view, IBuildingBlockRepository buildingBlockRepository, ILazyLoadTask lazyLoadTask) : base(view)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _lazyLoadTask = lazyLoadTask;
         _selectionDTO = new SimulationComparisonSelectionDTO();
         _nullSimulation = new NullSimulation();
         _selectionDTO.Reference = _nullSimulation;
         _selectionDTO.GroupingItem.Color = PKSimColors.StartGroupingColor;
      }

      public IEnumerable<Simulation> AvailableSimulations()
      {
         yield return _nullSimulation;

         foreach (var simulation in allPopulationSimulations)
         {
            yield return simulation;
         }
      }

      private IEnumerable<PopulationSimulation> allPopulationSimulations
      {
         get { return _buildingBlockRepository.All<PopulationSimulation>().OrderBy(x => x.Name); }
      }

      public IEnumerable<Symbols> AllSymbols()
      {
         return EnumHelper.AllValuesFor<Symbols>();
      }

      public void ReferenceSimulationChanged()
      {
         var simulationSelection = _selectionDTO.AllSimulations.FirstOrDefault(x => ReferenceSimulationIs(x.Simulation));
         if (simulationSelection == null) return;
         simulationSelection.Selected = true;
      }

      public bool ReferenceSimulationIs(ISimulation simulation)
      {
         return Equals(_selectionDTO.Reference, simulation);
      }

      public bool Edit(PopulationSimulationComparison simulationComparison)
      {
         updateSelectionFromComparison(simulationComparison);

         _view.BindTo(_selectionDTO);
         ViewChanged();
         _view.Display();

         if (_view.Canceled)
            return false;

         simulationComparison.RemoveAllSimulations();
         selectedSimulations.Each(s =>
         {
            _lazyLoadTask.Load(s);
            simulationComparison.AddSimulation(s);
         });

         //add reference settings
         simulationComparison.ReferenceSimulation = referenceSimulation;
         simulationComparison.ReferenceGroupingItem = referenceGroupingItem;

         return true;
      }

      private void updateSelectionFromComparison(PopulationSimulationComparison simulationComparison)
      {
         _selectionDTO.AllSimulations = allPopulationSimulations.Select(s => mapFrom(s, simulationComparison)).ToList();
         if (simulationComparison.HasReference)
            _selectionDTO.Reference = simulationComparison.ReferenceSimulation;

         if(simulationComparison.ReferenceGroupingItem!=null)
           _selectionDTO.GroupingItem.UpdateFrom(simulationComparison.ReferenceGroupingItem);
      }

      private IEnumerable<PopulationSimulation> selectedSimulations
      {
         get
         {
            return _selectionDTO.AllSimulations.Where(x => x.Selected)
               .Select(x => x.Simulation).Cast<PopulationSimulation>().ToList();
         }
      }

      private PopulationSimulation referenceSimulation
      {
         get { return _selectionDTO.Reference as PopulationSimulation; }
      }

      private GroupingItem referenceGroupingItem
      {
         get { return _selectionDTO.GroupingItem.ToGroupingItem(); }
      }

      private PopulationSimulationSelectionDTO mapFrom(PopulationSimulation populationSimulation, PopulationSimulationComparison simulationComparison)
      {
         return new PopulationSimulationSelectionDTO(populationSimulation)
         {
            Selected = simulationComparison.HasSimulation(populationSimulation)
         };
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         _view.OkEnabled = CanClose;
      }

      public override bool CanClose
      {
         get { return base.CanClose && selectedSimulations.Any(); }
      }
   }
}