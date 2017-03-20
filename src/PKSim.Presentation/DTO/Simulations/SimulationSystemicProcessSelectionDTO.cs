using System.Drawing;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;
using DevExpress.XtraEditors.DXErrorProvider;
using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Assets;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationSystemicProcessSelectionDTO : Notifier, IDXDataErrorInfo
   {
      public SystemicProcessType SystemicProcessType { get; set; }
      private SystemicProcess _selectedProcess;

      /// <summary>
      ///    Status of the selection (Image that will be displayed to the end user indicating if the mapping
      ///    appears to be allowed or not)
      /// </summary>
      public Image Image
      {
         get
         {
            if (_selectedProcess.IsAnImplementationOf<NotSelectedSystemicProcess>())
               return ApplicationIcons.Warning;

            if (_selectedProcess.IsAnImplementationOf<NotAvailableSystemicProcess>())
               return ApplicationIcons.Info;

            return ApplicationIcons.OK;
         }
      }

      public SystemicProcess SelectedProcess
      {
         get { return _selectedProcess; }
         set
         {
            _selectedProcess = value;
            OnPropertyChanged(() => SelectedProcess);
         }
      }

      public string CompoundName
      {
         get
         {
            if (_selectedProcess.ParentCompound == null)
               return string.Empty;

            return _selectedProcess.ParentCompound.Name;
         }
      }

      public void GetPropertyError(string propertyName, ErrorInfo info)
      {
         return;
      }

      public void GetError(ErrorInfo info)
      {
         if (_selectedProcess.IsAnImplementationOf<NotSelectedSystemicProcess>())
         {
            info.ErrorText = PKSimConstants.Warning.SystemicProcessAvailableInCompoundButWasNotSelected(SystemicProcessType.DisplayName);
            info.ErrorType = ErrorType.Warning;
         }
         else if (_selectedProcess.IsAnImplementationOf<NotAvailableSystemicProcess>())
         {
            info.ErrorText = PKSimConstants.Information.NoSystemicProcessDefinedInCompoundForType(SystemicProcessType.DisplayName);
            info.ErrorType = ErrorType.Information;
         }
      }
   }
}