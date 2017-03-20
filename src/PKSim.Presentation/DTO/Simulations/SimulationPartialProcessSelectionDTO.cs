using System;
using System.Drawing;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using DevExpress.XtraEditors.DXErrorProvider;
using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Assets;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationPartialProcessSelectionDTO : Notifier, IDXDataErrorInfo 
   {
      private readonly SimulationPartialProcess _simulationPartialProcess;

      public SimulationPartialProcess SimulationPartialProcess => _simulationPartialProcess;

      public SimulationPartialProcessSelectionDTO(SimulationPartialProcess simulationPartialProcess)
      {
         _simulationPartialProcess = simulationPartialProcess;
      }

      /// <summary>
      ///    Selected process in compound
      /// </summary>
      public PartialProcess  CompoundProcess => _simulationPartialProcess.CompoundProcess;

      public string CompoundProcessName => Status == SimulationPartialProcessStatus.ProcessNotSelected ? string.Empty : CompoundProcess.Name;

      /// <summary>
      ///    Enyyme used in the individual to be mapped to the selected process in compound
      /// </summary>
      public IndividualMolecule IndividualMolecule => _simulationPartialProcess.IndividualMolecule;

      public SimulationPartialProcessStatus Status => _simulationPartialProcess.Status;

      public string CompoundName => _simulationPartialProcess.CompoundName;

      /// <summary>
      ///    Status of the selection (Image that will be displayed to the end user indicating if the mapping
      ///    appears to be allowed or not)
      /// </summary>
      public Image Image => imageFrom(Status);

      public void GetPropertyError(string propertyName, ErrorInfo info)
      {
         return;
      }

      public void GetError(ErrorInfo info)
      {
         switch (Status)
         {
            case SimulationPartialProcessStatus.ProcessNotSelected:
               info.ErrorText = PKSimConstants.Warning.ProteinAvailableButProcessNotSelected(IndividualMolecule.Name);
               info.ErrorType = ErrorType.Warning;
               break;
            default:
               break;
         }
      }

      private Image imageFrom(SimulationPartialProcessStatus status)
      {
         switch (status)
         {
            case SimulationPartialProcessStatus.ProcessNotSelected:
               return ApplicationIcons.Warning;
            case SimulationPartialProcessStatus.CanBeUsedInSimulation:
               return ApplicationIcons.OK;
            default:
               throw new ArgumentOutOfRangeException("status");
         }
      }
   }
}