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
   public interface ISimulationCompoundProcessPresenter : IPresenter
   {
      /// <summary>
      ///    Saves the current mapping in the compound properties
      /// </summary>
      void SaveConfiguration();

      /// <summary>
      ///    Initializes the presenter with the simulation and the given compoundProperties
      /// </summary>
      void EditProcessesIn(Simulation simulation, CompoundProperties compoundProperties);

      /// <summary>
      ///    Returns the systemic processes available in the compound for the given selection
      /// </summary>
      IEnumerable<SystemicProcess> AllSystemicProcessesFor(SimulationSystemicProcessSelectionDTO systemicProcessSelectionDTO);

      /// <summary>
      ///    Is called whenever the selection of a systemic process has changed
      /// </summary>
      void SelectionChanged(SimulationSystemicProcessSelectionDTO systemicProcessSelectionDTO);

      /// <summary>
      ///    True if the subject of this presenter has any processes defined
      /// </summary>
      bool HasProcessesDefined { get; }
   }

   public interface ISimulationCompoundProcessPresenter<TPartialProcess, TPartialProcessDTO> : IPresenter<ISimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO>>, ISimulationCompoundProcessPresenter
      where TPartialProcess : PartialProcess
      where TPartialProcessDTO : SimulationPartialProcessSelectionDTO
   {
      /// <summary>
      ///    Returns the available proteins defined for the given partial process type
      /// </summary>
      IEnumerable<TPartialProcess> AllCompoundProcesses();

      /// <summary>
      ///    Called whenever the selected partial process is being changed
      /// </summary>
      /// <param name="simulationPartialProcessDTO">actual selection</param>
      /// <param name="partialProcess">Process being selected</param>
      void CompoundProcessChanged(TPartialProcessDTO simulationPartialProcessDTO, PartialProcess partialProcess);

      /// <summary>
      ///    Is called whenever the selection of a partial process has changed
      /// </summary>
      void SelectionChanged(TPartialProcessDTO simulationPartialProcessDTO);
   }

   public abstract class SimulationCompoundProcessPresenter<TMolecule, TPartialProcess, TPartialProcessDTO> :
      AbstractPresenter<ISimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO>, ISimulationCompoundProcessPresenter<TPartialProcess, TPartialProcessDTO>>,
      ISimulationCompoundProcessPresenter<TPartialProcess, TPartialProcessDTO>
      where TMolecule : IndividualMolecule
      where TPartialProcess : PartialProcess, new()
      where TPartialProcessDTO : SimulationPartialProcessSelectionDTO
   {
      private readonly IPartialProcessRetriever _partialProcessRetriever;
      protected IReadOnlyCollection<TPartialProcessDTO> _allPartialProcessesDTO;
      private IEnumerable<TPartialProcess> _allPartialProcesses;
      protected List<SimulationSystemicProcessSelectionDTO> _allSystemicProcessesDTO;
      protected Compound _compound;
      private NotSelectedSystemicProcess _notSelectedSystemicProcess;
      private NotAvailableSystemicProcess _notAvailableSystemicProcess;
      protected IEnumerable<SimulationPartialProcess> _selectedProcesses;
      protected CompoundProperties _compoundProperties;
      protected Simulation _simulation;
      protected readonly TPartialProcess _notSelectedPartialProcess;

      protected SimulationCompoundProcessPresenter(ISimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO> view, IPartialProcessRetriever partialProcessRetriever, string moleculeDisplayName, string compoundProcessCaption)
         : base(view)
      {
         _partialProcessRetriever = partialProcessRetriever;
         View.MoleculeName = moleculeDisplayName;
         View.CompoundProcessCaption = compoundProcessCaption;
         _notSelectedPartialProcess = new TPartialProcess {MoleculeName = PKSimConstants.UI.None};
         _notSelectedPartialProcess.RefreshName();
         _partialProcessRetriever.NotSelectedPartialProcess = _notSelectedPartialProcess;
      }

      public virtual void EditProcessesIn(Simulation simulation, CompoundProperties compoundProperties)
      {
         _simulation = simulation;
         _compound = compoundProperties.Compound;
         _compoundProperties = compoundProperties;
         _notSelectedSystemicProcess = new NotSelectedSystemicProcess();
         _notAvailableSystemicProcess = new NotAvailableSystemicProcess();

         _allPartialProcesses = _compound.AllProcesses<TPartialProcess>();

         var partialProcessSelection = ProcessSelectionGroup().AllPartialProcesses();

         _selectedProcesses = _partialProcessRetriever.AllFor<TMolecule,TPartialProcess>(simulation, _compound, partialProcessSelection, addDefaultPartialProcess: true);

         _allPartialProcessesDTO = MapPartialProcesses(_selectedProcesses).ToList();

         createSystemicProcessesFor(simulation);

         _view.BindToPartialProcesses(_allPartialProcessesDTO);
         _view.BindToSystemicProcesses(_allSystemicProcessesDTO);

         ValidateProcessSelection();


         //this needs to be done after binding to the processes, since binding affects the height of the view
         _view.AdjustHeight();
      }

      protected abstract IEnumerable<TPartialProcessDTO> MapPartialProcesses(IEnumerable<SimulationPartialProcess> selectedProcesses);

      public void CompoundProcessChanged(TPartialProcessDTO simulationPartialProcessDTO, PartialProcess partialProcess)
      {
         if (partialProcessAlreadySelected(partialProcess))
            throw new CannotSelectThePartialProcessMoreThanOnceException(partialProcess);

         simulationPartialProcessDTO.SimulationPartialProcess.CompoundProcess = partialProcess;
      }

      private bool partialProcessAlreadySelected(PartialProcess partialProcess)
      {
         return isSelected(partialProcess) && _allPartialProcessesDTO.Any(x => Equals(x.CompoundProcess, partialProcess));
      }

      private bool isSelected(PartialProcess partialProcess)
      {
         return !Equals(partialProcess, _notSelectedPartialProcess);
      }

      private void createSystemicProcessesFor(Simulation simulation)
      {
         _allSystemicProcessesDTO = new List<SimulationSystemicProcessSelectionDTO>();
         AddSystemicProcessSelectionFrom(simulation);
      }

      protected bool NoProcessSelectedFor(SimulationSystemicProcessSelectionDTO systemicProcessSelectionDTO)
      {
         return systemicProcessSelectionDTO == null || systemicProcessSelectionDTO.SelectedProcess.IsAnImplementationOf<NullSystemicProcess>();
      }

      public IEnumerable<SystemicProcess> AllSystemicProcessesFor(SimulationSystemicProcessSelectionDTO systemicProcessSelectionDTO)
      {
         var allAvailableProcesses = _compound.AllSystemicProcessesOfType(systemicProcessSelectionDTO.SystemicProcessType).ToList();
         if (allAvailableProcesses.Count == 0)
            allAvailableProcesses.Add(_notAvailableSystemicProcess);
         else
            allAvailableProcesses.Insert(0, _notSelectedSystemicProcess);

         return allAvailableProcesses;
      }

    

      public void SaveConfiguration()
      {
         SaveProcessSelectionGroup(ProcessSelectionGroup());
      }

      protected void SaveProcessSelectionGroup(ProcessSelectionGroup processSelectionGroup)
      {
         processSelectionGroup.ClearProcesses();
         _allPartialProcessesDTO.Each(process => processSelectionGroup.AddPartialProcessSelection(MapPartialProcessFrom(process)));
         _allSystemicProcessesDTO.Each(process => processSelectionGroup.AddSystemicProcessSelection(MapSystemicProcessFrom(process)));
      }

      public IEnumerable<TPartialProcess> AllCompoundProcesses()
      {
         yield return _notSelectedPartialProcess;
         foreach (var process in _allPartialProcesses)
         {
            yield return process;
         }
      }
      public virtual void SelectionChanged(SimulationSystemicProcessSelectionDTO systemicProcessSelectionDTO)
      {
         selectionChanged();
      }

      public virtual void SelectionChanged(TPartialProcessDTO simulationPartialProcessDTO)
      {
         selectionChanged();
      }

      private void selectionChanged()
      {
         ValidateProcessSelection();
         _view.AdjustHeight();
      }
      
      public bool HasProcessesDefined => _allPartialProcessesDTO != null && _allPartialProcessesDTO.Any() ||
                                         _allSystemicProcessesDTO != null && _allSystemicProcessesDTO.Any();

      protected virtual void ValidateProcessSelection()
      {
         //per default, systemic process are valid. Only needs to hide hint view
         View.WarningVisible = false;
      }

      protected abstract void AddSystemicProcessSelectionFrom(Simulation simulation);

      protected abstract ProcessSelectionGroup ProcessSelectionGroup();

      protected void AddDefaultProcessFor(SystemicProcessType systemicProcessType, Simulation simulation)
      {
         var selectedSystemicProcesses = ProcessSelectionGroup().AllSystemicProcesses();
         var allProcesses = _compound.AllSystemicProcessesOfType(systemicProcessType).ToList();
         if (!allProcesses.Any())
            return;

         var simulationSelection = selectedSystemicProcesses.FirstOrDefault(x => x.ProcessType == systemicProcessType);
         var systemicProcess = simulationSelection != null ? allProcesses.FindByName(simulationSelection.ProcessName) ?? _notSelectedSystemicProcess : allProcesses.First();
         var systemicProcessSelectionDTO = new SimulationSystemicProcessSelectionDTO
         {
            SystemicProcessType = systemicProcessType,
            SelectedProcess = systemicProcess
         };
         _allSystemicProcessesDTO.Add(systemicProcessSelectionDTO);
      }

      protected virtual ProcessSelection MapPartialProcessFrom(TPartialProcessDTO partialProcessDTO)
      {
         return new ProcessSelection
         {
            CompoundName = partialProcessDTO.CompoundName,
            ProcessName = partialProcessDTO.CompoundProcessName,
            MoleculeName = partialProcessDTO.IndividualMolecule.Name,
         };
      }

      protected SystemicProcessSelection MapSystemicProcessFrom(SimulationSystemicProcessSelectionDTO systemicProcessDTO)
      {
         return new SystemicProcessSelection
         {
            CompoundName = systemicProcessDTO.CompoundName,
            ProcessType = systemicProcessDTO.SystemicProcessType,
            ProcessName = systemicProcessDTO.SelectedProcess.Name
         };
      }
   }
}