﻿using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation.Presenters.Observers
{
   public interface IImportObserversPresenter : IObserverItemPresenter
   {
      IEnumerable<IObserverBuilder> Observers { get; }
      void RemoveObserver(ImportObserverDTO observer);
      void AddObserver();
      void SelectObserver(ImportObserverDTO observer);
   }

   public class ImportObserversPresenter : AbstractSubPresenter<IImportObserversView, IImportObserversPresenter>, IImportObserversPresenter
   {
      private readonly IObserverBuildingBlockTask _observerBuildingBlockTask;
      private readonly IObserverInfoPresenter _observerInfoPresenter;
      private readonly IDialogCreator _dialogCreator;
      private readonly List<ImportObserverDTO> _observerDTOs = new List<ImportObserverDTO>();

      public ImportObserversPresenter(
         IImportObserversView view,
         IObserverBuildingBlockTask observerBuildingBlockTask,
         IObserverInfoPresenter observerInfoPresenter,
         IDialogCreator dialogCreator) : base(view)
      {
         _observerBuildingBlockTask = observerBuildingBlockTask;
         _observerInfoPresenter = observerInfoPresenter;
         _dialogCreator = dialogCreator;
         AddSubPresenters(_observerInfoPresenter);
         _view.AddObserverView(_observerInfoPresenter.View);
         _view.BindTo(_observerDTOs);
      }

      public IEnumerable<IObserverBuilder> Observers { get; } = new List<IObserverBuilder>();

      public void RemoveObserver(ImportObserverDTO observerDTO)
      {
         _observerDTOs.Remove(observerDTO);
         updateView();
      }

      public void AddObserver()
      {
         var newFile = getNewFile();
         if (string.IsNullOrEmpty(newFile))
            return;

         var observer = _observerBuildingBlockTask.LoadObserverFrom(newFile);
         if (observer == null)
            return;

         if (_observerDTOs.ExistsByName(observer.Name))
            throw new OSPSuiteException(PKSimConstants.Error.NameAlreadyExistsInContainerType(observer.Name, ObjectTypes.ObserverBuildingBlock));

         var observerDTO = new ImportObserverDTO(observer) {FilePath = newFile};
         _observerDTOs.Add(observerDTO);
         updateView(observerDTO);
      }

      public void SelectObserver(ImportObserverDTO observer)
      {
         _observerInfoPresenter.Edit(observer?.Observer);
      }

      private void updateView(ImportObserverDTO observerToSelect = null)
      {
         _view.Rebind();
         SelectObserver(observerToSelect);
      }

      private string getNewFile()
      {
         return _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectObserverFileToImport, Constants.Filter.PKML_FILE_FILTER, Constants.DirectoryKey.MODEL_PART);
      }
   }
}