using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_TemplatePresenter : ContextSpecificationAsync<ITemplatePresenter>
   {
      protected ITemplateTaskQuery _templateTaskQuery;
      protected ITemplateView _view;
      private ITreeNodeContextMenuFactory _contextMenuFactory;
      private IApplicationController _applicationController;
      protected IDialogCreator _dialogCreator;
      protected IStartOptions _startOptions;
      protected IApplicationConfiguration _configuration;

      protected override Task Context()
      {
         _view = A.Fake<ITemplateView>();
         _templateTaskQuery = A.Fake<ITemplateTaskQuery>();
         _contextMenuFactory = A.Fake<ITreeNodeContextMenuFactory>();
         _applicationController = A.Fake<IApplicationController>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _startOptions = A.Fake<IStartOptions>();
         _configuration = A.Fake<IApplicationConfiguration>();
         sut = new TemplatePresenter(_view, _templateTaskQuery, _contextMenuFactory, _applicationController, _dialogCreator, _startOptions, _configuration);
         return _completed;
      }
   }

   public class When_checking_if_the_user_can_edit_a_given_template : concern_for_TemplatePresenter
   {
      private Template _userTemplate;
      private Template _systemTemplate;
      private Template _remoteTemplate;
      private TemplateDTO _userTemplateDTO;
      private TemplateDTO _systemTemplateDTO;
      private TemplateDTO _remoteTemplateDTO;

      protected override async Task Context()
      {
         await base.Context();
         _userTemplate = new LocalTemplate {DatabaseType = TemplateDatabaseType.User};
         _systemTemplate = new LocalTemplate {DatabaseType = TemplateDatabaseType.System};
         _remoteTemplate = new RemoteTemplate();
         _userTemplateDTO = new TemplateDTO(_userTemplate);
         _systemTemplateDTO = new TemplateDTO(_systemTemplate);
         _remoteTemplateDTO = new TemplateDTO(_remoteTemplate);
      }

      [Observation]
      public void should_return_true_if_the_template_is_a_user_template()
      {
         sut.CanEdit(_userTemplateDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_template_is_a_system_template()
      {
         sut.CanEdit(_systemTemplateDTO).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_template_is_a_system_template_and_the_user_is_admin()
      {
         A.CallTo(() => _startOptions.IsDeveloperMode).Returns(true);
         sut.CanEdit(_systemTemplateDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_template_is_a_remote_template_and_the_user_is_admin()
      {
         A.CallTo(() => _startOptions.IsDeveloperMode).Returns(true);
         sut.CanEdit(_remoteTemplateDTO).ShouldBeFalse();
      }
   }

   public class When_loading_a_template_with_references_containing_recursive_references : concern_for_TemplatePresenter
   {
      private IReadOnlyList<Compound> _allTemplates;
      private List<Template> _templates;
      private LocalTemplate _template1;
      private LocalTemplate _template2;
      private Compound _compound1;
      private Compound _compound2;

      protected override async Task Context()
      {
         await base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new LocalTemplate {Name = "Template1", Id = "Id1"};
         _template2 = new LocalTemplate {Name = "Template2", Id = "Id2"};
         _template1.References.Add(_template2);
         _template2.References.Add(_template1);
         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.SelectedTemplatesChanged(new[] {new TemplateDTO(_template1)});
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template2)).Returns(_compound2);

         A.CallTo(() => _templateTaskQuery.AllReferenceTemplatesFor(_template1, _compound1)).Returns(new[] {_template2});
         A.CallTo(() => _templateTaskQuery.AllReferenceTemplatesFor(_template2, _compound2)).Returns(new[] {_template1});
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override async Task Because()
      {
         _allTemplates = await sut.LoadFromTemplateAsync<Compound>(TemplateType.Compound);
      }

      [Observation]
      public void should_not_load_the_same_reference_twice()
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

      protected override async Task Context()
      {
         await base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new LocalTemplate {Name = "Template1", Id = "Id1"};
         _template2 = new LocalTemplate {Name = "Template2", Id = "Id2"};

         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.SelectedTemplatesChanged(new[] {new TemplateDTO(_template1), new TemplateDTO(_template2)});
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override async Task Because()
      {
         _allTemplates = await sut.LoadFromTemplateAsync<Compound>(TemplateType.Compound);
      }

      [Observation]
      public void should_load_all_selected_templates()
      {
         _allTemplates.ShouldOnlyContain(_compound1, _compound2);
      }

      [Observation]
      public void should_have_update_the_view_with_the_number_of_selected_templates()
      {
         _view.Description.ShouldBeEqualTo(PKSimConstants.UI.NumberOfTemplatesSelectedIs(2, TemplateType.Compound.ToString()));
      }
   }

   public class When_loading_templates_from_local_and_remote_locations : concern_for_TemplatePresenter
   {
      private List<Template> _templates;
      private Template _template1;
      private Template _remoteTemplateValid;
      private RemoteTemplate _remoteTemplateInvalid;
      private IReadOnlyList<TemplateDTO> _allTemplateDTOs;

      protected override async Task Context()
      {
         await base.Context();

         _template1 = new LocalTemplate {Name = "Template1", Id = "Id1"};
         _remoteTemplateInvalid = new RemoteTemplate {MinVersion = "13.0"};
         _remoteTemplateValid = new RemoteTemplate {Name = "Template2", Id = "Id2"};
         A.CallTo(() => _configuration.Version).Returns("12.0");
         _templates = new List<Template> {_template1, _remoteTemplateValid, _remoteTemplateInvalid};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         A.CallTo(() => _view.BindTo(A<IReadOnlyList<TemplateDTO>>._))
            .Invokes(x => _allTemplateDTOs = x.GetArgument<IReadOnlyList<TemplateDTO>>(0));
      }

      protected override async Task Because()
      {
         await sut.LoadFromTemplateAsync<Compound>(TemplateType.Compound);
      }

      [Observation]
      public void should_filter_out_templates_that_are_not_valid_for_this_version()
      {
         _allTemplateDTOs.Select(x => x.Template).ShouldOnlyContain(_template1, _remoteTemplateValid);
      }
   }

   public class When_deleting_the_selected_template_and_the_user_decided_to_not_delete_the_template_after_all : concern_for_TemplatePresenter
   {
      private List<Template> _templates;
      private Template _template1;
      private Template _template2;
      private Compound _compound1;
      private Compound _compound2;
      private TemplateDTO _templateDTO1;

      protected override async Task Context()
      {
         await base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new LocalTemplate {Name = "Template1", Id = "Id1"};
         _template2 = new LocalTemplate {Name = "Template2", Id = "Id2"};

         _templates = new List<Template> {_template1, _template2};
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         _templateDTO1 = new TemplateDTO(_template1);
         sut.SelectedTemplatesChanged(new[] {_templateDTO1, new TemplateDTO(_template2)});
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.No);
      }

      protected override Task Because()
      {
         sut.Delete(_templateDTO1);
         return _completed;
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
      private TemplateDTO _templateDTO1;

      protected override async Task Context()
      {
         await base.Context();
         _compound1 = new Compound();
         _compound2 = new Compound();

         _template1 = new LocalTemplate {Name = "Template1", Id = "Id1"};
         _template2 = new LocalTemplate {Name = "Template2", Id = "Id2"};

         _templates = new List<Template> {_template1, _template2};
         _templateDTO1 = new TemplateDTO(_template1);
         A.CallTo(() => _templateTaskQuery.AllTemplatesFor(TemplateType.Compound)).Returns(_templates);
         sut.SelectedTemplatesChanged(new[] {_templateDTO1, new TemplateDTO(_template2)});
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template1)).Returns(_compound1);
         A.CallTo(() => _templateTaskQuery.LoadTemplateAsync<Compound>(_template2)).Returns(_compound2);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
         await sut.LoadFromTemplateAsync<Compound>(TemplateType.Compound);
      }

      protected override Task Because()
      {
         sut.Delete(_templateDTO1);
         return _completed;
      }

      [Observation]
      public void should_delete_the_template_from_the_database()
      {
         A.CallTo(() => _templateTaskQuery.DeleteTemplate(_template1)).MustHaveHappened();
      }

      [Observation]
      public void should_rebind_to_the_view()
      {
         A.CallTo(() => _view.BindTo(A<IReadOnlyList<TemplateDTO>>._)).MustHaveHappened();
      }
   }
}