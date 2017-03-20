using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_TemplateTask : ContextSpecification<ITemplateTask>
   {
      protected IDialogCreator _dialogCreator;
      protected ITemplateTaskQuery _templateTaskQuery;
      protected IApplicationController _applicationController;
      protected IObjectTypeResolver _objectTypeResolver;

      protected override void Context()
      {
         _dialogCreator= A.Fake<IDialogCreator>();
         _templateTaskQuery = A.Fake<ITemplateTaskQuery>();
         _applicationController= A.Fake<IApplicationController>();
         _objectTypeResolver= A.Fake<IObjectTypeResolver>();
         sut = new TemplateTask(_templateTaskQuery,_applicationController,_objectTypeResolver,_dialogCreator);
      }
   }

   public class When_loading_an_object_from_the_template_database : concern_for_TemplateTask
   {
      private IPopulationAnalysisField _field;
      private ITemplatePresenter _templatePresenter;
      private IPopulationAnalysisField _template;

      protected override void Context()
      {
         base.Context();
         _templatePresenter= A.Fake<ITemplatePresenter>();
         _template = A.Fake<IPopulationAnalysisField>();
         A.CallTo(() => _applicationController.Start<ITemplatePresenter>()).Returns(_templatePresenter);
         A.CallTo(() => _templatePresenter.LoadFromTemplate<IPopulationAnalysisField>(TemplateType.PopulationAnalysisField)).Returns(new[]{_template});
      }

      protected override void Because()
      {
         _field = sut.LoadFromTemplate<IPopulationAnalysisField>(TemplateType.PopulationAnalysisField);
      }

      [Observation]
      public void should_leverage_the_template_selection_presenter_to_select_a_template_from_the_database_and_return_the_selected_template()
      {
         _field.ShouldBeEqualTo(_template);
      }
   }

   public class When_saving_an_object_to_the_template_database : concern_for_TemplateTask
   {
      private IPopulationAnalysisField _field;
      private INameTemplatePresenter _namePresenter;
      private Template _templateItem;

      protected override void Context()
      {
         base.Context();
         _field = A.Fake<IPopulationAnalysisField>();
         _namePresenter= A.Fake<INameTemplatePresenter>();
         A.CallTo(_namePresenter).WithReturnType<bool>().Returns(true);
         A.CallTo(() => _namePresenter.Name).Returns("NEW_NAME");
         A.CallTo(() => _namePresenter.Description).Returns("DESCRIPTION");
         A.CallTo(() => _applicationController.Start<INameTemplatePresenter>()).Returns(_namePresenter);
         A.CallTo(() => _templateTaskQuery.SaveToTemplate(A<Template>._))
            .Invokes(x => _templateItem = x.GetArgument<Template>(0));

      }

      protected override void Because()
      {
         sut.SaveToTemplate(_field,TemplateType.PopulationAnalysisField,"TOTO");
      }

      [Observation]
      public void should_ask_user_to_enter_a_name_for_the_selected_template()
      {
         A.CallTo(() => _namePresenter.NewName("TOTO", TemplateType.PopulationAnalysisField)).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_object_to_the_template_database()
      {
         _templateItem.Name.ShouldBeEqualTo("NEW_NAME");
         _templateItem.Description.ShouldBeEqualTo("DESCRIPTION");
         _templateItem.TemplateType.ShouldBeEqualTo(TemplateType.PopulationAnalysisField);
      }

      [Observation]
      public void should_show_a_message_to_the_user_notifying_that_the_template_was_saved_properly()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustHaveHappened();
      }
   }

   public class When_saving_an_object_and_the_user_cancels_the_action : concern_for_TemplateTask
   {
      private IPopulationAnalysisField _field;
      private INameTemplatePresenter _namePresenter;

      protected override void Context()
      {
         base.Context();
         _field = A.Fake<IPopulationAnalysisField>();
         _namePresenter = A.Fake<INameTemplatePresenter>();
         A.CallTo(_namePresenter).WithReturnType<bool>().Returns(false);
         A.CallTo(() => _applicationController.Start<INameTemplatePresenter>()).Returns(_namePresenter);
      }

      protected override void Because()
      {
         sut.SaveToTemplate(_field, TemplateType.PopulationAnalysisField, "TOTO");
      }

      [Observation]
      public void should_not_do_anyting()
      {
         A.CallTo(_templateTaskQuery).WithAnyArguments().MustNotHaveHappened();
      }
   }
}	