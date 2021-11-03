using System.Collections.Generic;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{


   public interface ICreateExpressionProfilePresenter : ICreateBuildingBlockPresenter<ExpressionProfile>, IContainerPresenter
   {
   }

   public class CreateExpressionProfilePresenter : AbstractSubPresenterContainerPresenter<ICreateExpressionProfileView, ICreateExpressionProfilePresenter, IExpressionProfileItemPresenter>, ICreateExpressionProfilePresenter
   {
      public CreateExpressionProfilePresenter(ICreateExpressionProfileView view, ISubPresenterItemManager<IExpressionProfileItemPresenter> subPresenterItemManager, IReadOnlyList<ISubPresenterItem> subPresenterItems, IDialogCreator dialogCreator) : base(view, subPresenterItemManager, subPresenterItems, dialogCreator)
      {
      }

      public IPKSimCommand Create()
      {
         throw new System.NotImplementedException();
      }

      public ExpressionProfile BuildingBlock { get; }
   }
}