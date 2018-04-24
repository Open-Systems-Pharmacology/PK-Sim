using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_TemplatePresenter : ContextSpecification<ITemplatePresenter>
   {
      protected ITemplateTaskQuery _templateTaskQuery;
      protected IBuildingBlockFromTemplateView _view;
      protected IObjectTypeResolver _objectTypeResolver;
      private ITreeNodeFactory _treeNodeFactory;
      private ITreeNodeContextMenuFactory _contextMenuFactory;
      private IApplicationController _applicationController;
      protected IDialogCreator _dialogCreator;
      protected IStartOptions _startOptions;

      protected override void Context()
      {
         _view = A.Fake<IBuildingBlockFromTemplateView>();
         _templateTaskQuery = A.Fake<ITemplateTaskQuery>();
         _objectTypeResolver = A.Fake<IObjectTypeResolver>();
         _treeNodeFactory = A.Fake<ITreeNodeFactory>();
         _contextMenuFactory = A.Fake<ITreeNodeContextMenuFactory>();
         _applicationController = A.Fake<IApplicationController>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _startOptions = A.Fake<IStartOptions>();
         sut = new TemplatePresenter(_view, _templateTaskQuery, _objectTypeResolver, _treeNodeFactory, _contextMenuFactory, _applicationController, _dialogCreator, _startOptions);
      }
   }

   public class When_checking_if_the_user_can_edit_a_given_template : concern_for_TemplatePresenter
   {
      private Template _userTemplate;
      private Template _systemTemplate;

      protected override void Context()
      {
         base.Context();
         _userTemplate = new Template {DatabaseType = TemplateDatabaseType.User};
         _systemTemplate = new Template {DatabaseType = TemplateDatabaseType.System};
      }

      [Observation]
      public void should_return_true_if_the_template_is_a_user_template()
      {
         sut.CanEdit(_userTemplate).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_template_is_a_system_template()
      {
         sut.CanEdit(_systemTemplate).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_template_is_a_system_template_and_the_user_is_admin()
      {
         A.CallTo(() => _startOptions.IsDeveloperMode).Returns(true);
         sut.CanEdit(_systemTemplate).ShouldBeTrue();
      }
   }

   public class When_loading_a_template_with_references_containing_recursive_references : concern_for_TemplatePresenter
   {
      private IReadOnlyList<Compound> _allTemplates;
      private List<Template> _templates;
      private Template _template1;
      private Template _template2;
      private Compound _compound1;
      private Compound _compound2;

      protected override void Context()
      {
         base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new Template {Name = "Template1", Id = "Id1"};
         _template2 = new Template {Name = "Template2", Id = "Id2"};
         _template1.References.Add(_template2);
         _template2.References.Add(_template1);
         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.ActivateNodes(new[] {new ObjectWithIdAndNameNode<Template>(_template1)});
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         _allTemplates = sut.LoadFromTemplate<Compound>(TemplateType.Compound);
      }

      [Observation]
      public void should_not_load_thee_same_reference_twice()
      {
         _allTemplates.ShouldOnlyContain(_compound1, _compound2);
      }
   }

   public class When_selecting_multiple_templates_at_the_same_time : concern_for_TemplatePresenter
   {
      private IReadOnlyList<Compound> _allTemplates;
      private List<Template> _templates;
      private Template _template1;
      private Template _template2;
      private Compound _compound1;
      private Compound _compound2;
      private readonly string _templateType = "TEMPLATE TYPE";

      protected override void Context()
      {
         base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new Template {Name = "Template1", Id = "Id1"};
         _template2 = new Template {Name = "Template2", Id = "Id2"};

         A.CallTo(() => _objectTypeResolver.TypeFor<Compound>()).Returns(_templateType);
         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.ActivateNodes(new[] {new ObjectWithIdAndNameNode<Template>(_template1), new ObjectWithIdAndNameNode<Template>(_template2)});
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         _allTemplates = sut.LoadFromTemplate<Compound>(TemplateType.Compound);
      }

      [Observation]
      public void should_load_all_selected_templates()
      {
         _allTemplates.ShouldOnlyContain(_compound1, _compound2);
      }

      [Observation]
      public void should_have_update_the_view_with_the_number_of_selected_templates()
      {
         _view.Description.ShouldBeEqualTo(PKSimConstants.UI.NumberOfTemplatesSelectedIs(2, _templateType));
      }
   }

   public class When_deleting_the_selected_template_and_the_user_decided_to_not_delete_the_template_after_all : concern_for_TemplatePresenter
   {
      private List<Template> _templates;
      private Template _template1;
      private Template _template2;
      private Compound _compound1;
      private Compound _compound2;
      private readonly string _templateType = "TEMPLATE TYPE";

      protected override void Context()
      {
         base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new Template {Name = "Template1", Id = "Id1"};
         _template2 = new Template {Name = "Template2", Id = "Id2"};

         A.CallTo(() => _objectTypeResolver.TypeFor<Compound>()).Returns(_templateType);
         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.ActivateNodes(new[] {new ObjectWithIdAndNameNode<Template>(_template1), new ObjectWithIdAndNameNode<Template>(_template2)});
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.Delete(_template1);
      }

      [Observation]
      public void should_not_delete_the_template_from_the_database()
      {
         A.CallTo(() => _templateTaskQuery.DeleteTemplate(_template1)).MustNotHaveHappened();
      }
   }

   public class When_deleting_the_selected_template_and_the_user_confirms_deletion : concern_for_TemplatePresenter
   {
      private List<Template> _templates;
      private Template _template1;
      private Template _template2;
      private Compound _compound1;
      private Compound _compound2;
      private readonly string _templateType = "TEMPLATE TYPE";

      protected override void Context()
      {
         base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new Template { Name = "Template1", Id = "Id1" };
         _template2 = new Template { Name = "Template2", Id = "Id2" };

         A.CallTo(() => _objectTypeResolver.TypeFor<Compound>()).Returns(_templateType);
         _templates = new List<Template> { _template1, _template2 };
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.ActivateNodes(new[] { new ObjectWithIdAndNameNode<Template>(_template1), new ObjectWithIdAndNameNode<Template>(_template2) });
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplate<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }
      
      protected override void Because()
      {
         sut.Delete(_template1);
      }

      [Observation]
      public void should_delete_the_template_from_the_database()
      {
         A.CallTo(() => _templateTaskQuery.DeleteTemplate(_template1)).MustHaveHappened();
      }

      [Observation]
      public void should_refresh_the_selected_node_from_the_view()
      {
         A.CallTo(() => _view.SelectTemplate(_template2)).MustHaveHappened();
      }
   }

}