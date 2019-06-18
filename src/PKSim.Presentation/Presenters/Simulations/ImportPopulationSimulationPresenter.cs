using System;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility;

namespace PKSim.Presentation.Presenters.Simulations
{
   public enum PopulationImportMode
   {
      BuildingBlock,
      File,
      Size
   }

   public interface IImportPopulationSimulationPresenter : IDisposablePresenter, IPresenter<IImportPopulationSimulationView>
   {
      /// <summary>
      ///    Starts the import population workflow. If the <paramref name="simulationFilePath" /> is defined, the file will be
      ///    used for the simulation
      ///    and the simulation selection will be hidden.
      /// </summary>
      PopulationSimulation CreateImportPopulationSimulation(string simulationFilePath = null);

      void StartImport();
      void SelectSimulationFile();
      void SelectPopulationFile();
   }

   public class ImportPopulationSimulationPresenter : AbstractDisposablePresenter<IImportPopulationSimulationView, IImportPopulationSimulationPresenter>, IImportPopulationSimulationPresenter
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly IImportSimulationTask _importSimulationTask;
      private readonly ImportPopulationSimulationDTO _importPopulationSimulationDTO;
      private PopulationSimulationImport _populationSimulationImport;

      public ImportPopulationSimulationPresenter(IImportPopulationSimulationView view, IDialogCreator dialogCreator,
         IImportSimulationTask importSimulationTask) : base(view)
      {
         _dialogCreator = dialogCreator;
         _importSimulationTask = importSimulationTask;
         _importPopulationSimulationDTO = new ImportPopulationSimulationDTO {PopulationImportMode = PopulationImportMode.BuildingBlock};
      }

      public PopulationSimulation CreateImportPopulationSimulation(string simulationFilePath = null)
      {
         updateSimulationSelectionFor(simulationFilePath);
         View.BindTo(_importPopulationSimulationDTO);
         View.Display();

         if (View.Canceled || _populationSimulationImport == null)
            return null;

         return _populationSimulationImport.PopulationSimulation;
      }

      private void updateSimulationSelectionFor(string simulationFilePath)
      {
         if (FileHelper.FileExists(simulationFilePath))
         {
            _importPopulationSimulationDTO.FilePath = simulationFilePath;
            _view.SimulationSelectionVisible = false;
         }
         else
            _view.SimulationSelectionVisible = true;
      }

      public void StartImport()
      {
         _populationSimulationImport = importSimulation();
         _importPopulationSimulationDTO.Messages = _populationSimulationImport.Log;
         _importPopulationSimulationDTO.Status = _populationSimulationImport.Status;
      }

      private PopulationSimulationImport importSimulation()
      {
         switch (_importPopulationSimulationDTO.PopulationImportMode)
         {
            case PopulationImportMode.BuildingBlock:
               return _importSimulationTask.ImportFromBuildingBlock(_importPopulationSimulationDTO.FilePath, _importPopulationSimulationDTO.Population);
            case PopulationImportMode.File:
               return _importSimulationTask.ImportFromPopulationFile(_importPopulationSimulationDTO.FilePath, _importPopulationSimulationDTO.PopulationFile);
            case PopulationImportMode.Size:
               return _importSimulationTask.ImportFromPopulationSize(_importPopulationSimulationDTO.FilePath, _importPopulationSimulationDTO.NumberOfIndividuals);
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      public void SelectSimulationFile()
      {
         var pkml = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectSimulationPKMLFile, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART);
         if (string.IsNullOrEmpty(pkml)) return;

         _importPopulationSimulationDTO.FilePath = pkml;
      }

      public void SelectPopulationFile()
      {
         var csv = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectPopulationFileToImport, Constants.Filter.CSV_FILE_FILTER, Constants.DirectoryKey.POPULATION);
         if (string.IsNullOrEmpty(csv)) return;

         _importPopulationSimulationDTO.PopulationFile = csv;
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         udpateButtonState();
      }

      private void udpateButtonState()
      {
         _view.ImportEnabled = !View.HasError;
         _view.OkEnabled = CanClose;
      }

      public override bool CanClose
      {
         get { return base.CanClose && _populationSimulationImport != null; }
      }
   }
}