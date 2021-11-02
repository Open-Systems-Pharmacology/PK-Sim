using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundsSelectionPresenter : ISimulationModelConfigurationItemPresenter
   {
      /// <summary>
      ///    Is triggered whenever a compound is selected or deselected in the selection view
      /// </summary>
      void SelectionChanged(CompoundSelectionDTO compoundSelectionDTO);

      /// <summary>
      ///    Adds a compound to the project
      /// </summary>
      void AddCompound();

      /// <summary>
      ///    Loads a compound into the project
      /// </summary>
      Task LoadCompoundAsync();

      /// <summary>
      ///    Returns the compounds selected by the user
      /// </summary>
      IReadOnlyList<Compound> SelectedCompounds { get; }

      IEnumerable<ToolTipPart> ToolTipFor(CompoundSelectionDTO compoundSelectionDTO);

      /// <summary>
      /// Ensures that the given <paramref name="templateCompound"/> is used instead of the compound with same name that might have changed in the simulation
      /// </summary>
      /// <param name="templateCompound">The template compound that should be selected</param>
      void UpdateSelectedCompound(Compound templateCompound);
   }

   public class SimulationCompoundsSelectionPresenter : AbstractSubPresenter<ISimulationCompoundsSelectionView, ISimulationCompoundsSelectionPresenter>, ISimulationCompoundsSelectionPresenter
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly ICompoundTask _compoundTask;
      private readonly IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;
      private readonly NotifyList<CompoundSelectionDTO> _compoundSelectionDTOs;

      public SimulationCompoundsSelectionPresenter(ISimulationCompoundsSelectionView view, IBuildingBlockRepository buildingBlockRepository,
         IBuildingBlockInSimulationManager buildingBlockInSimulationManager, ICompoundTask compoundTask, IBuildingBlockSelectionDisplayer buildingBlockSelectionDisplayer)
         : base(view)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _compoundTask = compoundTask;
         _buildingBlockSelectionDisplayer = buildingBlockSelectionDisplayer;
         _compoundSelectionDTOs = new NotifyList<CompoundSelectionDTO>();
         _view.BindTo(_compoundSelectionDTOs);
         _compoundSelectionDTOs.CollectionChanged += onCollectionChanged;
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         bindToCompounds(_buildingBlockInSimulationManager.TemplateBuildingBlocksUsedBy<Compound>(simulation));
      }

      public void UpdateSelectedCompound(Compound templateCompound)
      {
         var dto = dtoFor(templateCompound);
         if (dto == null) return;
         dto.Selected = true;
      }

      public void SelectionChanged(CompoundSelectionDTO compoundSelectionDTO)
      {
         removeDuplicateSelection(compoundSelectionDTO);
         updateErrorMessage();
      }

      private void removeDuplicateSelection(CompoundSelectionDTO compoundSelectionDTO)
      {
         //compound was unselected. Nothing to check
         if (!compoundSelectionDTO.Selected) return;
         var selectedCompoundName = compoundSelectionDTO.BuildingBlock.Name;
         var allOtherSelectedCompoundsDTO = selectedCompoundDTOs.ToList();
         allOtherSelectedCompoundsDTO.Remove(compoundSelectionDTO);

         var allOtherCompoundWithSameName = allOtherSelectedCompoundsDTO.Where(x => x.BuildingBlock.IsNamed(selectedCompoundName));
         allOtherCompoundWithSameName.Each(dto => dto.Selected = false);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _compoundSelectionDTOs.CollectionChanged -= onCollectionChanged;
      }

      private void onCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
      {
         updateErrorMessage();
      }

      private void updateErrorMessage()
      {
         if (compoundSelectionsIsValid())
            _view.HideError();
         else if (!atLeastOneCompoundIsSelected())
            _view.SetError(PKSimConstants.Error.AtLeastOneCompoundMustBeSelected);

         _view.AdjustHeight();
      }

      public void AddCompound()
      {
         addToSelection(_compoundTask.AddToProject());
      }

      private void addToSelection(Compound compoundAdded)
      {
         if (compoundAdded == null) return;
         _compoundSelectionDTOs.Add(mapFrom(compoundAdded, isSelected: true));
      }

      public async Task LoadCompoundAsync()
      {
         addToSelection(await _compoundTask.LoadSingleFromTemplateAsync());
      }

      public override void Initialize()
      {
         base.Initialize();
         bindToTemplateBuildingBlocks();
      }

      private void bindToTemplateBuildingBlocks()
      {
         bindToCompounds(new List<Compound>());
      }

      private void bindToCompounds(IReadOnlyList<Compound> usedCompounds)
      {
         _compoundSelectionDTOs.Clear();
         var availableCompounds = _buildingBlockRepository.All<Compound>().Union(usedCompounds).OrderBy(x => x.Name).ToList();

         availableCompounds.Each(c => _compoundSelectionDTOs.Add(mapFrom(c, isSelected: shouldSelectCompoundByDefault(c, usedCompounds, availableCompounds))));
      }

      private static bool shouldSelectCompoundByDefault(Compound compound, IEnumerable<Compound> usedCompounds, IReadOnlyCollection<Compound> availableCompounds)
      {
         return (usedCompounds.Contains(compound) || availableCompounds.Count == 1);
      }

      private CompoundSelectionDTO mapFrom(Compound compound, bool isSelected)
      {
         return new CompoundSelectionDTO
         {
            BuildingBlock = compound,
            Selected = isSelected,
            DisplayName = compoundNameFor(compound)
         };
      }

      private string compoundNameFor(Compound compound)
      {
         return _buildingBlockSelectionDisplayer.DisplayNameFor(compound);
      }

      public override bool CanClose => base.CanClose && compoundSelectionsIsValid();

      private bool compoundSelectionsIsValid()
      {
         return atLeastOneCompoundIsSelected();
      }

      private bool atLeastOneCompoundIsSelected()
      {
         return SelectedCompounds.Any();
      }

      public IReadOnlyList<Compound> SelectedCompounds
      {
         get { return selectedCompoundDTOs.Select(x => x.BuildingBlock).ToList(); }
      }

      private IEnumerable<CompoundSelectionDTO> selectedCompoundDTOs
      {
         get { return _compoundSelectionDTOs.Where(x => x.Selected); }
      }

      public IEnumerable<ToolTipPart> ToolTipFor(CompoundSelectionDTO compoundSelectionDTO)
      {
         return _buildingBlockSelectionDisplayer.ToolTipFor(compoundSelectionDTO.BuildingBlock);
      }

      private CompoundSelectionDTO dtoFor(Compound compound)
      {
         return _compoundSelectionDTOs.FirstOrDefault(x => Equals(x.BuildingBlock, compound));
      }
   }
}