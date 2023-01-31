using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.Parameters.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IParameterGroupsPresenter : IPresenter<IParameterGroupsView>, ICommandCollectorPresenter, IPresenterWithSettings, IPresenterWithContextMenu<ITreeNode>
   {
      /// <summary>
      ///    Active the <paramref name="node">parameter group node</paramref>
      /// </summary>
      void ActivateNode(ITreeNode node);

      /// <summary>
      ///    Initialize the parameter group presenter with the container for which the parameter should be displayed
      /// </summary>
      void InitializeWith(IContainer container);

      /// <summary>
      ///    Initialize the parameter group presenter with the container for which the parameter filtered with the predicate
      ///    should be displayed
      /// </summary>
      /// <param name="container">root container</param>
      /// <param name="predicate">predicate used to filter the parameters to display</param>
      void InitializeWith(IContainer container, Func<IParameter, bool> predicate);

      /// <summary>
      ///    Initialize the parameter group presenter with the container for which then given parameter should be displayed
      /// </summary>
      /// <param name="container">root container</param>
      /// <param name="allParameters">Parameters to display</param>
      void InitializeWith(IContainer container, IEnumerable<IParameter> allParameters);

      /// <summary>
      ///    Caption to display when no group is being selected
      /// </summary>
      string NoSelectionCaption { get; set; }

      /// <summary>
      ///    Indicates that the way the parameters are displayed has changed
      /// </summary>
      ParameterGroupingMode ParameterGroupingMode { get; set; }

      IEnumerable<ParameterGroupingMode> AllGroupingModes { get; }

      IEnumerable<IParameter> AllParametersInSelectedGroup { get; }

      //Ensure that the presenter currently activated is being refreshed 
      void RefreshActivePresenter();
   }

   public class ParameterGroupsPresenter : AbstractCommandCollectorPresenter<IParameterGroupsView, IParameterGroupsPresenter>, IParameterGroupsPresenter
   {
      private readonly IParameterGroupTask _parameterGroupTask;
      private readonly IParameterGroupNodeCreator _groupNodeCreator;
      private readonly IParameterContainerToTreeNodeMapper _containerNodeMapper;
      private readonly INodeToCustomizableParametersPresenterMapper _parametersPresenterMapper;
      private readonly INoItemInSelectionPresenter _noItemInSelectionPresenter;
      private readonly IGroupRepository _groupRepository;
      private readonly ICoreUserSettings _userSettings;
      private readonly IPresentationSettingsTask _presentationSettingsTask;
      private readonly ITreeNodeContextMenuFactory _treeNodeContextMenuFactory;
      private readonly ICache<ITreeNode, ICustomParametersPresenter> _parameterPresenterCache;
      private ICustomParametersPresenter _activePresenter;
      private ParameterGroupingMode _parameterGroupingMode;
      private IContainer _container;
      private IReadOnlyCollection<IParameter> _allVisibleParameters;
      private readonly ITreeNode _allGroupNode;
      private readonly ITreeNode _favoriteNode;
      private readonly ITreeNode _userDefinedNode;
      private readonly ICache<ParameterGroupingMode, IEnumerable<ITreeNode>> _nodesCache;
      private ParameterGroupsPresenterSettings _settings;
      public string NoSelectionCaption { get; set; }

      public ParameterGroupsPresenter(IParameterGroupsView view, IParameterGroupTask parameterGroupTask,
         IParameterGroupNodeCreator groupNodeCreator,
         IParameterContainerToTreeNodeMapper containerNodeMapper,
         INodeToCustomizableParametersPresenterMapper parametersPresenterMapper,
         INoItemInSelectionPresenter noItemInSelectionPresenter,
         ITreeNodeFactory treeNodeFactory, IGroupRepository groupRepository, ICoreUserSettings userSettings,
         IPresentationSettingsTask presentationSettingsTask, ITreeNodeContextMenuFactory treeNodeContextMenuFactory)
         : base(view)
      {
         _parameterGroupTask = parameterGroupTask;
         _groupNodeCreator = groupNodeCreator;
         _containerNodeMapper = containerNodeMapper;
         _parametersPresenterMapper = parametersPresenterMapper;
         _noItemInSelectionPresenter = noItemInSelectionPresenter;
         _groupRepository = groupRepository;
         _userSettings = userSettings;
         _presentationSettingsTask = presentationSettingsTask;
         _treeNodeContextMenuFactory = treeNodeContextMenuFactory;
         _parameterPresenterCache = new Cache<ITreeNode, ICustomParametersPresenter>();
         _nodesCache = new Cache<ParameterGroupingMode, IEnumerable<ITreeNode>>();
         _allGroupNode = treeNodeFactory.CreateGroupAll();
         _favoriteNode = treeNodeFactory.CreateGroupFavorites();
         _userDefinedNode = treeNodeFactory.CreateGroupUserDefined();
      }

      public void ActivateNode(ITreeNode node)
      {
         if (node == null) return;

         var alreadyLoaded = _parameterPresenterCache.Contains(node);
         _activePresenter = presenterFor(node);
         _settings.SelectedNodeId = node.Id;

         //this needs to be done before editing the parameters
         _view.ActivateView(_activePresenter.BaseView);

         if (alreadyLoaded && !_activePresenter.AlwaysRefresh) return;

         _activePresenter.Edit(allVisibleParametersIn(node));
      }

      public IEnumerable<IParameter> AllParametersInSelectedGroup => _activePresenter?.EditedParameters ?? Enumerable.Empty<IParameter>();

      public void RefreshActivePresenter() => _activePresenter?.Edit(_activePresenter.EditedParameters);

      private ICustomParametersPresenter presenterFor(ITreeNode node)
      {
         if (!_parameterPresenterCache.Contains(node))
         {
            var hasParameters = allVisibleParametersIn(node).Any();
            var presenter = _parametersPresenterMapper.MapFrom(node);
            if (presenter != null && (hasParameters || presenter.ForcesDisplay))
            {
               presenter.InitializeWith(CommandCollector);
               presenter.StatusChanged += OnStatusChanged;
               presenter.Description = descriptionFor(node);
            }
            else
            {
               presenter = _noItemInSelectionPresenter;
               presenter.Description = NoSelectionCaption;
            }

            _parameterPresenterCache.Add(node, presenter);
         }

         return _parameterPresenterCache[node];
      }

      private string descriptionFor(ITreeNode node)
      {
         var parameterGroupNode = node as ITreeNode<IGroup>;
         if (parameterGroupNode == null)
            return string.Empty;

         var group = parameterGroupNode.Tag;
         return group == null ? string.Empty : group.Description;
      }

      private IEnumerable<IParameter> allVisibleParametersIn(ITreeNode node)
      {
         return allParametersIn(node).OrderBy(x => x.Sequence);
      }

      private IEnumerable<IParameter> allParametersIn(ITreeNode node)
      {
         if (node == _favoriteNode)
            return _allVisibleParameters;

         if (node == _userDefinedNode)
            return _allVisibleParameters;

         if (node == _allGroupNode)
            return _allVisibleParameters;

         var container = containerFrom(node);
         if (container != null)
            return container.AllVisibleParameters();

         return _parameterGroupTask.ParametersIn(parameterGroupFrom(node), _allVisibleParameters);
      }

      public void InitializeWith(IContainer container)
      {
         InitializeWith(container, x => true);
      }

      public void InitializeWith(IContainer container, IEnumerable<IParameter> allParameters)
      {
         _container = container;
         _allVisibleParameters = allParameters.Where(p => p.Visible).ToList();
         _parameterPresenterCache.Clear();
         _nodesCache.Clear();
         LoadSettingsForSubject(container);
         ParameterGroupingMode = _parameterGroupingMode;
         _view.BindToGroupingMode(this);
      }

      public void InitializeWith(IContainer container, Func<IParameter, bool> predicate)
      {
         InitializeWith(container, container.GetAllChildren(predicate));
      }

      public void ShowContextMenu(ITreeNode treeNode, Point popupLocation)
      {
         var contextMenu = _treeNodeContextMenuFactory.CreateFor(treeNode, this);
         contextMenu.Show(_view, popupLocation);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _parameterPresenterCache.Each(p => p.ReleaseFrom(eventPublisher));
         _parameterPresenterCache.Clear();
         _nodesCache.Clear();
      }

      public ParameterGroupingMode ParameterGroupingMode
      {
         get => _parameterGroupingMode;
         set
         {
            _parameterGroupingMode = value;
            if (_parameterGroupingMode == ParameterGroupingModes.Hierarchical)
               initHierarchyView();
            else if (_parameterGroupingMode == ParameterGroupingModes.Advanced)
               initAdvancedGroupView();
            else if (_parameterGroupingMode == ParameterGroupingModes.Simple)
               initSimpleGroupView();
            else
               throw new ArgumentOutOfRangeException(_parameterGroupingMode.DisplayName);

            _view.AddNodes(_nodesCache[value]);
            _view.SelectNodeById(_settings.SelectedNodeId);
            _settings.ParameterGroupingMode = _parameterGroupingMode;
         }
      }

      private void initHierarchyView()
      {
         if (_nodesCache.Contains(ParameterGroupingModes.Hierarchical))
            return;

         var containerNodes = _containerNodeMapper.MapFrom(_container).Children;
         _nodesCache[ParameterGroupingModes.Hierarchical] = containerNodes;
      }

      private void initAdvancedGroupView()
      {
         if (_nodesCache.Contains(ParameterGroupingModes.Advanced)) return;

         var allTopNodes = defaultNodesFor(_allVisibleParameters);
         allTopNodes.Add(_allGroupNode);

         _nodesCache[ParameterGroupingModes.Advanced] = allTopNodes;
      }

      private void initSimpleGroupView()
      {
         if (_nodesCache.Contains(ParameterGroupingModes.Simple)) return;

         var allSimpleParameters = _allVisibleParameters.Where(x => !_groupRepository.GroupByName(x.GroupName).IsAdvanced);
         var allTopNodes = defaultNodesFor(allSimpleParameters);
         _nodesCache[ParameterGroupingModes.Simple] = allTopNodes;
      }

      private List<ITreeNode> defaultNodesFor(IEnumerable<IParameter> parameters)
      {
         var allTopNodes = parameterGroupsFor(parameters);
         allTopNodes.Insert(0, _favoriteNode);
         allTopNodes.Insert(1, _userDefinedNode);
         return allTopNodes;
      }

      //visible parameters can either be all parameters defined in the top container or only the one not in advanced groups
      private List<ITreeNode> parameterGroupsFor(IEnumerable<IParameter> visibleParameters)
      {
         var allVisibleParameters = visibleParameters.ToList();

         //Initialize groups
         var parameterGroups = _parameterGroupTask.TopGroupsUsedBy(allVisibleParameters)
            .Where(group => group.Visible)
            .OrderBy(group => group.Sequence)
            .ThenBy(group => group.DisplayName);

         return parameterGroups.Select(group => _groupNodeCreator.MapFrom(group, allVisibleParameters)).ToList();
      }

      public IEnumerable<ParameterGroupingMode> AllGroupingModes => ParameterGroupingModes.All();

      private IContainer containerFrom(ITreeNode node)
      {
         var containerNode = node as ITreeNode<IContainer>;
         return containerNode?.Tag;
      }

      private IGroup parameterGroupFrom(ITreeNode node)
      {
         var parameterGroupNode = node as ITreeNode<IGroup>;
         return parameterGroupNode == null ? new Group {Name = "not available"} : parameterGroupNode.Tag;
      }

      public override bool CanClose => _activePresenter == null || _activePresenter.CanClose;

      public void LoadSettingsForSubject(IWithId subject)
      {
         _settings = _presentationSettingsTask.PresentationSettingsFor<ParameterGroupsPresenterSettings>(this, subject);
         _settings.DefaultParameterGroupingModeId = _userSettings.DefaultParameterGroupingMode;
         _parameterGroupingMode = _settings.ParameterGroupingMode;
      }

      public string PresentationKey => PresenterConstants.PresenterKeys.ParameterGroupPresenter;
   }
}