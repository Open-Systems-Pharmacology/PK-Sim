using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ILipophilicityGroupPresenter : ICompoundParameterGroupWithAlternativePresenter
   {
      void SetLipophilicityValue(LipophilictyAlternativeDTO lipophilictyAlternativeDTO, double newValue);
   }

   public class LipophilicityGroupPresenter : CompoundParameterGroupWithAlternativePresenter<ILipophilicityGroupView>, ILipophilicityGroupPresenter
   {
      private readonly IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper _lipophilicityAlternativeDTOMapper;
      private IReadOnlyList<LipophilictyAlternativeDTO> _lipophilicityDTOs;

      public LipophilicityGroupPresenter(ILipophilicityGroupView view, ICompoundAlternativeTask compoundAlternativeTask,
                                         IRepresentationInfoRepository representationRepo, 
                                         IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper lipophilicityAlternativeDTOMapper, IDialogCreator dialogCreator) :
         base(view, representationRepo, compoundAlternativeTask, dialogCreator, CoreConstants.Groups.COMPOUND_LIPOPHILICITY)
      {
         _lipophilicityAlternativeDTOMapper = lipophilicityAlternativeDTOMapper;
      }

      public void SetLipophilicityValue(LipophilictyAlternativeDTO lipophilictyAlternativeDTO, double newValue)
      {
        AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(lipophilictyAlternativeDTO.LipophilictyParameter.Parameter, newValue));
      }

      protected override IEnumerable<ParameterAlternativeDTO> FillUpParameterGroupAlternatives()
      {
         _lipophilicityDTOs = _parameterGroup.AllAlternatives.MapAllUsing(_lipophilicityAlternativeDTOMapper);
         _view.BindTo(_lipophilicityDTOs);
         return _lipophilicityDTOs;
      }
   }
}