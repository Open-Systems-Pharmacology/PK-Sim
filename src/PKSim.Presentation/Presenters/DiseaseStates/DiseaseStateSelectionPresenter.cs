﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Views.DiseaseStates;

namespace PKSim.Presentation.Presenters.DiseaseStates
{
   public interface IDiseaseStateSelectionPresenter : IPresenter<IDiseaseStateSelectionView>
   {
      void DiseaseStateChanged();
      void ResetDiseaseState();
      IReadOnlyList<DiseaseState> AllDiseaseStates { get; set; }
      void Edit(DiseaseStateDTO diseaseStateDTO);
      void Refresh();
   }

   public class DiseaseStateSelectionPresenter : AbstractPresenter<IDiseaseStateSelectionView, IDiseaseStateSelectionPresenter>, IDiseaseStateSelectionPresenter
   {
      private readonly IDiseaseStateRepository _diseaseStateRepository;
      private readonly IDiseaseStateUpdater _diseaseStateUpdater;
      private DiseaseStateDTO _diseaseStateDTO;

      public DiseaseStateSelectionPresenter(
         IDiseaseStateSelectionView view,
         IDiseaseStateRepository diseaseStateRepository,
         IDiseaseStateUpdater diseaseStateUpdater) : base(view)
      {
         _diseaseStateRepository = diseaseStateRepository;
         _diseaseStateUpdater = diseaseStateUpdater;
      }

      public void DiseaseStateChanged()
      {
         refreshDiseaseState();
         Refresh();
      }

      public IReadOnlyList<DiseaseState> AllDiseaseStates { get; set; }

      public void Edit(DiseaseStateDTO diseaseStateDTO)
      {
         _diseaseStateDTO = diseaseStateDTO;
      }

      private void refreshDiseaseState() => _diseaseStateUpdater.UpdateDiseaseStateParameters(_diseaseStateDTO);

      public void Refresh()
      {
         _view.BindTo(_diseaseStateDTO);
      }

      public void ResetDiseaseState()
      {
         //after species change, we are always in healthy state
         _diseaseStateDTO.Value = _diseaseStateRepository.HealthyState;
         refreshDiseaseState();
      }
   }
}