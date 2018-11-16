using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundEnzymaticProcessPresenter : ISimulationCompoundProcessPresenter<EnzymaticProcess, SimulationEnzymaticProcessSelectionDTO>
   {
      IEnumerable<string> AllMetabolitesFor(SimulationEnzymaticProcessSelectionDTO dto);
      void AddPartialProcessMappingBaseOn(SimulationEnzymaticProcessSelectionDTO dto);
      void DeletePartialProcessMapping(SimulationEnzymaticProcessSelectionDTO dto);
      bool CanDeletePartialProcess(SimulationEnzymaticProcessSelectionDTO dto);
   }

   public class SimulationCompoundEnzymaticProcessPresenter : SimulationCompoundProcessPresenter<IndividualEnzyme, EnzymaticProcess, SimulationEnzymaticProcessSelectionDTO>, ISimulationCompoundEnzymaticProcessPresenter
   {
      public SimulationCompoundEnzymaticProcessPresenter(ISimulationCompoundEnzymaticProcessView view, IPartialProcessRetriever partialProcessRetriever)
         : base(view, partialProcessRetriever, PKSimConstants.ObjectTypes.Enzyme, PKSimConstants.UI.CompoundEnzymaticProcess)
      {
         View.ApplicationIcon = ApplicationIcons.Metabolism;
         View.Caption = PKSimConstants.UI.SimulationMetabolism;
      }

      protected override void AddSystemicProcessSelectionFrom(Simulation simulation)
      {
         AddDefaultProcessFor(SystemicProcessTypes.Hepatic, simulation);
      }

      protected override ProcessSelectionGroup ProcessSelectionGroup()
      {
         return _compoundProperties.Processes.MetabolizationSelection;
      }

      public override void EditProcessesIn(Simulation simulation, CompoundProperties compoundProperties)
      {
         base.EditProcessesIn(simulation, compoundProperties);
         if (simulation.Compounds.Count > 1)
            return;

         enzymaticProcessView.HideMultipleCompoundColumns();
      }

      protected override IEnumerable<SimulationEnzymaticProcessSelectionDTO> MapPartialProcesses(IEnumerable<SimulationPartialProcess> selectedProcesses)
      {
         return selectedProcesses.Select(mapFrom);
      }

      private SimulationEnzymaticProcessSelectionDTO mapFrom(SimulationPartialProcess simulationPartialProcess)
      {
         var dto = new SimulationEnzymaticProcessSelectionDTO(simulationPartialProcess);
         updateMetabliteSelectionFor(dto);
         return dto;
      }

      public override void SelectionChanged(SimulationEnzymaticProcessSelectionDTO simulationPartialProcessDTO)
      {
         base.SelectionChanged(simulationPartialProcessDTO);
         updateMetabliteSelectionFor(simulationPartialProcessDTO);
      }

      private void updateMetabliteSelectionFor(SimulationEnzymaticProcessSelectionDTO simulationPartialProcessDTO)
      {
         var simulationPartialProcess = simulationPartialProcessDTO.SimulationPartialProcess;
         var partialProcessMapping = simulationPartialProcess.PartialProcessMapping as EnzymaticProcessSelection;
         var metaboliteName = simulationPartialProcess.CompoundProcess.DowncastTo<EnzymaticProcess>().MetaboliteName;

         if (partialProcessMapping != null)
            metaboliteName = partialProcessMapping.MetaboliteName;

         var allPossibleMetabolites = AllMetabolitesFor(simulationPartialProcessDTO).ToList();
         simulationPartialProcessDTO.MetaboliteName = allPossibleMetabolites.Contains(metaboliteName) ? metaboliteName : allPossibleMetabolites.FirstOrDefault();
      }

      protected override void ValidateProcessSelection()
      {
         var hepatic = _allSystemicProcessesDTO.FirstOrDefault(x => x.SystemicProcessType == SystemicProcessTypes.Hepatic);
         var hasSpecific = _allPartialProcessesDTO.Any(x => x.Status == SimulationPartialProcessStatus.CanBeUsedInSimulation);
         bool isValid = (NoProcessSelectedFor(hepatic) || !hasSpecific);
         _view.WarningVisible = !isValid;
         if (isValid) return;
         _view.Warning = PKSimConstants.Warning.HepaticAndSpecific;
      }

      public IEnumerable<string> AllMetabolitesFor(SimulationEnzymaticProcessSelectionDTO dto)
      {
         yield return PKSimConstants.UI.Sink;
         if (Equals(dto.CompoundProcess, _notSelectedPartialProcess))
            yield break;

         foreach (var compound in _simulation.Compounds.Where(c => !Equals(c, _compound)))
         {
            yield return compound.Name;
         }
      }

      public void AddPartialProcessMappingBaseOn(SimulationEnzymaticProcessSelectionDTO processSelectionDTO)
      {
         var currentSimulationPartialProcess = processSelectionDTO.SimulationPartialProcess;
         var newProcess = new SimulationPartialProcess
         {
            CompoundProcess = _notSelectedPartialProcess,
            IndividualMolecule = currentSimulationPartialProcess.IndividualMolecule,
         };

         var newProcessDTO = mapFrom(newProcess);
         var insertIndex = _allPartialProcessesDTO.IndexOf(processSelectionDTO);
         _allPartialProcessesDTO.Insert(insertIndex + 1, newProcessDTO);

         rebindPartialProcess();
      }

      public void DeletePartialProcessMapping(SimulationEnzymaticProcessSelectionDTO dto)
      {
         _allPartialProcessesDTO.Remove(dto);
         rebindPartialProcess();
      }

      public bool CanDeletePartialProcess(SimulationEnzymaticProcessSelectionDTO dto)
      {
         return _allPartialProcessesDTO.Count(x => x.IndividualMolecule == dto.IndividualMolecule) > 1;
      }

      private void rebindPartialProcess()
      {
         _view.BindToPartialProcesses(_allPartialProcessesDTO);
         _view.AdjustHeight();
      }

      protected override ProcessSelection MapPartialProcessFrom(SimulationEnzymaticProcessSelectionDTO simulationEnzymaticProcessDTO)
      {
         var metaboliteName = simulationEnzymaticProcessDTO.MetaboliteName;
         if (string.Equals(metaboliteName, PKSimConstants.UI.Sink))
            metaboliteName = string.Empty;

         return new EnzymaticProcessSelection
         {
            CompoundName = simulationEnzymaticProcessDTO.CompoundName,
            ProcessName = simulationEnzymaticProcessDTO.CompoundProcessName,
            MoleculeName = simulationEnzymaticProcessDTO.IndividualMolecule.Name,
            MetaboliteName = metaboliteName
         };
      }

      private ISimulationCompoundEnzymaticProcessView enzymaticProcessView => View.DowncastTo<ISimulationCompoundEnzymaticProcessView>();
   }
}