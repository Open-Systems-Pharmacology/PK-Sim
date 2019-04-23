using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IParticleDissolutionGroupPresenter : ICompoundAdvancedParameterGroupPresenter
   {
   }

   public class ParticleDissolutionGroupPresenter : CompoundAdvancedParameterGroupPresenter<ICompoundAdvancedParameterGroupView>, IParticleDissolutionGroupPresenter
   {
      public ParticleDissolutionGroupPresenter(ICompoundAdvancedParameterGroupView view, IRepresentationInfoRepository representationInfoRepository,
                                               IMultiParameterEditPresenter parameterEditPresenter, IParameterGroupTask parameterGroupTask)
         : base(view, representationInfoRepository, parameterEditPresenter, parameterGroupTask, CoreConstants.Groups.COMPOUND_DISSOLUTION)
      {
         view.Hint = PKSimConstants.UI.CompoundParticleDissolutionParametersNote;
         view.IsLargeHint = true;
      }
   }
}