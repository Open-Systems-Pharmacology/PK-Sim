using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_TemplatePresenter : ContextSpecification<ITemplatePresenter>
   {
      protected ITemplateTaskQuery _templateTaskQuery;
      private IBuildingBlockFromTemplateView _view;
      private IObjectTypeResolver _objectTypeResolver;
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
         _compound1= new Compound();
         _compound2 = new Compound();

         _template1 = new Template {Name = "Template1", Id = "Id1"};
         _template2 = new Template { Name = "Template2", Id = "Id2" };
         _template1.References.Add(_template2);
         _template2.References.Add(_template1);
         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.ActivateNode(new ObjectWithIdAndNameNode<Template>(_template1));
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
         _allTemplates.ShouldOnlyContain(_compound1,_compound2);
      }
   }
}