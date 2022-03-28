using System;
using System.Drawing;
using DevExpress.XtraEditors.DXErrorProvider;
using OSPSuite.Assets;
using OSPSuite.Utility.Reflection;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationPartialProcessSelectionDTO : Notifier, IWithImageDTO
   {
      public SimulationPartialProcess SimulationPartialProcess { get; }

      public SimulationPartialProcessSelectionDTO(SimulationPartialProcess simulationPartialProcess)
      {
         SimulationPartialProcess = simulationPartialProcess;
      }

      /// <summary>
      ///    Selected process in compound
      /// </summary>
      public PartialProcess CompoundProcess => SimulationPartialProcess.CompoundProcess;

      public string CompoundProcessName => Status == SimulationPartialProcessStatus.ProcessNotSelected ? string.Empty : CompoundProcess.Name;

      /// <summary>
      ///    Enzyme used in the individual to be mapped to the selected process in compound
      /// </summary>
      public IndividualMolecule IndividualMolecule => SimulationPartialProcess.IndividualMolecule;

      public SimulationPartialProcessStatus Status => SimulationPartialProcess.Status;

      public string CompoundName => SimulationPartialProcess.CompoundName;

      /// <summary>
      ///    Status of the selection (Image that will be displayed to the end user indicating if the mapping
      ///    appears to be allowed or not)
      /// </summary>
      public Image Image => imageFrom(Status).ToImage();

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

      private ApplicationIcon imageFrom(SimulationPartialProcessStatus status)
      {
         switch (status)
         {
            case SimulationPartialProcessStatus.ProcessNotSelected:
               return ApplicationIcons.Warning;
            case SimulationPartialProcessStatus.CanBeUsedInSimulation:
               return ApplicationIcons.OK;
            default:
               throw new ArgumentOutOfRangeException(nameof(status));
         }
      }
   }
}