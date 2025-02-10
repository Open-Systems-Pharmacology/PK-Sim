using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.DiseaseStates;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualSettingsPresenter : IIndividualItemPresenter
   {
      IEnumerable<Species> AllSpecies();
      IReadOnlyList<SpeciesPopulation> PopulationsFor(Species species);
      IReadOnlyList<Gender> GenderFor(SpeciesPopulation population);
      IEnumerable<ParameterValueVersion> AllParameterValueVersionsFor(string category);
      IEnumerable<CalculationMethod> AllCalculationMethodsFor(string category);

      void PrepareForCreating();
      void PrepareForScaling(Individual individualToScale);
      void PopulationChanged();
      void RetrieveMeanValues();
      void SpeciesChanged();
      void GenderChanged();
      bool ShouldDisplayPvvCategory(string category);
      bool ShouldDisplayCmCategory(string category);
      void CreateIndividual();
      Individual Individual { get; }
      bool IndividualCreated { get; }
      bool SpeciesVisible { set; }
      void AgeChanged();
      void GestationalAgeChanged();
   }

   public class IndividualSettingsPresenter : AbstractSubPresenter<IIndividualSettingsView, IIndividualSettingsPresenter>, IIndividualSettingsPresenter
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly IIndividualDefaultValueUpdater _defaultValueUpdater;
      private readonly IIndividualToIIndividualSettingsDTOMapper _individualSettingsDTOMapper;
      private readonly IIndividualSettingsDTOToIndividualMapper _individualMapper;
      private readonly IEditValueOriginPresenter _editValueOriginPresenter;
      private readonly IDiseaseStateSelectionPresenter _diseaseStateSelectionPresenter;
      private readonly IDiseaseStateRepository _diseaseStateRepository;
      private readonly ICloner _cloner;

      public bool IndividualCreated { get; private set; }

      private bool _isUpdating;
      private IndividualSettingsDTO _individualSettingsDTO;

      public IndividualSettingsPresenter(
         IIndividualSettingsView view,
         ISpeciesRepository speciesRepository,
         ICalculationMethodCategoryRepository calculationMethodCategoryRepository,
         IDefaultIndividualRetriever defaultIndividualRetriever,
         IIndividualDefaultValueUpdater defaultValueUpdater,
         IIndividualToIIndividualSettingsDTOMapper individualSettingsDTOMapper,
         IIndividualSettingsDTOToIndividualMapper individualMapper,
         IEditValueOriginPresenter editValueOriginPresenter,
         IDiseaseStateSelectionPresenter diseaseStateSelectionPresenter,
         IDiseaseStateRepository diseaseStateRepository, 
         ICloner cloner) : base(view)
      {
         _speciesRepository = speciesRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _defaultValueUpdater = defaultValueUpdater;
         _individualSettingsDTOMapper = individualSettingsDTOMapper;
         _individualMapper = individualMapper;
         _editValueOriginPresenter = editValueOriginPresenter;
         _diseaseStateSelectionPresenter = diseaseStateSelectionPresenter;
         _diseaseStateRepository = diseaseStateRepository;
         _cloner = cloner;
         _editValueOriginPresenter.ShowCaption = false;
         AddSubPresenters(_editValueOriginPresenter, _diseaseStateSelectionPresenter);
         _view.AddValueOriginView(_editValueOriginPresenter.View);
         _view.AddDiseaseStateView(_diseaseStateSelectionPresenter.View);
         _editValueOriginPresenter.ValueOriginUpdated = valueOriginUpdated;
      }

      private void loadFromIndividual(Individual individual)
      {
         _individualSettingsDTO = _individualSettingsDTOMapper.MapFrom(individual);
         _view.BindToSettings(_individualSettingsDTO);
         _view.BindToParameters(_individualSettingsDTO);
         _view.BindToSubPopulation(_individualSettingsDTO.SubPopulation);
         _editValueOriginPresenter.Edit(_individualSettingsDTO);
         _diseaseStateSelectionPresenter.Edit(_individualSettingsDTO.DiseaseState);
         updatePopulationControls();
      }

      public virtual void EditIndividual(Individual individualToEdit)
      {
         loadFromIndividual(individualToEdit);
         Individual = individualToEdit;
         IndividualCreated = true;
         _view.IsReadOnly = true;
      }

      private void valueOriginUpdated(ValueOrigin valueOrigin)
      {
         _individualSettingsDTO.ValueOrigin.UpdateFrom(valueOrigin);
         ViewChanged();
      }

      private IReadOnlyList<DiseaseState> allDiseaseStatesFor(SpeciesPopulation population)
      {
         var list = new List<DiseaseState> {_diseaseStateRepository.HealthyState};
         list.AddRange(_diseaseStateRepository.AllFor(population));
         return list;
      }

      public void PrepareForCreating()
      {
         //clone individual to ensure that we do not modify the default individual cached
         var individual = _cloner.Clone(_defaultIndividualRetriever.DefaultIndividual());
         loadFromIndividual(individual);
      }

      public void PrepareForScaling(Individual individualToScale) => loadFromIndividual(individualToScale);

      public bool SpeciesVisible
      {
         set => _view.SpeciesVisible = value;
      }

      public void AgeChanged() => RetrieveMeanValues();

      public void GestationalAgeChanged() => RetrieveMeanValues();

      public void SpeciesChanged()
      {
         _defaultValueUpdater.UpdateSettingsAfterSpeciesChange(_individualSettingsDTO);
         updateView();
      }

      public void PopulationChanged()
      {
         _individualSettingsDTO.Gender = _individualSettingsDTO.Population.DefaultGender;
         updateView();
      }

      public void GenderChanged()
      {
         updateView();
      }

      private void updateView()
      {
         if (_isUpdating) return;
         try
         {
            _isUpdating = true;
            _view.RefreshAllIndividualList();
            retrieveDefaultValues();
            updatePopulationControls();
         }
         finally
         {
            _isUpdating = false;
         }
      }

      public override void ViewChanged()
      {
         IndividualCreated = false;
         base.ViewChanged();
      }

      public void RetrieveMeanValues()
      {
         _defaultValueUpdater.UpdateMeanValueFor(_individualSettingsDTO);
         _view.BindToParameters(_individualSettingsDTO);
      }

      private void retrieveDefaultValues()
      {
         _defaultValueUpdater.UpdateDefaultValueFor(_individualSettingsDTO);
         _view.BindToParameters(_individualSettingsDTO);
         _view.BindToSubPopulation(_individualSettingsDTO.SubPopulation);
         _diseaseStateSelectionPresenter.ResetDiseaseState();
      }

      private void updatePopulationControls()
      {
         _view.AgeVisible = _individualSettingsDTO.Population.IsAgeDependent;
         _view.HeightAndBMIVisible = _individualSettingsDTO.Population.IsHeightDependent;
         _view.GestationalAgeVisible = _individualSettingsDTO.Population.IsPreterm;
         var allDiseaseStates = allDiseaseStatesFor(_individualSettingsDTO.Population);
         _diseaseStateSelectionPresenter.AllDiseaseStates = allDiseaseStates;
         //One is healthy. We show the selection if we have more than one
         _view.UpdateControlSizeAndVisibility(allDiseaseStates.HasAtLeastTwo());
      }

      public IEnumerable<Species> AllSpecies() => _speciesRepository.All();

      public IReadOnlyList<SpeciesPopulation> PopulationsFor(Species species) => species.Populations;

      public IReadOnlyList<Gender> GenderFor(SpeciesPopulation population) => population.Genders;

      public IEnumerable<ParameterValueVersion> AllParameterValueVersionsFor(string category)
      {
         return _individualSettingsDTO.Species.PVVCategoryByName(category).AllItems();
      }

      public IEnumerable<CalculationMethod> AllCalculationMethodsFor(string category)
      {
         return _calculationMethodCategoryRepository.FindBy(category).AllForSpecies(_individualSettingsDTO.Species);
      }

      public bool ShouldDisplayPvvCategory(string category) => AllParameterValueVersionsFor(category).HasAtLeastTwo();

      public bool ShouldDisplayCmCategory(string category) => AllCalculationMethodsFor(category).HasAtLeastTwo();

      public void CreateIndividual()
      {
         this.DoWithinExceptionHandler(() =>
         {
            Individual = null;
            Individual = _individualMapper.MapFrom(_individualSettingsDTO);
            IndividualCreated = true;
         });
      }

      public Individual Individual { get; private set; }
   }
}