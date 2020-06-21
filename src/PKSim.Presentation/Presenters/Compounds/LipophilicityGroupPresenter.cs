using System.Collections.Generic;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ILipophilicityGroupPresenter : ICompoundParameterGroupWithAlternativePresenter
   {
      void SetLipophilicityValue(LipophilictyAlternativeDTO lipophilicityAlternativeDTO, double newValue);
   }

   public class LipophilicityGroupPresenter : CompoundParameterGroupWithAlternativePresenter<ILipophilicityGroupView>, ILipophilicityGroupPresenter
   {
      private readonly IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper _lipophilicityAlternativeDTOMapper;
      private IReadOnlyList<LipophilictyAlternativeDTO> _lipophilicityDTOs;

      public LipophilicityGroupPresenter(ILipophilicityGroupView view, 
         ICompoundAlternativeTask compoundAlternativeTask,
         ICompoundAlternativePresentationTask compoundAlternativePresentationTask,
         IRepresentationInfoRepository representationRepo,
         IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper lipophilicityAlternativeDTOMapper, 
         IDialogCreator dialogCreator ) :
         base(view, representationRepo, compoundAlternativeTask,compoundAlternativePresentationTask, dialogCreator, CoreConstants.Groups.COMPOUND_LIPOPHILICITY)
      {
         _lipophilicityAlternativeDTOMapper = lipophilicityAlternativeDTOMapper;
      }

      public void SetLipophilicityValue(LipophilictyAlternativeDTO lipophilicityAlternativeDTO, double newValue)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(lipophilicityAlternativeDTO.LipophilictyParameter.Parameter, newValue));
      }

      protected override IEnumerable<ParameterAlternativeDTO> FillUpParameterGroupAlternatives()
      {
         _lipophilicityDTOs = _parameterGroup.AllAlternatives.MapAllUsing(_lipophilicityAlternativeDTOMapper);
         _view.BindTo(_lipophilicityDTOs);
         return _lipophilicityDTOs;
      }
   }
}