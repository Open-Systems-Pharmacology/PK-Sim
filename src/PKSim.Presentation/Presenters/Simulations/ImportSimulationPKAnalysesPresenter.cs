using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IImportSimulationPKAnalysesPresenter : IDisposablePresenter
   {
      IReadOnlyCollection<QuantityPKParameter> ImportPKAnalyses(PopulationSimulation populationSimulation);
      void SelectFile();
      Task StartImportProcess();
      string FileName { get; }
   }

   public class ImportSimulationPKAnalysesPresenter : AbstractDisposablePresenter<IImportSimulationPKAnalysesView, IImportSimulationPKAnalysesPresenter>, IImportSimulationPKAnalysesPresenter
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly ISimulationPKParametersImportTask _pkParametersImportTask;
      private ImportPKAnalysesDTO _importDTO;
      private readonly CancellationTokenSource _cancelationTokenSource;
      private IReadOnlyCollection<QuantityPKParameter> _pkAnalyses;
      private PopulationSimulation _populationSimulation;

      public ImportSimulationPKAnalysesPresenter(IImportSimulationPKAnalysesView view, IDialogCreator dialogCreator, ISimulationPKParametersImportTask pkParametersImportTask) : base(view)
      {
         _dialogCreator = dialogCreator;
         _pkParametersImportTask = pkParametersImportTask;
         _cancelationTokenSource = new CancellationTokenSource();
         _pkAnalyses = new List<QuantityPKParameter>();
      }

      public IReadOnlyCollection<QuantityPKParameter> ImportPKAnalyses(PopulationSimulation populationSimulation)
      {
         _populationSimulation = populationSimulation;
         _importDTO = new ImportPKAnalysesDTO();
         _view.BindTo(_importDTO);
         udpateButtonState();
         _view.Display();

         return _view.Canceled ? new List<QuantityPKParameter>() : _pkAnalyses;
      }

      public void SelectFile()
      {
         var file = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectPKAnalysesFileToImport, Constants.Filter.CSV_FILE_FILTER, Constants.DirectoryKey.POPULATION);
         if (string.IsNullOrEmpty(file))
            return;

         _importDTO.FilePath = file;
         udpateButtonState();
      }

      public override bool CanClose
      {
         get { return base.CanClose && _pkAnalyses.Count > 0 && !_importDTO.Status.Is(NotificationType.Error); }
      }

      public async Task StartImportProcess()
      {
         try
         {
            _view.ImportingResults = true;
            var pkAnalysesImport = await _pkParametersImportTask.ImportPKParameters(_populationSimulation, _importDTO.FilePath, _cancelationTokenSource.Token);
            updateFromImportedPKAnalyses(pkAnalysesImport);
         }
         catch (OperationCanceledException)
         {
            /*canceled*/
         }
         finally
         {
            _view.ImportingResults = false;
            udpateButtonState();
         }
      }

      public string FileName
      {
         get { return _importDTO.FilePath; }
      }

      private void udpateButtonState()
      {
         _view.OkEnabled = CanClose;
      }

      private void updateFromImportedPKAnalyses(SimulationPKParametersImport importResult)
      {
         _pkAnalyses = importResult.PKParameters.ToList();
         _importDTO.Status = importResult.Status;
         _importDTO.Messages = importResult.Log;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         udpateButtonState();
      }
   }
}