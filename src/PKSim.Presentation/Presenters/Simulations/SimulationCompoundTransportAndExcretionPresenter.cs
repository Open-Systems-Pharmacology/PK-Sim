using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundTransportAndExcretionPresenter : ISimulationCompoundProcessPresenter<TransportPartialProcess, SimulationPartialProcessSelectionDTO>
   {
   }

   public class SimulationCompoundTransportAndExcretionPresenter : SimulationCompoundProcessPresenter<IndividualTransporter, TransportPartialProcess, SimulationPartialProcessSelectionDTO>, ISimulationCompoundTransportAndExcretionPresenter
   {
      public SimulationCompoundTransportAndExcretionPresenter(ISimulationCompoundProcessView<TransportPartialProcess, SimulationPartialProcessSelectionDTO> view, 
         IPartialProcessRetriever partialProcessRetriever)
         : base(view, partialProcessRetriever, PKSimConstants.ObjectTypes.Transporter, PKSimConstants.UI.CompoundTransportProcess)
      {
         View.ApplicationIcon = ApplicationIcons.Transport;
         View.Caption = PKSimConstants.UI.SimulationTransportAndExcretion;
      }

      protected override void AddSystemicProcessSelectionFrom(Simulation simulation)
      {
         AddDefaultProcessFor(SystemicProcessTypes.Renal, simulation);
         AddDefaultProcessFor(SystemicProcessTypes.GFR, simulation);
         AddDefaultProcessFor(SystemicProcessTypes.Biliary, simulation);
      }

      protected override ProcessSelectionGroup ProcessSelectionGroup()
      {
         return _compoundProperties.Processes.TransportAndExcretionSelection;
      }

      protected override IEnumerable<SimulationPartialProcessSelectionDTO> MapPartialProcesses(IEnumerable<SimulationPartialProcess> selectedProcesses)
      {
         return selectedProcesses.Select(x => new SimulationPartialProcessSelectionDTO(x));
      }

      protected override void ValidateProcessSelection()
      {
         var gfr = _allSystemicProcessesDTO.FirstOrDefault(x => x.SystemicProcessType == SystemicProcessTypes.GFR);
         var renal = _allSystemicProcessesDTO.FirstOrDefault(x => x.SystemicProcessType == SystemicProcessTypes.Renal);
         bool isValid;
         //no renal or no gfr=>combination is valid
         if (NoProcessSelectedFor(renal) || NoProcessSelectedFor(gfr))
            isValid = true;
         else 
            //gfr and renal clearnace are defined. combination is valid only if renal process is not a plasma process
            isValid = !string.Equals(renal.SelectedProcess.InternalName, CoreConstants.Process.KIDNEY_CLEARANCE);

         _view.WarningVisible = ! isValid;
         if (isValid) return;
         _view.Warning = PKSimConstants.Warning.RenalAndGFRSelected;
      }
   }
}