using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundInteractionSelectionPresenter : IEditSimulationPresenter, IConfigurationPresenter
   {
      void CompoundProcessSelectionChanged(SimulationInteractionProcessSelectionDTO dto, PartialProcess interactionProcess);
      IEnumerable<InteractionProcess> AllCompoundProcesses();
      void AddInteraction();
      void RemoveInteraction(SimulationInteractionProcessSelectionDTO dto);
      void IndividualMoleculeSelectionChanged(SimulationInteractionProcessSelectionDTO dto, IndividualMolecule individualMolecule);
      IEnumerable<IndividualMolecule> AllIndividualMolecules();
   }

   public class SimulationCompoundInteractionSelectionPresenter : AbstractSubPresenter<ISimulationCompoundInteractionSelectionView, ISimulationCompoundInteractionSelectionPresenter>, ISimulationCompoundInteractionSelectionPresenter
   {
      private readonly IInteractionProcessRetriever _interactionProcessRetriever;
      private List<InteractionProcess> _allInteractionProcesses;
      private IReadOnlyList<SimulationPartialProcess> _allPartialProcesses;
      private List<SimulationInteractionProcessSelectionDTO> _allInteractionProcessesDTO;
      private IEnumerable<IndividualMolecule> _allMolecules;
      private readonly NoInteractionProcess _noInteractionProcessSelection;
      private InteractionProperties _interactionProperties;

      public SimulationCompoundInteractionSelectionPresenter(ISimulationCompoundInteractionSelectionView view, IInteractionProcessRetriever interactionProcessRetriever) : base(view)
      {
         _interactionProcessRetriever = interactionProcessRetriever;
         _noInteractionProcessSelection = new NoInteractionProcess();
         _interactionProcessRetriever.NotSelectedInteractionProcess = _noInteractionProcessSelection;
         _view.Warning = PKSimConstants.Warning.InhibitorClearanceMustBeDefinedSeparately;
      }

      public void SaveConfiguration()
      {
         _interactionProperties.ClearInteractions();
         selectedInteractions().Each(_interactionProperties.AddInteraction);
      }

      private IEnumerable<InteractionSelection> selectedInteractions()
      {
         return _allInteractionProcessesDTO.Select(mapFrom);
      }

      public void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         _allMolecules = simulation.Individual.AllDefinedMolecules().ToList();
         _interactionProperties = simulation.InteractionProperties;
         _allInteractionProcesses = simulation.Compounds.SelectMany(x => x.AllProcesses<InteractionProcess>()).ToList();
         _allInteractionProcesses.Insert(0, _noInteractionProcessSelection);
         var shouldAddDefaultMapping = creationMode == CreationMode.New;
         _allPartialProcesses = _interactionProcessRetriever.AllFor(simulation, simulation.InteractionProperties.Interactions, shouldAddDefaultMapping);
         _allInteractionProcessesDTO = mapPartialProcesses(_allPartialProcesses);
         rebind();
      }

      private List<SimulationInteractionProcessSelectionDTO> mapPartialProcesses(IEnumerable<SimulationPartialProcess> partialProcesses)
      {
         return partialProcesses.Select(mapFrom).ToList();
      }

      private InteractionSelection mapFrom(SimulationInteractionProcessSelectionDTO dto)
      {
         return new InteractionSelection
         {
            MoleculeName = dto.IndividualMolecule.Name,
            ProcessName = isSelected(dto) ? dto.CompoundProcess.Name : string.Empty,
            CompoundName = dto.CompoundName
         };
      }

      private SimulationInteractionProcessSelectionDTO mapFrom(SimulationPartialProcess x)
      {
         return new SimulationInteractionProcessSelectionDTO(x);
      }

      public void CompoundProcessSelectionChanged(SimulationInteractionProcessSelectionDTO dto, PartialProcess interactionProcess)
      {
         if (processAlreadySelected(interactionProcess))
            throw new CannotSelectThePartialProcessMoreThanOnceException(interactionProcess);

         dto.SimulationPartialProcess.CompoundProcess = interactionProcess;
         updateWaring();
      }

      private void updateWaring()
      {
         _view.WarningVisible = _allInteractionProcessesDTO.Select(x=>x.CompoundProcess).OfType<InteractionProcess>().Any(x => x.InteractionType == InteractionType.IrreversibleInhibition);
      }

      private bool processAlreadySelected(PartialProcess interactionProcess)
      {
         return isSelected(interactionProcess) &&
                _allInteractionProcessesDTO.Any(x => Equals(x.CompoundProcess, interactionProcess));
      }

      private bool isSelected(SimulationInteractionProcessSelectionDTO dto)
      {
         return isSelected(dto.CompoundProcess);
      }

      private bool isSelected(PartialProcess interactionProcess)
      {
         return !Equals(interactionProcess, _noInteractionProcessSelection);
      }

      private void rebind()
      {
         _view.BindTo(_allInteractionProcessesDTO);
         updateWaring();
      }

      public IEnumerable<InteractionProcess> AllCompoundProcesses()
      {
         return _allInteractionProcesses;
      }

      public void AddInteraction()
      {
         _allInteractionProcessesDTO.Add(mapFrom(defaultInteractionProcess));
         rebind();
      }

      public void RemoveInteraction(SimulationInteractionProcessSelectionDTO dto)
      {
         _allInteractionProcessesDTO.Remove(dto);
         rebind();
      }

      private SimulationPartialProcess defaultInteractionProcess => new SimulationPartialProcess
      {
         IndividualMolecule = _allMolecules.FirstOrDefault(),
         CompoundProcess = _noInteractionProcessSelection
      };

      public void IndividualMoleculeSelectionChanged(SimulationInteractionProcessSelectionDTO dto, IndividualMolecule individualMolecule)
      {
         dto.SimulationPartialProcess.IndividualMolecule = individualMolecule;
      }

      public IEnumerable<IndividualMolecule> AllIndividualMolecules()
      {
         return _allMolecules;
      }
   }
}