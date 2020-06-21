using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IOrganPermeabilityGroupPresenter : IPermeabilityGroupPresenter
   {
   }

   public class OrganPermeabilityGroupPresenter : PermeabilityGroupPresenterBase, IOrganPermeabilityGroupPresenter
   {
      public OrganPermeabilityGroupPresenter(IPermeabilityGroupView view,
         ICompoundAlternativeTask compoundAlternativeTask,
         ICompoundAlternativePresentationTask compoundAlternativePresentationTask,
         IRepresentationInfoRepository representationRepo,
         IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper permeabilityAlternativeDTOMapper,
         ICalculatedParameterValuePresenter calculatedParameterValuePresenter, IDialogCreator dialogCreator)
         : base(view, compoundAlternativeTask,compoundAlternativePresentationTask,  representationRepo, permeabilityAlternativeDTOMapper, calculatedParameterValuePresenter, dialogCreator, CoreConstants.Groups.COMPOUND_PERMEABILITY)
      {
      }

      public override void UpdateCalculatedValue()
      {
         _calculatedParameterValuePresenter.Edit(_compoundAlternativeTask.PermeabilityValuesFor(_compound));
      }

      protected override IEnumerable<PermeabilityAlternativeDTO> GetPermeabilityDTOs()
      {
         return _parameterGroup.AllAlternatives.Select(alternative => _permeabilityAlternativeDTOMapper.MapFrom(alternative, CoreConstants.Parameters.PERMEABILITY));
      }
   }
}