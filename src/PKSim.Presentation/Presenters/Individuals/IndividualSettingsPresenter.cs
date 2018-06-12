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
      IEnumerable<SpeciesPopulation> PopulationsFor(Species species);
      IEnumerable<Gender> GenderFor(SpeciesPopulation speciesPopulation);
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
      private readonly IIndividualDefaultValueRetriever _defaultValueRetriever;
      private readonly IIndividualToIIndividualSettingsDTOMapper _individualSettingsDTOMapper;
      private readonly IIndividualSettingsDTOToIndividualMapper _individualMapper;
      private readonly IEditValueOriginPresenter _editValueOriginPresenter;

      public bool IndividualCreated { get; private set; }

      private bool _isUpdating;
      private IndividualSettingsDTO _individualSettingsDTO;

      public IndividualSettingsPresenter(
         IIndividualSettingsView view,
         ISpeciesRepository speciesRepository,
         ICalculationMethodCategoryRepository calculationMethodCategoryRepository,
         IIndividualDefaultValueRetriever defaultValueRetriever,
         IIndividualToIIndividualSettingsDTOMapper individualSettingsDTOMapper,
         IIndividualSettingsDTOToIndividualMapper individualMapper,
         IEditValueOriginPresenter editValueOriginPresenter) : base(view)
      {
         _speciesRepository = speciesRepository;
         _calculationMethodCategoryRepository = calculationMethodCategoryRepository;
         _defaultValueRetriever = defaultValueRetriever;
         _individualSettingsDTOMapper = individualSettingsDTOMapper;
         _individualMapper = individualMapper;
         _editValueOriginPresenter = editValueOriginPresenter;
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

      public IEnumerable<CalculationMethod> AllCalculationMethodsFor(string category)
      {
         return _calculationMethodCategoryRepository.FindBy(category).AllForSpecies(_individualSettingsDTO.Species);
      }

      public void PrepareForCreating()
      {
         _individualSettingsDTO = _defaultValueRetriever.DefaultSettings();
         _view.BindToSettings(_individualSettingsDTO);

         updatePopulationControls();
         retrieveDefaultValues();
         _editValueOriginPresenter.Edit(_individualSettingsDTO);
      }

      public void PrepareForScaling(Individual individualToScale)
      {
         loadFromIndividual(individualToScale);
      }

      public bool SpeciesVisible
      {
         set => _view.SpeciesVisible = value;
      }

      public void AgeChanged() => RetrieveMeanValues();

      public void GestationalAgeChanged() => RetrieveMeanValues();

      public void SpeciesChanged()
      {
         _defaultValueRetriever.UpdateSettingsAfterSpeciesChange(_individualSettingsDTO);
         updateView();
      }

      public void PopulationChanged()
      {
         _individualSettingsDTO.Gender = _defaultValueRetriever.DefaultGenderFor(_individualSettingsDTO.SpeciesPopulation);
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
         _defaultValueRetriever.RetrieveMeanValueFor(_individualSettingsDTO);
         _view.BindToParameters(_individualSettingsDTO);
      }

      private void retrieveDefaultValues()
      {
         _defaultValueRetriever.RetrieveDefaultValueFor(_individualSettingsDTO);
         _view.BindToParameters(_individualSettingsDTO);
         _view.BindToSubPopulation(_individualSettingsDTO.SubPopulation);
      }

      private void updatePopulationControls()
      {
         _view.AgeVisible = _individualSettingsDTO.SpeciesPopulation.IsAgeDependent;
         _view.HeightAndBMIVisible = _individualSettingsDTO.SpeciesPopulation.IsHeightDependent;
         _view.GestationalAgeVisible = _individualSettingsDTO.SpeciesPopulation.IsPreterm;
      }

      public IEnumerable<Species> AllSpecies()
      {
         return _speciesRepository.All();
      }

      public IEnumerable<SpeciesPopulation> PopulationsFor(Species species)
      {
         return species.Populations;
      }

      public IEnumerable<Gender> GenderFor(SpeciesPopulation speciesPopulation)
      {
         return speciesPopulation.Genders;
      }

      public IEnumerable<ParameterValueVersion> AllParameterValueVersionsFor(string category)
      {
         return _individualSettingsDTO.Species.PVVCategoryByName(category).AllItems();
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