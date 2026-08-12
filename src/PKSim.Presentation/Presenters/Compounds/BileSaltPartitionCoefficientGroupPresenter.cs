using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IBileSaltPartitionCoefficientGroupPresenter : IPermeabilityGroupPresenter
   {
   }

   public class BileSaltPartitionCoefficientGroupPresenter : PermeabilityGroupPresenterBase, IBileSaltPartitionCoefficientGroupPresenter
   {
      public BileSaltPartitionCoefficientGroupPresenter(IPermeabilityGroupView view,
         ICompoundAlternativeTask compoundAlternativeTask,
         ICompoundAlternativePresentationTask compoundAlternativePresentationTask,
         IRepresentationInfoRepository representationRepo,
         IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper permeabilityAlternativeDTOMapper,
         ICalculatedParameterValuePresenter calculatedParameterValuePresenter, IDialogCreator dialogCreator)
         : base(view, compoundAlternativeTask, compoundAlternativePresentationTask, representationRepo, permeabilityAlternativeDTOMapper, calculatedParameterValuePresenter, dialogCreator, CoreConstants.Groups.COMPOUND_BILE_SALT_PARTITION_COEFFICIENT)
      {
      }

      public override string ValueColumnCaption => PKSimConstants.UI.BileSaltPartitionCoefficientNeutral;

      protected override string CalculatedValueDescription => PKSimConstants.UI.BileSaltPartitionCoefficientCalculatedFromLipo;

      public override void UpdateCalculatedValue()
      {
         _calculatedParameterValuePresenter.Edit(_compoundAlternativeTask.BileSaltPartitionCoefficientValuesFor(_compound));
      }

      protected override IEnumerable<PermeabilityAlternativeDTO> GetPermeabilityDTOs()
      {
         return _parameterGroup.AllAlternatives.Select(alternative => _permeabilityAlternativeDTOMapper.MapFrom(alternative, CoreConstants.Parameters.BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL));
      }
   }
}
