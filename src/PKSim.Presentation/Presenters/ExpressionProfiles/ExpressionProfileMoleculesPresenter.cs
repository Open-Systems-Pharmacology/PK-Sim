using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IExpressionProfileMoleculesPresenter : IExpressionProfileItemPresenter
   {
   }

   public class ExpressionProfileMoleculesPresenter : AbstractSubPresenter<IExpressionProfileMoleculesView,  IExpressionProfileMoleculesPresenter>, IExpressionProfileMoleculesPresenter
   {
      public ExpressionProfileMoleculesPresenter(IExpressionProfileMoleculesView view) : base(view)
      {
      }
   }
}