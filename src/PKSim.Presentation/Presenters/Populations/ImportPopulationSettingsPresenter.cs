using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Populations;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IImportPopulationSettingsPresenter : IPopulationSettingsPresenter<ImportPopulation>
   {
      void AddFile();
      void RemoveFile(PopulationFileSelectionDTO fileSelectionDTO);
      void ChangePath(PopulationFileSelectionDTO fileSelectionDTO);
   }

   public class ImportPopulationSettingsPresenter : AbstractSubPresenter<IImportPopulationSettingsView, IImportPopulationSettingsPresenter>, IImportPopulationSettingsPresenter
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly IImportPopulationFactory _importPopulationFactory;
      private readonly ImportPopulationSettingsDTO _importPopulationSettingsDTO;
      private CancellationTokenSource _cancellationTokenSource;
      public bool IsLatched { get; set; }
      public bool PopulationCreated { get; private set; }
      public event EventHandler<PopulationCreationEventArgs> PopulationCreationFinished = delegate { };

      public ImportPopulation Population { get; private set; }

      public ImportPopulationSettingsPresenter(IImportPopulationSettingsView view, ILazyLoadTask lazyLoadTask, IDialogCreator dialogCreator, IImportPopulationFactory importPopulationFactory)
         : base(view)
      {
         _lazyLoadTask = lazyLoadTask;
         _dialogCreator = dialogCreator;
         _importPopulationFactory = importPopulationFactory;
         _importPopulationSettingsDTO = new ImportPopulationSettingsDTO();
      }

      public async Task CreatePopulation()
      {
         _cancellationTokenSource = new CancellationTokenSource();
         try
         {
            _view.CreatingPopulation = true;
            Population = await _importPopulationFactory.CreateFor(filesToImport, _importPopulationSettingsDTO.Individual, _cancellationTokenSource.Token);
            PopulationCreated = Population.ImportSuccessful;
            this.DoWithinLatch(updateViewForPopulation);
            raisePopulationCreationFinish(success: Population.ImportSuccessful, hasWarningOrError: Population.ImportHasWarningOrError);
         }
         catch (Exception e)
         {
            raisePopulationCreationFinish(success: false, hasWarningOrError: false);
            if (!(e is OperationCanceledException))
               throw;
         }
         finally
         {
            _view.CreatingPopulation = false;
         }
      }

      private void raisePopulationCreationFinish(bool success, bool hasWarningOrError)
      {
         PopulationCreationFinished(this, new PopulationCreationEventArgs(success, hasWarningOrError));
      }

      private void updateViewForPopulation()
      {
         _importPopulationSettingsDTO.PopulationFiles.Clear();
         Population.Settings.AllFiles.Each(f => _importPopulationSettingsDTO.AddFile(PopulationFileSelectionDTO.From(f)));
         updateView();
      }

      private IReadOnlyCollection<string> filesToImport
      {
         get { return _importPopulationSettingsDTO.PopulationFiles.Select(x => x.FilePath).ToList(); }
      }

      public void LoadPopulation(ImportPopulation population)
      {
         _view.UpdateLayoutForEditing();
         Population = population;
         //do in latch since no update should be perform
         this.DoWithinLatch(updateViewForPopulation);
         PopulationCreated = true;
      }

      public void Cancel()
      {
         _cancellationTokenSource?.Cancel();
      }

      public override bool CanClose => base.CanClose && _importPopulationSettingsDTO.IsValid();

      public void AddFile()
      {
         var newFile = getNewFile();
         if (string.IsNullOrEmpty(newFile)) return;
         _importPopulationSettingsDTO.AddFile(newFile);
      }

      private string getNewFile()
      {
         return _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectPopulationFileToImport, CoreConstants.Filter.POPULATION_FILE_FILTER, Constants.DirectoryKey.POPULATION);
      }

      public void RemoveFile(PopulationFileSelectionDTO fileSelectionDTO)
      {
         _importPopulationSettingsDTO.RemoveFile(fileSelectionDTO);
      }

      public void ChangePath(PopulationFileSelectionDTO fileSelectionDTO)
      {
         var newFile = getNewFile();
         if (string.IsNullOrEmpty(newFile)) return;
         fileSelectionDTO.FilePath = newFile;
      }

      public void PrepareForCreating(Individual basedIndividual)
      {
         _lazyLoadTask.Load(basedIndividual);
         _importPopulationSettingsDTO.Individual = basedIndividual;
         this.DoWithinLatch(updateView);
         ViewChanged();
      }

      public void IndividualSelectionChanged(Individual newIndividual)
      {
         if (IsLatched) return;
         PrepareForCreating(newIndividual);
      }

      private void updateView()
      {
         _view.BindTo(_importPopulationSettingsDTO);
      }

      public override void ViewChanged()
      {
         if (IsLatched) return;
         PopulationCreated = false;
         OnStatusChanged();
      }
   }
}