using PKSim.Core.Repositories;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ISolubilityAlternativeNamePresenter : IParameterAlternativeNamePresenter
   {
   }

   public class SolubilityAlternativeNamePresenter : ParameterAlternativeNamePresenter, ISolubilityAlternativeNamePresenter
   {
      public SolubilityAlternativeNamePresenter(ISolubilityAlternativeNameView view, IRepresentationInfoRepository representationInfoRepository) : base(view, representationInfoRepository)
      {
      }
   }
}