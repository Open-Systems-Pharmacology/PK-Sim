using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
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
      IReadOnlyList<DiseaseState> AllDiseaseStatesFor(SpeciesPopulation population);

      void PrepareForCreating();
      void PrepareForScaling(Individual individualToScale);
      void PopulationChanged();
      void DiseaseStateChanged();
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
      private readonly IDiseaseStateRepository _diseaseStateRepository;

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
         IDiseaseStateRepository diseaseStateRepository) : base(view)
      {
         _speciesRepository = speciesRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _defaultValueUpdater = defaultValueUpdater;
         _individualSettingsDTOMapper = individualSettingsDTOMapper;
         _individualMapper = individualMapper;
         _editValueOriginPresenter = editValueOriginPresenter;
         _diseaseStateRepository = diseaseStateRepository;
         _editValueOriginPresenter.ShowCaption = false;
         AddSubPresenters(_editValueOriginPresenter);
         _view.AddValueOriginView(_editValueOriginPresenter.View);
         _editValueOriginPresenter.ValueOriginUpdated = valueOriginUpdated;
      }

      private void loadFromIndividual(Individual individual)
      {
         _individualSettingsDTO = _individualSettingsDTOMapper.MapFrom(individual);
         _view.BindToSettings(_individualSettingsDTO);
         _view.BindToParameters(_individualSettingsDTO);
         _view.BindToSubPopulation(_individualSettingsDTO.SubPopulation);
         _editValueOriginPresenter.Edit(_individualSettingsDTO);
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

      public IReadOnlyList<DiseaseState> AllDiseaseStatesFor(SpeciesPopulation population)
      {
         var list = new List<DiseaseState> {_diseaseStateRepository.HealthyState};
         list.AddRange(_diseaseStateRepository.AllFor(population));
         return list;
      }

      public void PrepareForCreating()
      {
         var individual = _defaultIndividualRetriever.DefaultIndividual();
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

      public void DiseaseStateChanged()
      {
         _defaultValueUpdater.UpdateDiseaseStateFor(_individualSettingsDTO);
         updateDiseaseStatesControls();
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
      }

      private void updatePopulationControls()
      {
         _view.AgeVisible = _individualSettingsDTO.Population.IsAgeDependent;
         _view.HeightAndBMIVisible = _individualSettingsDTO.Population.IsHeightDependent;
         _view.GestationalAgeVisible = _individualSettingsDTO.Population.IsPreterm;
         updateDiseaseStatesControls();
      }

      private void updateDiseaseStatesControls()
      {
         _view.BindToDiseaseState(_individualSettingsDTO);
      }

      public IEnumerable<Species> AllSpecies() => _speciesRepository.All();

      public IReadOnlyList<SpeciesPopulation> PopulationsFor(Species species)
      {
         return species.Populations;
      }

      public IReadOnlyList<Gender> GenderFor(SpeciesPopulation population) => population.Genders;

      public IEnumerable<ParameterValueVersion> AllParameterValueVersionsFor(string category)
      {
         return _individualSettingsDTO.Species.PVVCategoryByName(category).AllItems();
      }

      public IEnumerable<CalculationMethod> AllCalculationMethodsFor(string category)
      {
         return _calculationMethodCategoryRepository.FindBy(category).AllForSpecies(_individualSettingsDTO.Species);
      }

      public bool ShouldDisplayPvvCategory(string category)
      {
         return AllParameterValueVersionsFor(category).Count() > 1;
      }

      public bool ShouldDisplayCmCategory(string category)
      {
         return AllCalculationMethodsFor(category).Count() > 1;
      }

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