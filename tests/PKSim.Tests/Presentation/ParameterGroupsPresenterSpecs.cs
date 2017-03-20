using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Parameters.Mappers;
using PKSim.Presentation.Services;

using PKSim.Presentation.Views.Parameters;

using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterGroupsPresenter : ContextSpecification<IParameterGroupsPresenter>
   {
      protected IParameterGroupsView _view;
      protected IParameterGroupNodeCreator _groupNodeCreator;
      protected IParameterGroupTask _parameterGroupTask;
      protected Organism _organism;
      private IParameter _parameter;
      private ParameterDTO _parameterDTO;
      protected INodeToCustomableParametersPresenterMapper _parameterPresenterMapper;

      protected IParameterContainerToTreeNodeMapper _containerNodeMapper;
      protected ITreeNode _containerNode;
      protected List<IParameter> _allParameters;
      protected ITreeNodeFactory _treeNodeFactory;
      protected ITreeNode<IGroup> _groupAllNode;
      protected ITreeNode<IGroup> _groupFavoritesNode;
      protected IGroupRepository _groupRepository;
      protected IUserSettings _userSettings;
      protected INoItemInSelectionPresenter _noItemInSelectionPresenter;
      private IPresentationSettingsTask _presenterSettingsTask;
      private ITreeNodeContextMenuFactory _treeNodeContextMenuFactory;

      protected override void Context()
      {
         _view = A.Fake<IParameterGroupsView>();
         _groupNodeCreator = A.Fake<IParameterGroupNodeCreator>();
         _parameterGroupTask = A.Fake<IParameterGroupTask>();
         _containerNodeMapper = A.Fake<IParameterContainerToTreeNodeMapper>();
         _parameterPresenterMapper = A.Fake<INodeToCustomableParametersPresenterMapper>();
         _noItemInSelectionPresenter = A.Fake<INoItemInSelectionPresenter>();
         _presenterSettingsTask= A.Fake<IPresentationSettingsTask>();
         _treeNodeFactory = A.Fake<ITreeNodeFactory>();
         _groupRepository = A.Fake<IGroupRepository>();
         _userSettings = A.Fake<IUserSettings>();
         _treeNodeContextMenuFactory= A.Fake<ITreeNodeContextMenuFactory>();
         _organism = A.Fake<Organism>();
         _allParameters = new List<IParameter>();
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _parameter.Rules).Returns(new BusinessRuleSet());
         _parameterDTO = A.Fake<ParameterDTO>();
         A.CallTo(() => _parameterDTO.Parameter).Returns(_parameter);
         A.CallTo(() => _organism.GetAllChildren(A<Func<IParameter, bool>>.Ignored)).Returns(_allParameters);
         _containerNode = A.Fake<ITreeNode>();
         A.CallTo(() => _containerNodeMapper.MapFrom(_organism)).Returns(_containerNode);
         A.CallTo(() => _containerNode.Children).Returns(new List<ITreeNode>());
         _groupAllNode = A.Fake<ITreeNode<IGroup>>();
         _groupFavoritesNode = A.Fake<ITreeNode<IGroup>>();
         A.CallTo(() => _treeNodeFactory.CreateGroupAll()).Returns(_groupAllNode);
         A.CallTo(() => _treeNodeFactory.CreateGroupFavorites()).Returns(_groupFavoritesNode);
         A.CallTo(() => _noItemInSelectionPresenter.BaseView).Returns(A.Fake<IView>());
      }

      protected void CreateSutForSettings(ParameterGroupingModeId parameterGroupingModeId)
      {
         A.CallTo(() => _userSettings.DefaultParameterGroupingMode).Returns(parameterGroupingModeId);
         var settings=new ParameterGroupsPresenterSettings();
         settings.DefaultParameterGroupingModeId =parameterGroupingModeId;
         A.CallTo(_presenterSettingsTask).WithReturnType<ParameterGroupsPresenterSettings>().Returns(settings);
         sut = new ParameterGroupsPresenter(_view, _parameterGroupTask, _groupNodeCreator, _containerNodeMapper,
                                            _parameterPresenterMapper, _noItemInSelectionPresenter, _treeNodeFactory, _groupRepository, _userSettings,
                                            _presenterSettingsTask, _treeNodeContextMenuFactory);
      }
   }

   public abstract class When_setting_the_grouping_mode_for_parameters : concern_for_ParameterGroupsPresenter
   {
      protected IEnumerable<ITreeNode> _nodes;
      private IParameter _visibleParameter;
      private Group _group;
      private IGroup _selectedGroup;

      protected override void Context()
      {
         base.Context();

         _selectedGroup = A.Fake<IGroup>();
         _selectedGroup.DisplayName = "toto";
         _selectedGroup.Visible = true;

         _visibleParameter = A.Fake<IParameter>();
         A.CallTo(() => _visibleParameter.Rules).Returns(new BusinessRuleSet());
         _visibleParameter.Visible = true;
         _visibleParameter.Name = "toto";
         A.CallTo(() => _visibleParameter.GroupName).Returns("toto");

         _group = new Group {Name = "toto", IsAdvanced = false};

         A.CallTo(() => _view.AddNodes(A<IEnumerable<ITreeNode>>._)).Invokes(x => _nodes = x.Arguments.Get<IEnumerable<ITreeNode>>(0));
         CreateSutForSettings(ParameterGroupingModeId.Advanced);

         A.CallTo(() => _groupRepository.GroupByName(_visibleParameter.GroupName)).Returns(_group);

         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).Returns(new[] { _selectedGroup });

         sut.InitializeWith(_organism, _allParameters);
      }
   }

   public class When_setting_the_grouping_mode_to_hierarchical_for_parameters : When_setting_the_grouping_mode_for_parameters
   {
      protected override void Because()
      {
         sut.ParameterGroupingMode = ParameterGroupingModes.Hierarchical;
      }

      [Observation]
      public void shouold_not_contain_favourites()
      {
         _nodes.ShouldNotContain(_groupFavoritesNode);
      }
   }

   public class When_setting_the_grouping_mode_to_simple_for_parameters : When_setting_the_grouping_mode_for_parameters
   {
      protected override void Because()
      {
         sut.ParameterGroupingMode = ParameterGroupingModes.Simple;
      }

      [Observation]
      public void Must_insert_favourites_at_the_top()
      {
         _nodes.First().ShouldBeEqualTo(_groupFavoritesNode);
      }
   }

   public class When_setting_the_grouping_mode_to_advanced_for_parameters : When_setting_the_grouping_mode_for_parameters
   {
      protected override void Because()
      {
         sut.ParameterGroupingMode = ParameterGroupingModes.Advanced;
      }

      [Observation]
      public void Must_insert_favourites_at_the_top()
      {
         _nodes.First().ShouldBeEqualTo(_groupFavoritesNode);
      }
   }

   public class When_editing_compound_parameters : concern_for_ParameterGroupsPresenter
   {
      private ITreeNode _compoundTreeNode;

      protected ICompoundInSimulationPresenter _compoundInSimulationPresenter;

      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Simple);
         _compoundTreeNode = A.Fake<ITreeNode>();
         
         _compoundInSimulationPresenter = A.Fake<ICompoundInSimulationPresenter>();
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_compoundTreeNode)).Returns(_compoundInSimulationPresenter);
         A.CallTo(_compoundInSimulationPresenter).WithReturnType<bool>().Returns(true);
         sut.InitializeWith(_organism, _allParameters);
      }

      protected override void Because()
      {
         sut.ActivateNode(_compoundTreeNode);
      }
 
      [Observation]
      public void should_edit_parameters_and_not_a_compound()
      {
         A.CallTo(() => _compoundInSimulationPresenter.Edit(A<IEnumerable<IParameter>>._)).MustHaveHappened();
      }
   }

   public class When_loading_an_container_in_the_parameter_group_presenter_in_advanced_mode : concern_for_ParameterGroupsPresenter
   {
      private IEnumerable<IGroup> _parameterGroups;
      private IGroup _group2;
      private IGroup _group1;
      private ITreeNode<IGroup> _nodeGroup2;
      private ITreeNode<IGroup> _nodeGroup1;
      private ICustomParametersPresenter _parameterEditPresenter1;
      private ICustomParametersPresenter _parameterEditPresenter2;
      private IEnumerable<ITreeNode> _nodes;

      protected override void Context()
      {
         base.Context();

         CreateSutForSettings(ParameterGroupingModeId.Advanced);

         _group1 = new Group{Name="Group1",Visible = true};
         _group2 = new Group{Name="Group2",Visible = true};
         _group1.IsAdvanced = false;
         _nodeGroup1 = new GroupNode(_group1);
         _nodeGroup2 = new GroupNode(_group2);
         _parameterEditPresenter1 = A.Fake<ICustomParametersPresenter>();
         _parameterEditPresenter2 = A.Fake<ICustomParametersPresenter>();
         _parameterGroups = new List<IGroup> {_group1, _group2};
         A.CallTo(() => _groupNodeCreator.MapFrom(_group1, A<IReadOnlyCollection<IParameter>>._)).Returns(_nodeGroup1);
         A.CallTo(() => _groupNodeCreator.MapFrom(_group2, A<IReadOnlyCollection<IParameter>>._)).Returns(_nodeGroup2);


         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).Returns(_parameterGroups);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_nodeGroup1)).Returns(_parameterEditPresenter1);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_nodeGroup2)).Returns(_parameterEditPresenter2);
         A.CallTo(() => _parameterGroupTask.ParametersIn(_group1, A<IEnumerable<IParameter>>.Ignored)).Returns(new List<IParameter>());
         A.CallTo(() => _view.AddNodes(A<IEnumerable<ITreeNode>>.Ignored)).Invokes(x => _nodes = x.GetArgument<IEnumerable<ITreeNode>>(0));
      }

      protected override void Because()
      {
         sut.InitializeWith(_organism, _allParameters);
      }

      [Observation]
      public void should_retrieve_all_parameter_groups_used_in_its_organism()
      {
         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_display_one_node_in_the_tree_view_for_each_visible_parameter_groups_and_the_all_and_favoritegroup()
      {
         _nodes.ShouldOnlyContain(_nodeGroup1, _nodeGroup2, _groupAllNode, _groupFavoritesNode);
      }
   }

   public class When_loading_an_container_in_the_parameter_group_presenter_in_simple_mode : concern_for_ParameterGroupsPresenter
   {
      private IEnumerable<IGroup> _parameterGroups;
      private IGroup _group2;
      private IGroup _group1;
      private ITreeNode<IGroup> _nodeGroup2;
      private ITreeNode<IGroup> _nodeGroup1;
      private ICustomParametersPresenter _parameterEditPresenter1;
      private ICustomParametersPresenter _parameterEditPresenter2;
      private IEnumerable<ITreeNode> _nodes;

      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Simple);

         _group1 = new Group { Name = "Group1", Visible = true };
         _group2 = new Group { Name = "Group2", Visible = true };

         _group1.IsAdvanced = false;
         _nodeGroup1 = new GroupNode(_group1);
         _nodeGroup2 = new GroupNode(_group2);
         _parameterEditPresenter1 = A.Fake<ICustomParametersPresenter>();
         _parameterEditPresenter2 = A.Fake<ICustomParametersPresenter>();
         _parameterGroups = new List<IGroup> {_group1, _group2};

         A.CallTo(() => _groupNodeCreator.MapFrom(_group1, A<IReadOnlyCollection<IParameter>>._)).Returns(_nodeGroup1);
         A.CallTo(() => _groupNodeCreator.MapFrom(_group2, A<IReadOnlyCollection<IParameter>>._)).Returns(_nodeGroup2);
         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).Returns(_parameterGroups);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_nodeGroup1)).Returns(_parameterEditPresenter1);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_nodeGroup2)).Returns(_parameterEditPresenter2);
         A.CallTo(() => _parameterGroupTask.ParametersIn(_group1, A<IEnumerable<IParameter>>.Ignored)).Returns(new List<IParameter>());
         A.CallTo(() => _view.AddNodes(A<IEnumerable<ITreeNode>>.Ignored)).Invokes(x => _nodes = x.GetArgument<IEnumerable<ITreeNode>>(0));
      }

      protected override void Because()
      {
         sut.InitializeWith(_organism, _allParameters);
      }

      [Observation]
      public void should_retrieve_all_parameter_groups_used_in_its_organism()
      {
         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_display_one_node_in_the_tree_view_for_each_visible_parameter_groups_and_the_favorite_group()
      {
         _nodes.ShouldOnlyContain(_nodeGroup1, _nodeGroup2, _groupFavoritesNode);
      }
   }

   public class When_the_user_is_selection_the_hierarchical_view : concern_for_ParameterGroupsPresenter
   {
      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Hierarchical);

         var group1 = new Group { Name = "Group1", Visible = true };

         var nodeGroup1 = new GroupNode(group1);
         var parameterEditPresenter1 = A.Fake<ICustomParametersPresenter>();
         var parameterGroups = new List<IGroup> {group1};
         A.CallTo(() => _groupNodeCreator.MapFrom(group1, A<IReadOnlyCollection<IParameter>>._)).Returns(nodeGroup1);


         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).Returns(parameterGroups);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(nodeGroup1)).Returns(parameterEditPresenter1);
         A.CallTo(() => _parameterGroupTask.ParametersIn(group1, A<IEnumerable<IParameter>>.Ignored)).Returns(new List<IParameter>());
      }

      protected override void Because()
      {
         sut.InitializeWith(_organism, _allParameters);
      }

      [Observation]
      public void should_retrieve_the_container_node_from_the_given_container()
      {
         A.CallTo(() => _containerNodeMapper.MapFrom(_organism)).MustHaveHappened();
      }

      [Observation]
      public void should_tell_the_view_to_display_the_children_of_the_root_node_for_the_container()
      {
         A.CallTo(() => _view.AddNodes(_containerNode.Children)).MustHaveHappened();
      }
   }

   public class When_a_parameter_group_node_is_being_activated : concern_for_ParameterGroupsPresenter
   {
      private ITreeNode<IGroup> _parameterGroupNode;
      private IGroup _selectedGroup;
      private IParameter _visibleParameter;
      private IParameter _hiddenParameter;
      private ICustomParametersPresenter _parameterEditPresenter;
      private string _fullPath;
      private IGroup _group;
      private IGroup _anotherGroup;

      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Simple);

         _fullPath = "A B C";
         _group = new Group{Name = "toto"};
         _group.IsAdvanced = false;

         _anotherGroup = new Group { Name = "toto" };
         _anotherGroup.IsAdvanced = false;

         _parameterEditPresenter = A.Fake<ICustomParametersPresenter>();
         A.CallTo(() => _parameterEditPresenter.BaseView).Returns(A.Fake<IMultiParameterEditView>());
         //One visible parameter
         _visibleParameter = A.Fake<IParameter>();
         A.CallTo(() => _visibleParameter.Rules).Returns(new BusinessRuleSet());
         _visibleParameter.Visible = true;
         _visibleParameter.Name = "toto";
         A.CallTo(() => _visibleParameter.GroupName).Returns("toto");
         //One hidden parameter
         _hiddenParameter = A.Fake<IParameter>();
         _hiddenParameter.Visible = false;
         A.CallTo(() => _hiddenParameter.GroupName).Returns("tata");
         _hiddenParameter.Name = "tata";

         _allParameters.AddRange(new[] {_visibleParameter, _hiddenParameter});
         _parameterGroupNode = A.Fake<ITreeNode<IGroup>>();
         _parameterGroupNode.Text = "toto";
         A.CallTo(() => _parameterGroupNode.FullPath(PKSimConstants.UI.DisplayPathSeparator)).Returns(_fullPath);
         _selectedGroup = A.Fake<IGroup>();
         _selectedGroup.DisplayName = "toto";
         A.CallTo(() => _parameterGroupNode.Tag).Returns(_selectedGroup);
         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).Returns(new[] {_selectedGroup});

         A.CallTo(() => _parameterPresenterMapper.MapFrom(_parameterGroupNode)).Returns(_parameterEditPresenter);
         A.CallTo(() => _parameterGroupTask.ParametersIn(_selectedGroup, A<IEnumerable<IParameter>>.Ignored)).Returns(new[] {_visibleParameter});
         A.CallTo(() => _groupRepository.GroupByName(_visibleParameter.GroupName)).Returns(_group);
         A.CallTo(() => _groupRepository.GroupByName(_hiddenParameter.GroupName)).Returns(_anotherGroup);
         var visibleParameterDTO = A.Fake<ParameterDTO>();
         A.CallTo(() => visibleParameterDTO.Name).Returns(_visibleParameter.Name);

         sut.InitializeWith(_organism, _allParameters);
      }

      protected override void Because()
      {
         sut.ActivateNode(_parameterGroupNode);
      }

      [Observation]
      public void should_update_the_selected_group_caption()
      {
         _view.GroupCaption.ShouldBeEqualTo(_fullPath);
      }

      [Observation]
      public void should_update_the_parameter_view_control()
      {
         A.CallTo(() => _view.ActivateView(_parameterEditPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_parameter_group_presenter_is_asked_if_the_presenter_can_be_closed : concern_for_ParameterGroupsPresenter
   {
      private ITreeNode<IGroup> _nodeToActivate;
      private IGroup _group;
      private ICustomParametersPresenter _validPresenter;

      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Simple);

         _nodeToActivate = A.Fake<ITreeNode<IGroup>>();
         _group = A.Fake<IGroup>();
         _group.DisplayName = "tutu";
         A.CallTo(() => _nodeToActivate.Tag).Returns(_group);
         _validPresenter = A.Fake<ICustomParametersPresenter>();
         var pkSimParameter = A.Fake<IParameter>();
         pkSimParameter.Visible = true;
         A.CallTo(() => pkSimParameter.GroupName).Returns("group1");
         var pkSimParameter2 = A.Fake<IParameter>();
         A.CallTo(() => pkSimParameter2.GroupName).Returns("group1");
         pkSimParameter2.Visible = true;
         _allParameters.Add(pkSimParameter);
         _allParameters.Add(pkSimParameter2);
         A.CallTo(() => _groupRepository.GroupByName("group1")).Returns(_group);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_nodeToActivate)).Returns(_validPresenter);
         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>._)).Returns(new[] {_group});
         A.CallTo(() => _parameterGroupTask.ParametersIn(_group, A<IEnumerable<IParameter>>._)).Returns(new[] {pkSimParameter, pkSimParameter2});
         A.CallTo(() => _groupNodeCreator.MapFrom(_group, A<IReadOnlyCollection<IParameter>>._)).Returns(_nodeToActivate);

         sut.InitializeWith(_organism, _allParameters);

         sut.ActivateNode(_nodeToActivate);
      }

      [Observation]
      public void should_return_true_if_the_active_presenter_is_valid()
      {
         A.CallTo(() => _validPresenter.CanClose).Returns(true);
         sut.CanClose.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_active_presenter_is_invalid()
      {
         A.CallTo(() => _validPresenter.CanClose).Returns(false);
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_a_node_for_which_the_underlying_group_contains_no_parameter_is_selected : concern_for_ParameterGroupsPresenter
   {
      private string _noParameterSelection;
      private ITreeNode<IGroup> _nodeToActivate;

      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Simple);
         _nodeToActivate = A.Fake<ITreeNode<IGroup>>();
         var group = A.Fake<IGroup>();
         A.CallTo(() => _nodeToActivate.Tag).Returns(group);
         _noParameterSelection = "no parameters";
         sut.NoSelectionCaption = _noParameterSelection;
         A.CallTo(() => _parameterGroupTask.ParametersIn(group, _allParameters)).Returns(Enumerable.Empty<IParameter>());
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_nodeToActivate)).Returns(A.Fake<IMultiParameterEditPresenter>());
         sut.InitializeWith(_organism, _allParameters);
      }

      protected override void Because()
      {
         sut.ActivateNode(_nodeToActivate);
      }

      [Observation]
      public void should_display_the_defined_node()
      {
         _noItemInSelectionPresenter.Description.ShouldBeEqualTo(_noParameterSelection);
         A.CallTo(() => _view.ActivateView(_noItemInSelectionPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_parameter_group_node_all_is_being_activated : concern_for_ParameterGroupsPresenter
   {
      private ICustomParametersPresenter _validPresenter;
      private IGroup _group;
      private IParameter _visibleParameter;
      private IEnumerable<IParameter> _parameters;

      protected override void Context()
      {
         base.Context();
         CreateSutForSettings(ParameterGroupingModeId.Advanced);

         _group = A.Fake<IGroup>();
         _validPresenter = A.Fake<ICustomParametersPresenter>();
         _visibleParameter = A.Fake<IParameter>();
         _visibleParameter.Visible = true;
         var hiddenParameter = A.Fake<IParameter>();
         hiddenParameter.Visible = false;
         _allParameters.Add(_visibleParameter);
         _allParameters.Add(hiddenParameter);
         A.CallTo(() => _visibleParameter.GroupName).Returns("toto");
         A.CallTo(() => hiddenParameter.GroupName).Returns("toto");
         A.CallTo(() => _groupRepository.GroupByName(_visibleParameter.GroupName)).Returns(_group);
         A.CallTo(() => _parameterPresenterMapper.MapFrom(_groupAllNode)).Returns(_validPresenter);
         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>.Ignored)).Returns(new[] {_group});
         A.CallTo(() => _parameterGroupTask.ParametersIn(_group, _allParameters)).Returns(new[] {_visibleParameter, hiddenParameter});
         A.CallTo(() => _validPresenter.Edit(A<IEnumerable<IParameter>>.Ignored)).Invokes(x => _parameters = x.GetArgument<IEnumerable<IParameter>>(0));
         sut.InitializeWith(_organism, _allParameters);
         sut.ParameterGroupingMode = ParameterGroupingModes.Advanced;
      }

      protected override void Because()
      {
         sut.ActivateNode(_groupAllNode);
      }

      [Observation]
      public void should_edit_all_the_visible_parameters()
      {
         _parameters.ShouldOnlyContain(_visibleParameter);
      }
   }
}