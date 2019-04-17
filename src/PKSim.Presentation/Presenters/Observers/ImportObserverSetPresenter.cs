using System;
using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IImportObserverSetPresenter : IObserverSetItemPresenter
   {
      IEnumerable<IObserverBuilder> Observers { get; }
      void RemoveObserver(ImportObserverDTO observer);
      void AddObserver();
      void SelectObserver(ImportObserverDTO observer);
      bool ShowFilePath { get; set; }
   }

   public class ImportObserverSetPresenter : AbstractSubPresenter<IImportObserverSetView, IImportObserverSetPresenter>, IImportObserverSetPresenter, ILatchable
   {
      private readonly IObserverInfoPresenter _observerInfoPresenter;
      private readonly IDialogCreator _dialogCreator;
      private readonly IObserverTask _observerTask;
      private readonly IEntityTask _entityTask;
      private readonly List<ImportObserverDTO> _observerDTOs = new List<ImportObserverDTO>();
      private ObserverSet _observerSet;
      public bool IsLatched { get; set; }

      public ImportObserverSetPresenter(
         IImportObserverSetView view,
         IObserverInfoPresenter observerInfoPresenter,
         IDialogCreator dialogCreator,
         IObserverTask observerTask,
         IEntityTask entityTask) : base(view)
      {
         _observerInfoPresenter = observerInfoPresenter;
         _dialogCreator = dialogCreator;
         _observerTask = observerTask;
         _entityTask = entityTask;
         AddSubPresenters(_observerInfoPresenter);
         _view.AddObserverView(_observerInfoPresenter.View);
         _view.BindTo(_observerDTOs);
      }

      public IEnumerable<IObserverBuilder> Observers { get; } = new List<IObserverBuilder>();

      public void Edit(ObserverSet observerSet)
      {
         //This is to prevent edit triggered by action happening in this presenter
         if (IsLatched)
            return;

         _observerSet = observerSet;
         _observerDTOs.Clear();
         _observerSet.Observers.Each(x => addObserver(x));
         _view.Rebind();
      }

      public void RemoveObserver(ImportObserverDTO observerDTO)
      {
         AddCommand(() => _observerTask.RemoveObserver(observerDTO.Observer, _observerSet));
         _observerDTOs.Remove(observerDTO);
         updateView();
      }

      public void AddObserver()
      {
         var newFile = getNewFile();
         if (string.IsNullOrEmpty(newFile))
            return;

         var observer = _observerTask.LoadObserverFrom(newFile);
         if (observer == null)
            return;

         if (_observerDTOs.ExistsByName(observer.Name))
         {
            var newName = _entityTask.NewNameFor(observer, _observerDTOs.AllNames());
            if (string.IsNullOrEmpty(newName))
               return;

            observer.Name = newName;
         }

         var observerDTO = addObserver(observer, newFile);
         AddCommand(() => _observerTask.AddObserver(observer, _observerSet));
         updateView(observerDTO);
      }

      public void SelectObserver(ImportObserverDTO observer)
      {
         _observerInfoPresenter.Edit(observer?.Observer);
      }

      public override void AddCommand(Func<ICommand> commandFunc)
      {
         this.DoWithinLatch(() => base.AddCommand(commandFunc));
      }

      public bool ShowFilePath
      {
         get => _view.ShowFilePath;
         set => _view.ShowFilePath = value;
      }

      private void updateView(ImportObserverDTO observerToSelect = null)
      {
         _view.Rebind();
         _view.SelectObserver(observerToSelect);
      }

      private string getNewFile()
      {
         return _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectObserverFileToImport, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART);
      }

      private ImportObserverDTO addObserver(IObserverBuilder observer, string filePath = null)
      {
         var observedDTO = map(observer, filePath);
         _observerDTOs.Add(observedDTO);
         return observedDTO;
      }

      private ImportObserverDTO map(IObserverBuilder observer, string filePath = null) => new ImportObserverDTO(observer) {FilePath = filePath};
   }
}