using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Presentation.Presenters;
using ISimulationResultsImportTask = PKSim.Core.Services.ISimulationResultsImportTask;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IImportSimulationResultsPresenter : IDisposablePresenter
   {
      SimulationResults ImportResultsFor(PopulationSimulation populationSimulation);
      Task StartImportProcess();
      void BrowseForFolder();
      void RemoveFile(SimulationResultsFileSelectionDTO simulationResultsFile);
      void ChangePath(SimulationResultsFileSelectionDTO simulationResultsFile);
      void AddFile();
   }

   public class ImportSimulationResultsPresenter : AbstractDisposablePresenter<IImportSimulationResultsView, IImportSimulationResultsPresenter>, IImportSimulationResultsPresenter, ILatchable
   {
      private readonly ISimulationResultsImportTask _simulationResultsImportTask;
      private readonly IDialogCreator _dialogCreator;
      private PopulationSimulation _populationSimulation;
      private SimulationResults _simulationResults = new NullSimulationResults();
      private CancellationTokenSource _cancelationTokenSource;
      private readonly ImportSimulationResultsDTO _importSimulationResultsDTO;
      public bool IsLatched { get; set; }

      public ImportSimulationResultsPresenter(IImportSimulationResultsView view, ISimulationResultsImportTask simulationResultsImportTask, IDialogCreator dialogCreator) : base(view)
      {
         _simulationResultsImportTask = simulationResultsImportTask;
         _dialogCreator = dialogCreator;
         _importSimulationResultsDTO = new ImportSimulationResultsDTO();
      }

      public SimulationResults ImportResultsFor(PopulationSimulation populationSimulation)
      {
         _populationSimulation = populationSimulation;
         this.DoWithinLatch(rebind);
         ViewChanged();
         _view.Display();

         return _view.Canceled ? new NullSimulationResults() : _simulationResults;
      }

      private void rebind()
      {
         _view.BindTo(_importSimulationResultsDTO);
         updateButtonState();
      }

      private void updateButtonState()
      {
         View.OkEnabled = CanClose;
         View.ImportEnabled = _importSimulationResultsDTO.SimulationResultsFile.Any();
      }

      public override bool CanClose
      {
         get { return base.CanClose && !_simulationResults.IsNull() && !_importSimulationResultsDTO.Status.Is(NotificationType.Error); }
      }

      public async Task StartImportProcess()
      {
         _cancelationTokenSource = new CancellationTokenSource();

         try
         {
            _view.ImportingResults = true;
            _simulationResults = new NullSimulationResults();
            var importResults = await _simulationResultsImportTask.ImportResults(_populationSimulation, filesToImport, _cancelationTokenSource.Token);
            this.DoWithinLatch(() => updateFromImportedResults(importResults));
         }
         catch (OperationCanceledException)
         {
            /*canceled*/
         }
         finally
         {
            _view.ImportingResults = false;
         }
      }

      private void updateFromImportedResults(SimulationResultsImport importResults)
      {
         _simulationResults = importResults.SimulationResults;
         _importSimulationResultsDTO.Clear();
         importResults.SimulationResultsFiles.Each(f => _importSimulationResultsDTO.AddFile(SimulationResultsFileSelectionDTO.From(f)));
         _importSimulationResultsDTO.Status = importResults.Status;
         _importSimulationResultsDTO.Messages = importResults.Log;
         rebind();
      }

      private IReadOnlyCollection<string> filesToImport
      {
         get { return _importSimulationResultsDTO.SimulationResultsFile.Select(x => x.FilePath).ToList(); }
      }

      public void BrowseForFolder()
      {
         var folder = _dialogCreator.AskForFolder(PKSimConstants.UI.SelectFolderContainingSimulationResults, Constants.DirectoryKey.POPULATION);
         if (string.IsNullOrEmpty(folder))
            return;

         _importSimulationResultsDTO.ImportFolder = folder;
         var resultsDirectory = new DirectoryInfo(folder);
         if (!resultsDirectory.Exists)
            return;

         _importSimulationResultsDTO.Clear();
         resultsDirectory.GetFiles(CoreConstants.Filter.SIMULATION_RESULTS_FILTER).Each(f => addFile(f.FullName));
      }

      public void RemoveFile(SimulationResultsFileSelectionDTO fileSelectionDTO)
      {
         _importSimulationResultsDTO.RemoveFile(fileSelectionDTO);
      }

      public void ChangePath(SimulationResultsFileSelectionDTO fileSelectionDTO)
      {
         string newFile = getNewFile();
         if (string.IsNullOrEmpty(newFile)) return;
         fileSelectionDTO.FilePath = newFile;
      }

      public void AddFile()
      {
         addFile(getNewFile());
      }

      private void addFile(string fileToAdd)
      {
         if (string.IsNullOrEmpty(fileToAdd)) return;
         _importSimulationResultsDTO.AddFile(fileToAdd);
      }

      private string getNewFile()
      {
         return _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectPopulationFileToImport, Constants.Filter.CSV_FILE_FILTER, Constants.DirectoryKey.POPULATION);
      }

      public override void ViewChanged()
      {
         if (IsLatched) return;
         _simulationResults = new NullSimulationResults();
         updateButtonState();
         base.ViewChanged();
      }
   }
}