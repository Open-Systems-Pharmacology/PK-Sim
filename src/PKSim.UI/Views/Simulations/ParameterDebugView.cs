using System.Data;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class ParameterDebugView : BaseModalView, IParameterDebugView
   {
      private readonly IDialogCreator _dialogCreator;

      public ParameterDebugView(IDialogCreator dialogCreator, Shell shell): base(shell)
      {
         _dialogCreator = dialogCreator;
         InitializeComponent();
      }

      public void AttachPresenter(IParameterDebugPresenter presenter)
      {
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         btnExport.Click +=(o,e)=> OnEvent(exportTable);
      }

      private void exportTable()
      {
         var file = _dialogCreator.AskForFileToSave(PKSimConstants.UI.ExportLogToFile, Constants.Filter.CSV_FILE_FILTER, Constants.DirectoryKey.REPORT);
         if (string.IsNullOrEmpty(file)) return;

         gridParameterId.DataSource.DowncastTo<DataTable>().ExportToCSV(file);
      }

      public void BindTo(DataTable dataTable)
      {
         gridParameterId.DataSource = dataTable;
      }

      public void AttachPresenter(IParameterValuesDebugPresenter presenter)
      {
      }
   }
}