using System.Collections.Generic;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IExpressionProfileItemPresenter : ISubPresenter
   {
      void Edit(ExpressionProfile expressionProfile);
      void Save();
   }

   public class ExpressionProfileItems
   {
      public static readonly ExpressionProfileItem<IExpressionProfileMoleculesPresenter> Molecules = new ExpressionProfileItem<IExpressionProfileMoleculesPresenter> {Index = 0};
      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> {Molecules};
   }

   public class ExpressionProfileItem<TExpressionProfileItemPresenter> : SubPresenterItem<TExpressionProfileItemPresenter> where TExpressionProfileItemPresenter : IExpressionProfileItemPresenter
   {
   }
}