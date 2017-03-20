using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Views.ProteinExpression;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_ProteinExpressionsPresenter : ContextSpecification<IProteinExpressionsPresenter>
   {
      protected IProteinExpressionsView _view;
      protected ISubPresenterItemManager<IExpressionItemPresenter> _subPresenterManager;
      protected IDialogCreator _dialogCreator;
      protected IProteinExpressionQueries _proteinExpressionQueries;
      protected IMappingPresenter _mappingPresenter;
      private IProteinExpressionDataHelper _dataHelper;

      protected override void Context()
      {
         _view = A.Fake<IProteinExpressionsView>();
         _subPresenterManager = A.Fake<ISubPresenterItemManager<IExpressionItemPresenter>>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _proteinExpressionQueries = A.Fake<IProteinExpressionQueries>();
         _mappingPresenter = A.Fake<IMappingPresenter>();
         _dataHelper= A.Fake<IProteinExpressionDataHelper>();

         sut = new ProteinExpressionsPresenter(_view, _subPresenterManager, _dialogCreator, _proteinExpressionQueries, _mappingPresenter, _dataHelper);
      }
   }


   public class When_the_protein_expressions_presenter_is_closed : concern_for_ProteinExpressionsPresenter
   {

      protected override void Because()
      {
         sut.Dispose();
      }

      [Observation]
      public void should_close_modal_presenters()
      {
         A.CallTo(() => _mappingPresenter.Dispose()).MustHaveHappened();
      }
   }
}	
