using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{

   public interface ISimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO> : IView<ISimulationCompoundProcessPresenter<TPartialProcess, TPartialProcessDTO>>, IResizableView 
      where TPartialProcess : PartialProcess
      where TPartialProcessDTO : SimulationPartialProcessSelectionDTO
   {
      void BindToPartialProcesses(IReadOnlyCollection<TPartialProcessDTO> allPartialProcessSelectionDTO);
      void BindToSystemicProcesses(IReadOnlyCollection<SimulationSystemicProcessSelectionDTO> allSystemicProcessSelectionDTO);
      string MoleculeName { set; }
      string CompoundProcessCaption { set; }
      bool WarningVisible { get; set; }
      string Warning { get; set; }
      string Info { get; set; }
   }
}