using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundSpecificBindingPresenter : ISimulationCompoundProcessPresenter<SpecificBindingPartialProcess, SimulationPartialProcessSelectionDTO>
   {
   }

   public class SimulationCompoundSpecificBindingPresenter : SimulationCompoundProcessPresenter<IndividualMolecule, SpecificBindingPartialProcess, SimulationPartialProcessSelectionDTO>, ISimulationCompoundSpecificBindingPresenter
   {
      public SimulationCompoundSpecificBindingPresenter(ISimulationCompoundProcessView<SpecificBindingPartialProcess, SimulationPartialProcessSelectionDTO> view,
         IPartialProcessRetriever partialProcessRetriever)
         : base(view, partialProcessRetriever, PKSimConstants.ObjectTypes.Protein, PKSimConstants.UI.CompoundBindingProcess)
      {
         View.ApplicationIcon = ApplicationIcons.SpecificBinding;
         View.Caption = PKSimConstants.UI.SimulationSpecificBinding;
      }

      protected override IEnumerable<SimulationPartialProcessSelectionDTO> MapPartialProcesses(IEnumerable<SimulationPartialProcess> selectedProcesses)
      {
         return selectedProcesses.Select(x => new SimulationPartialProcessSelectionDTO(x));
      }

      protected override void AddSystemicProcessSelectionFrom(Simulation simulation)
      {
         //no systemic processes for specific bindiing so far
      }

      protected override ProcessSelectionGroup ProcessSelectionGroup()
      {
         return _compoundProperties.Processes.SpecificBindingSelection;
      }
   }
}