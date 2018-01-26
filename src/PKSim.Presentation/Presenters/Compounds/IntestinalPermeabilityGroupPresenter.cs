using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IIntestinalPermeabilityGroupPresenter : IPermeabilityGroupPresenter
   {
   }

   public class IntestinalPermeabilityGroupPresenter : PermeabilityGroupPresenterBase, IIntestinalPermeabilityGroupPresenter
   {
      public IntestinalPermeabilityGroupPresenter(IPermeabilityGroupView view, ICompoundAlternativeTask compoundAlternativeTask, 
                                                  IRepresentationInfoRepository representationRepo,
                                                  IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper permeabilityAlternativeDTOMapper,
                                                  ICalculatedParameterValuePresenter calculatedParameterValuePresenter, IDialogCreator dialogCreator) :
         base(view, compoundAlternativeTask, representationRepo, permeabilityAlternativeDTOMapper, calculatedParameterValuePresenter, dialogCreator, CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY)
      {
      }

      public override void UpdateCalculatedValue()
      {
         _calculatedParameterValuePresenter.Edit(_compoundAlternativeTask.IntestinalPermeabilityValuesFor(_compound));
      }

      protected override IEnumerable<PermeabilityAlternativeDTO> GetPermeabilityDTOs()
      {
         return _parameterGroup.AllAlternatives.Select(alternative => _permeabilityAlternativeDTOMapper.MapFrom(alternative, CoreConstants.Parameter.SPECIFIC_INTESTINAL_PERMEABILITY));
      }
   }
}