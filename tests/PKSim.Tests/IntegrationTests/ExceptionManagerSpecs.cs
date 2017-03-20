using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using FakeItEasy;
using PKSim.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ExceptionManager : ContextSpecification<IExceptionManager>
   {
      protected IDialogCreator _dialogCreator;
      protected IExceptionView _exceptionView;
      private IPKSimConfiguration _configuration;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _exceptionView = A.Fake<IExceptionView>();
         _configuration = A.Fake<IPKSimConfiguration>();
         sut = new ExceptionManager(_dialogCreator, _exceptionView,_configuration);
      }
   }

   
   public class When_showing_a_pksim_exception : concern_for_ExceptionManager
   {
      private Exception _exceptionToShow;
      private Exception _childException;
      private string _message;

      protected override void Context()
      {
         base.Context();
         _childException = new PKSimException("ChildMessageError");
         _exceptionToShow = new PKSimException("ParentMessageError", _childException);
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>.Ignored)).Invokes(
            x => _message = x.GetArgument<string>(0));
      }

      protected override void Because()
      {
         sut.LogException(_exceptionToShow);
      }

      [Observation]
      public void should_leverage_the_dialog_creator_to_display_an_error_message_containg_the_exception_message()
      {
         _message.Contains("ParentMessageError").ShouldBeTrue();
         _message.Contains("ChildMessageError").ShouldBeTrue();
      }
   }

   
   public class When_showing_a_generic_exception : concern_for_ExceptionManager
   {
      private Exception _exceptionToShow;

      protected override void Context()
      {
         base.Context();
         _exceptionToShow = new Exception("Error");
      }

      protected override void Because()
      {
         sut.LogException(_exceptionToShow);
      }

      [Observation]
      public void should_leverage_the_exception_view_to_display_an_error_message_containg_the_exception_message()
      {
         A.CallTo(() => _exceptionView.Display(_exceptionToShow)).MustHaveHappened();
      }
   }
}