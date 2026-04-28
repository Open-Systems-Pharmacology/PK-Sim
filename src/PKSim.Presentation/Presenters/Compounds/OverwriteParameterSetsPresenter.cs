using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
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
}

public class OverwriteParameterSetsPresenter : AbstractSubPresenter<IOverwriteParameterSetsView, IOverwriteParameterSetsPresenter>, IOverwriteParameterSetsPresenter
{
   private readonly IOverwriteParameterSetToOverwriteParameterSetDTOMapper _mapper;
   private readonly IOverwriteParameterSetTask _overwriteParameterSetTask;
   private Compound _compound;

   public OverwriteParameterSetsPresenter(
      IOverwriteParameterSetsView view,
      IOverwriteParameterSetToOverwriteParameterSetDTOMapper mapper,
      IOverwriteParameterSetTask overwriteParameterSetTask)
      : base(view)
   {
      _mapper = mapper;
      _overwriteParameterSetTask = overwriteParameterSetTask;
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

   public void Handle(OverwriteParameterSetChangedEvent eventToHandle)
   {
      if (!Equals(eventToHandle.Compound, _compound))
         return;

      rebindView();
   }

   private void rebindView() => View.BindTo(_compound.OverwriteParameterSets.MapAllUsing(_mapper));
}