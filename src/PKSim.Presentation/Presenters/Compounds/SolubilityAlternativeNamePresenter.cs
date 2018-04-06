using PKSim.Core.Repositories;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ISolubilityAlternativeNamePresenter : IParameterAlternativeNamePresenter
   {
      bool CreateAsTable { get; set; }
   }

   public class SolubilityAlternativeNamePresenter : ParameterAlternativeNamePresenter<ISolubilityAlternativeNameView>, ISolubilityAlternativeNamePresenter
   {
      public bool CreateAsTable { get; set; }

      public SolubilityAlternativeNamePresenter(ISolubilityAlternativeNameView view, IRepresentationInfoRepository representationInfoRepository) : base(view, representationInfoRepository)
      {
         view.AttachPresenter(this);
      }
   }
}