using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;

using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.AdvancedParameters
{
   public interface IAdvancedParametersPresenter : IPresenter,
                                                   IListener<AddAdvancedParameterToContainerEvent>,
                                                   IListener<RemoveAdvancedParameterFromContainerEvent>
   {
      /// <summary>
      ///    Add the selected "non-advanced" parameter as an "advanced" parameter in the edited population
      /// </summary>
      void AddAdvancedParameter();

      /// <summary>
      ///    Remove the selected "advanced" parmeter as a "non-advanced" parameter in the edited population
      /// </summary>
      void RemoveAdvancedParameter();
   }

   public abstract class AdvancedParametersPresenter : AbstractSubPresenter<IAdvancedParametersView, IAdvancedParametersPresenter>,
                                                       IAdvancedParametersPresenter
   {
      protected readonly IEntityPathResolver _entityPathResolver;
      protected readonly IPopulationParameterGroupsPresenter _constantParameterGroupsPresenter;
      protected readonly IPopulationParameterGroupsPresenter _advancedParameterGroupsPresenter;
      private readonly IAdvancedParameterPresenter _advancedParameterPresenter;
      private readonly IAdvancedParametersTask _advancedParametersTask;
      private readonly IEventPublisher _eventPublisher;
      private IAdvancedParameterContainer _advancedParameterContainer;
      private PathCache<IParameter> _parameterCache;

      protected AdvancedParametersPresenter(IAdvancedParametersView view, IEntityPathResolver entityPathResolver,
                                            IPopulationParameterGroupsPresenter constantParameterGroupsPresenter, IPopulationParameterGroupsPresenter advancedParameterGroupsPresenter,
                                            IAdvancedParameterPresenter advancedParameterPresenter, IAdvancedParametersTask advancedParametersTask,
                                            IEventPublisher eventPublisher)
         : base(view)
      {
         _entityPathResolver = entityPathResolver;
         _constantParameterGroupsPresenter = constantParameterGroupsPresenter;
         _constantParameterGroupsPresenter.GroupNodeSelected += (o,e) => deactivateAdd();
         _constantParameterGroupsPresenter.ParameterNodeSelected += (o, e) => activateAdd();
         _advancedParameterGroupsPresenter = advancedParameterGroupsPresenter;
         _advancedParameterPresenter = advancedParameterPresenter;
         _advancedParametersTask = advancedParametersTask;
         _eventPublisher = eventPublisher;
         _advancedParameterGroupsPresenter.GroupNodeSelected += (o,e)=>advancedParameterGroupSelected(e);
         _advancedParameterGroupsPresenter.ParameterNodeSelected += (o, e) => advancedParameterSelected(e);
         _advancedParameterPresenter.OnDistributionTypeChanged += (o, e) => switchAdvancedParameterType(e);
         _advancedParameterGroupsPresenter.EnableFilter = false;
         _view.AddConstantParameterGroupsView(_constantParameterGroupsPresenter.View);
         _view.AddAdvancedParameterGroupsView(_advancedParameterGroupsPresenter.View);
         _view.AddAdvancedParameterView(_advancedParameterPresenter.View);
         _view.AddEnabled = false;
         _view.RemoveEnabled = false;
      }

   

      protected void EditAdvancedParametersFor(IAdvancedParameterContainer advancedParameterContainer, IEnumerable<IParameter> allParameters)
      {
         _advancedParameterContainer = advancedParameterContainer;
         _advancedParameterPresenter.RemoveSelection();
         //cache of all parameters that can be defined as advanced parameters
         _parameterCache = new PathCache<IParameter>(_entityPathResolver).For(advancedFilter(allParameters));
         _constantParameterGroupsPresenter.AddParameters(advancedFilter(advancedParameterContainer.AllConstantParameters(_entityPathResolver)), advancedParameterContainer.DisplayParameterUsingGroupStructure);
         _advancedParameterGroupsPresenter.AddParameters(advancedFilter(advancedParameterContainer.AllAdvancedParameters(_entityPathResolver)), advancedParameterContainer.DisplayParameterUsingGroupStructure);
      }

      private IEnumerable<IParameter> advancedFilter(IEnumerable<IParameter> parameters)
      {
         return parameters.Where(p => p.CanBeDefinedAsAdvanced());
      }

      public override void InitializeWith(ICommandCollector commandCollector)
      {
         base.InitializeWith(commandCollector);
         _advancedParameterPresenter.InitializeWith(commandCollector);
      }

      public void AddAdvancedParameter()
      {
         var parameterNode = _constantParameterGroupsPresenter.SelectedNode as ITreeNode<IParameter>;
         if (parameterNode == null) return;
         AddCommand(_advancedParametersTask.AddAdvancedParameter(parameterNode.Tag, _advancedParameterContainer));
         _advancedParameterGroupsPresenter.SelectParameter(parameterNode.Tag);
         _constantParameterGroupsPresenter.NodeSelected(_constantParameterGroupsPresenter.SelectedNode);
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _advancedParameterPresenter.ReleaseFrom(eventPublisher);
      }

      public void RemoveAdvancedParameter()
      {
         var parameterNode = _advancedParameterGroupsPresenter.SelectedNode as ITreeNode<IParameter>;
         if (parameterNode == null) return;
         AddCommand(_advancedParametersTask.RemoveAdvancedParameter(parameterNode.Tag, _advancedParameterContainer));
         _advancedParameterGroupsPresenter.NodeSelected(_advancedParameterGroupsPresenter.SelectedNode);
      }

      private void advancedParameterSelected(ParameterNodeSelectedEventArgs e)
      {
         var parameterNode = e.Node;
         if (parameterNode == null) return;
         _view.RemoveEnabled = true;
         var advancedParameter = _advancedParameterContainer.AdvancedParameterFor(_entityPathResolver, parameterNode.Tag);
         advancedParameter.FullDisplayName = parameterNode.FullPath();
         _advancedParameterPresenter.Edit(advancedParameter);
         _eventPublisher.PublishEvent(new AdvancedParameteSelectedEvent(_advancedParameterContainer, advancedParameter));
      }

      private void advancedParameterGroupSelected(NodeSelectedEventArgs e)
      {
         _view.RemoveEnabled = false;
         _advancedParameterPresenter.RemoveSelection();
      }

      private void switchAdvancedParameterType(DistributionTypeChangedEventArgs e)
      {
         AddCommand(_advancedParametersTask.SwitchDistributionTypeFor(parameterFrom(e.AdvancedParameter), _advancedParameterContainer, e.DistributionType));
         _advancedParameterPresenter.Edit(e.AdvancedParameter);
      }

      private void activateAdd()
      {
         _view.AddEnabled = true;
      }

      private void deactivateAdd()
      {
         _view.AddEnabled = false;
      }

      public void Handle(AddAdvancedParameterToContainerEvent eventToHandle)
      {
         var parameter = parameterFrom(eventToHandle);
         if (parameter == null) return;
         _constantParameterGroupsPresenter.RemoveParameter(parameter);
         _advancedParameterGroupsPresenter.AddParameter(parameter);
      }

      public void Handle(RemoveAdvancedParameterFromContainerEvent eventToHandle)
      {
         var parameter = parameterFrom(eventToHandle);
         if (parameter == null) return;
         _advancedParameterGroupsPresenter.RemoveParameter(parameter);
         _constantParameterGroupsPresenter.AddParameter(parameter);
         _advancedParameterPresenter.RemoveSelectionFor(eventToHandle.AdvancedParameter);
      }

      private IParameter parameterFrom(IAdvancedParameterEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return null;
         return parameterFrom(eventToHandle.AdvancedParameter);
      }

      private IParameter parameterFrom(AdvancedParameter advancedParameter)
      {
         return _parameterCache[advancedParameter.ParameterPath];
      }

      private bool canHandle(IAdvancedParameterEvent advancedParameterEvent)
      {
         return Equals(advancedParameterEvent.AdvancedParameterContainer, _advancedParameterContainer);
      }
   }
}