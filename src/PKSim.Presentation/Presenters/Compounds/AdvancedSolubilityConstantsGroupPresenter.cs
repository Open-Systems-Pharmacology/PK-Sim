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
      //the partition coefficient names are long enough to be truncated at an even column width
      private const int NAME_COLUMN_WEIGHT = 250;

      public AdvancedSolubilityConstantsGroupPresenter(ICompoundAdvancedParameterGroupView view, IRepresentationInfoRepository representationInfoRepository,
         IMultiParameterEditPresenter parameterEditPresenter, IParameterGroupTask parameterGroupTask)
         : base(view, representationInfoRepository, parameterEditPresenter, parameterGroupTask, CoreConstants.Groups.COMPOUND_ADVANCED_SOLUBILITY)
      {
         //the note belongs to the whole Advanced Intestinal Solubility panel and is shown by AdvancedSolubilityPresenter
         _parameterEditPresenter.ParameterNameColumnWeight = NAME_COLUMN_WEIGHT;
      }
   }
}
