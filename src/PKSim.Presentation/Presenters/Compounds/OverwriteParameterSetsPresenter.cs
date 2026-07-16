using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds;

public interface IOverwriteParameterSetsPresenter : ICompoundItemPresenter, IListener<OverwriteParameterSetChangedEvent>
{
   void UpdateParameterValue(OverwriteParameterSetDTO setDTO, OverwriteParameterValueDTO parameterValueDTO, double newValue);
   void RemoveParameterValue(OverwriteParameterSetDTO setDTO, OverwriteParameterValueDTO parameterValueDTO);
   void SetIsDefault(OverwriteParameterSetDTO setDTO, bool isDefault);
   void RemoveSet(OverwriteParameterSetDTO setDTO);
   void SetExtendedProperty(OverwriteParameterSetDTO setDTO, string propertyName, string newValue);
   IReadOnlyList<ExtendedPropertyOptionDTO> AllSpecies();
   IReadOnlyList<ExtendedPropertyOptionDTO> AllDiseaseStates();
   IReadOnlyList<string> MetadataPropertyNamesFor(OverwriteParameterSetDTO selectedSet);
   ExtendedPropertyOptionDTO ExtendedPropertyDTOFor(string extendedPropertyName);
}

public class OverwriteParameterSetsPresenter : AbstractSubPresenter<IOverwriteParameterSetsView, IOverwriteParameterSetsPresenter>, IOverwriteParameterSetsPresenter
{
   private readonly IOverwriteParameterSetToOverwriteParameterSetDTOMapper _mapper;
   private readonly IOverwriteParameterSetTask _overwriteParameterSetTask;
   private readonly IDialogCreator _dialogCreator;
   private Compound _compound;
   private readonly Cache<string, ExtendedPropertyOptionDTO> _allSpecies = new(getKey:x => x.Name);
   private readonly Cache<string, ExtendedPropertyOptionDTO> _allDiseaseStates = new(getKey: x => x.Name);
   private readonly ExtendedPropertyOptionDTO _emptySpecies = new(string.Empty, string.Empty);
   private readonly ExtendedPropertyOptionDTO _emptyDiseaseState = new(string.Empty, string.Empty);

   public OverwriteParameterSetsPresenter(
      IOverwriteParameterSetsView view,
      IOverwriteParameterSetToOverwriteParameterSetDTOMapper mapper,
      IOverwriteParameterSetTask overwriteParameterSetTask,
      IDialogCreator dialogCreator,
      ISpeciesRepository speciesRepository,
      IDiseaseStateRepository diseaseStateRepository)
      : base(view)
   {
      _mapper = mapper;
      _overwriteParameterSetTask = overwriteParameterSetTask;
      _dialogCreator = dialogCreator;
      _allSpecies.Add(_emptySpecies);
      _allDiseaseStates.Add(_emptyDiseaseState);
      _allDiseaseStates.AddRange(diseaseStateRepository.All().Select(diseaseState => new ExtendedPropertyOptionDTO(diseaseState.Name, diseaseState.DisplayName)));
      _allSpecies.AddRange(speciesRepository.All().Select(species => new ExtendedPropertyOptionDTO(species.Name, species.DisplayName, species.Icon)));
   }

   public IReadOnlyList<ExtendedPropertyOptionDTO> AllSpecies() => _allSpecies.ToList();

   public IReadOnlyList<ExtendedPropertyOptionDTO> AllDiseaseStates() => _allDiseaseStates.ToList();

   public IReadOnlyList<string> MetadataPropertyNamesFor(OverwriteParameterSetDTO selectedSet)
   {
      var defaultPropertyNames = new List<string> { PKSimConstants.UI.Species, PKSimConstants.UI.DiseaseState };
      if (selectedSet == null)
         return defaultPropertyNames;

      var existingNames = selectedSet.OverwriteParameterSet.ExtendedProperties.All.Select(x => x.Name);

      // preserve this order (adding existing names to defaults). That ensures that Species and Disease State
      // are always created in this order and before any other extended properties
      defaultPropertyNames.AddRange(existingNames);
      return defaultPropertyNames.Distinct().ToList();
   }

   public ExtendedPropertyOptionDTO ExtendedPropertyDTOFor(string extendedPropertyName)
   {
      if(_allDiseaseStates.Contains(extendedPropertyName))
         return _allDiseaseStates[extendedPropertyName];

      return _allSpecies[extendedPropertyName];
   }

   public void EditCompound(Compound compound)
   {
      _compound = compound;
      rebindView();
   }

   public void UpdateParameterValue(OverwriteParameterSetDTO setDTO, OverwriteParameterValueDTO parameterValueDTO, double newValue) =>
      AddCommand(_overwriteParameterSetTask.UpdateParameterValue(setDTO.OverwriteParameterSet, _compound, parameterValueDTO.ParameterValue.Path.PathAsString, newValue));

   public void RemoveParameterValue(OverwriteParameterSetDTO setDTO, OverwriteParameterValueDTO parameterValueDTO) =>
      AddCommand(_overwriteParameterSetTask.RemoveParameterValue(setDTO.OverwriteParameterSet, _compound, parameterValueDTO.ParameterValue.Path.PathAsString));

   public void SetIsDefault(OverwriteParameterSetDTO setDTO, bool isDefault) =>
      AddCommand(_overwriteParameterSetTask.SetIsDefault(setDTO.OverwriteParameterSet, _compound, isDefault));

   public void SetExtendedProperty(OverwriteParameterSetDTO setDTO, string propertyName, string newValue) =>
      AddCommand(_overwriteParameterSetTask.SetExtendedProperty(setDTO.OverwriteParameterSet, _compound, propertyName, newValue));

   public void RemoveSet(OverwriteParameterSetDTO setDTO)
   {
      var viewResult = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteObjectOfType(PKSimConstants.ObjectTypes.OverwriteParameterSet, setDTO.Name));
      if (viewResult == ViewResult.No)
         return;

      AddCommand(_overwriteParameterSetTask.RemoveSet(setDTO.OverwriteParameterSet, _compound));
   }

   public void Handle(OverwriteParameterSetChangedEvent eventToHandle)
   {
      if (!Equals(eventToHandle.Compound, _compound))
         return;

      rebindView();
   }

   private void rebindView() => View.BindTo(_compound.OverwriteParameterSets.MapAllUsing(_mapper));
}