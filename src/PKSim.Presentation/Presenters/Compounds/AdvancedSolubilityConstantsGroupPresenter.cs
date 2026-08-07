using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IAdvancedSolubilityConstantsGroupPresenter : ICompoundAdvancedParameterGroupPresenter
   {
   }

   public class AdvancedSolubilityConstantsGroupPresenter : CompoundAdvancedParameterGroupPresenter<ICompoundAdvancedParameterGroupView>, IAdvancedSolubilityConstantsGroupPresenter
   {
      public AdvancedSolubilityConstantsGroupPresenter(ICompoundAdvancedParameterGroupView view, IRepresentationInfoRepository representationInfoRepository,
         IMultiParameterEditPresenter parameterEditPresenter, IParameterGroupTask parameterGroupTask)
         : base(view, representationInfoRepository, parameterEditPresenter, parameterGroupTask, CoreConstants.Groups.COMPOUND_ADVANCED_SOLUBILITY)
      {
         view.Hint = PKSimConstants.UI.CompoundAdvancedSolubilityParametersNote;
      }
   }
}
