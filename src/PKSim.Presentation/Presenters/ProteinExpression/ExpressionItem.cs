using System.Collections.Generic;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public static class ExpressionItems
   {
      public static readonly ExpressionItem<IProteinSelectionPresenter> ProteinSelection = new ExpressionItem<IProteinSelectionPresenter>();
      public static readonly ExpressionItem<IExpressionDataPresenter> ExpressionData = new ExpressionItem<IExpressionDataPresenter>();
      public static readonly ExpressionItem<ITransferPresenter> Transfer = new ExpressionItem<ITransferPresenter>();

      public static readonly IReadOnlyList<ISubPresenterItem> All = new List<ISubPresenterItem> { ProteinSelection, ExpressionData, Transfer};
   }

   public class ExpressionItem<TExpressionItemPresenter> : SubPresenterItem<TExpressionItemPresenter> where TExpressionItemPresenter : IExpressionItemPresenter
   {
   }
}