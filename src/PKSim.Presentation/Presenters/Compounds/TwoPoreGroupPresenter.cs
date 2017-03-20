using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ITwoPoreGroupPresenter : ICompoundAdvancedParameterGroupPresenter
   {
   }

   public class TwoPoreGroupPresenter : CompoundAdvancedParameterGroupPresenter<ICompoundAdvancedParameterGroupView>, ITwoPoreGroupPresenter
   {
      public TwoPoreGroupPresenter(ICompoundAdvancedParameterGroupView view, IRepresentationInfoRepository representationInfoRepository, IMultiParameterEditPresenter parameterEditPresenter, IParameterGroupTask parameterGroupTask) :
         base(view, representationInfoRepository, parameterEditPresenter, parameterGroupTask, CoreConstants.Groups.COMPOUND_TWO_PORE)
      {
         view.Hint = PKSimConstants.UI.CompoundTwoPoreParametersNote;
      }
   }
}