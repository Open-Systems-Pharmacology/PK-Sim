using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
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
}

public class OverwriteParameterSetsPresenter : AbstractSubPresenter<IOverwriteParameterSetsView, IOverwriteParameterSetsPresenter>, IOverwriteParameterSetsPresenter
{
   private readonly IOverwriteParameterSetToOverwriteParameterSetDTOMapper _mapper;
   private readonly IOverwriteParameterSetTask _overwriteParameterSetTask;
   private readonly IDialogCreator _dialogCreator;
   private readonly ISpeciesRepository _speciesRepository;
   private readonly IDiseaseStateRepository _diseaseStateRepository;
   private Compound _compound;

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
      _speciesRepository = speciesRepository;
      _diseaseStateRepository = diseaseStateRepository;
   }

   public IReadOnlyList<ExtendedPropertyOptionDTO> AllSpecies() =>
      _speciesRepository.All().Select(species => new ExtendedPropertyOptionDTO(species.Name, species.DisplayName, species.Icon)).ToList();

   public IReadOnlyList<ExtendedPropertyOptionDTO> AllDiseaseStates() =>
      _diseaseStateRepository.All().Select(diseaseState => new ExtendedPropertyOptionDTO(diseaseState.Name, diseaseState.DisplayName)).ToList();

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