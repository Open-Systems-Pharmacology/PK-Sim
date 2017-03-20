using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Services;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IPermeabilityGroupPresenter : ICompoundParameterGroupWithCalculatedDefaultPresenter, IPresenter<IPermeabilityGroupView>
   {
      void SetPermabilityValue(PermeabilityAlternativeDTO permeabilityAlternativeDTO, double newValue);
      void SetPermabilityUnit(IParameterDTO permeabilityParameter, Unit newUnit);
   }

   public abstract class PermeabilityGroupPresenterBase : CompoundParameterGroupWithAlternativePresenter<IPermeabilityGroupView>, IPermeabilityGroupPresenter
   {
      protected readonly IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper _permeabilityAlternativeDTOMapper;
      protected readonly ICalculatedParameterValuePresenter _calculatedParameterValuePresenter;
      private PermeabilityAlternativeDTO _calculatedDefault;
      private List<PermeabilityAlternativeDTO> _permeabilityDTOs;

      protected PermeabilityGroupPresenterBase(IPermeabilityGroupView view, ICompoundAlternativeTask compoundAlternativeTask,
                                               IRepresentationInfoRepository representationRepo,
                                               IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper permeabilityAlternativeDTOMapper,
                                               ICalculatedParameterValuePresenter calculatedParameterValuePresenter, IDialogCreator dialogCreator, string groupName)
         : base(view, representationRepo, compoundAlternativeTask, dialogCreator, groupName)
      {
         _permeabilityAlternativeDTOMapper = permeabilityAlternativeDTOMapper;
         _calculatedParameterValuePresenter = calculatedParameterValuePresenter;
         _calculatedParameterValuePresenter.Description = PKSimConstants.UI.PermeabilityCalculatedFromLipoAndMolWeight;
         view.SetDynamicParameterView(_calculatedParameterValuePresenter.View);
         AddSubPresenters(_calculatedParameterValuePresenter);
      }

      public abstract void UpdateCalculatedValue();

      public bool IsCalculatedAlternative(ParameterAlternativeDTO parameterAlternativeDTO)
      {
         return _calculatedDefault == parameterAlternativeDTO;
      }

      public void SetPermabilityValue(PermeabilityAlternativeDTO permeabilityAlternativeDTO, double newValue)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(permeabilityAlternativeDTO.PermeabilityParameter.Parameter, newValue));
      }

      public void SetPermabilityUnit(IParameterDTO permeabilityParameter, Unit newUnit)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterUnit(permeabilityParameter.Parameter, newUnit));
      }

      protected override IEnumerable<ParameterAlternativeDTO> FillUpParameterGroupAlternatives()
      {
         _permeabilityDTOs = GetPermeabilityDTOs().ToList();
         _calculatedDefault = _permeabilityDTOs.First();
         _view.BindTo(_permeabilityDTOs);
         return _permeabilityDTOs;
      }

      protected abstract IEnumerable<PermeabilityAlternativeDTO> GetPermeabilityDTOs();
   }
}