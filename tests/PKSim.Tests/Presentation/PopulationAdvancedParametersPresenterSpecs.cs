using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.AdvancedParameters;

using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.AdvancedParameters;

using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAdvancedParametersPresenter : ContextSpecification<IPopulationAdvancedParametersPresenter>
   {
      protected IAdvancedParametersView _view;
      protected IPopulationParameterGroupsPresenter _allConstantParameterGroupsPresenter;
      protected IPopulationParameterGroupsPresenter _advancedParameterGroupsPresenter;
      protected IAdvancedParameterPresenter _advancedParameterPresenter;
      protected List<IParameter> _allConstantParameters;
      protected RandomPopulation _population;
      protected List<IParameter> _allAdvancedParameters;
      protected IEntityPathResolver _entityPathResolver;
      protected IAdvancedParametersTask _advancedParametersTask;
      protected ICommandCollector _commandRegister;
      protected List<IParameter> _allParameters;
      protected IParameter _para1;
      protected IParameter _para2;
      protected IParameter _advancedPara1;
      protected string _pathPara1;
      protected string _pathPara2;
      protected string _pathAdvancedPara1;
      protected IRepresentationInfoRepository _representationInfoRepository;
      private IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _view = A.Fake<IAdvancedParametersView>();
         _allConstantParameterGroupsPresenter = A.Fake<IPopulationParameterGroupsPresenter>();
         _advancedParameterGroupsPresenter = A.Fake<IPopulationParameterGroupsPresenter>();
         _advancedParameterPresenter = A.Fake<IAdvancedParameterPresenter>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _advancedParametersTask = A.Fake<IAdvancedParametersTask>();
         _eventPublisher = A.Fake<IEventPublisher>();
         A.CallTo(()=>_allConstantParameterGroupsPresenter.View).Returns( A.Fake<IPopulationParameterGroupsView>());
         A.CallTo(()=>_advancedParameterGroupsPresenter.View).Returns( A.Fake<IPopulationParameterGroupsView>());
         A.CallTo(()=>_advancedParameterPresenter.View).Returns( A.Fake<IAdvancedParameterView>());
         _population =  A.Fake<RandomPopulation>();
         _commandRegister =  A.Fake<ICommandCollector>();
         _allConstantParameters = new List<IParameter>();
         _allAdvancedParameters = new List<IParameter>();
         _allParameters = new List<IParameter>();
         A.CallTo(()=>_population.AllIndividualParameters()).Returns(_allParameters);
         A.CallTo(()=>_population.AllConstantParameters(_entityPathResolver)).Returns(_allConstantParameters);
         A.CallTo(()=>_population.AllAdvancedParameters(_entityPathResolver)).Returns(_allAdvancedParameters);
         _para1 =  A.Fake<IParameter>().WithName("_para1");
         _para1.Visible = true;
         _para1.CanBeVaried = true;
         _para1.CanBeVariedInPopulation = true;
         _para1.Editable = true;
         _para2 =  A.Fake<IParameter>().WithName("_para2");
         _para2.Editable = true;
         _para2.Visible = true;
         _para2.CanBeVaried = true;
         _para2.CanBeVariedInPopulation = true;
         _advancedPara1 = A.Fake<IParameter>().WithName("_advancedPara1");
         _advancedPara1.Visible = true;
         _advancedPara1.Editable = true;
         _advancedPara1.CanBeVaried = true;
         _advancedPara1.CanBeVariedInPopulation = true;
         _pathPara1 = "_pathPara1";
         _pathPara2 = "_pathPara2";
         _pathAdvancedPara1 = "_pathAdvancedPara1";
         _allParameters.AddRange(new[] {_para1, _para2, _advancedPara1});
         _allConstantParameters.AddRange(new[] {_para1, _para2});
         _allAdvancedParameters.AddRange(new[] {_advancedPara1});
         A.CallTo(()=>_entityPathResolver.PathFor(_para1)).Returns(_pathPara1);
         A.CallTo(()=>_entityPathResolver.PathFor(_para2)).Returns(_pathPara2);
         A.CallTo(() => _entityPathResolver.PathFor(_advancedPara1)).Returns(_pathAdvancedPara1);
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new PopulationAdvancedParametersPresenter(_view, _entityPathResolver, _allConstantParameterGroupsPresenter, _advancedParameterGroupsPresenter, _advancedParameterPresenter,
                                                         _advancedParametersTask, _eventPublisher);
         sut.InitializeWith(_commandRegister);
      }
   }

   public class When_creating_the_advanced_parameters_presenter : concern_for_PopulationAdvancedParametersPresenter
   {
      [Observation]
      public void the_presenter_should_add_the_views_from_its_sub_presenter_into_its_own_view()
      {
         A.CallTo(() => _view.AddConstantParameterGroupsView(_allConstantParameterGroupsPresenter.View)).MustHaveHappened();
         A.CallTo(() => _view.AddAdvancedParameterGroupsView(_advancedParameterGroupsPresenter.View)).MustHaveHappened();
      }
   }

   public class When_the_advanced_parameter_presenter_is_told_to_edit_a_population : concern_for_PopulationAdvancedParametersPresenter
   {
      protected override void Because()
      {
         sut.EditPopulation(_population);
      }

      [Observation]
      public void should_retrieve_the_constant_parameters_from_the_population_and_display_them_as_available_parameters()
      {
         A.CallTo(() => _allConstantParameterGroupsPresenter.AddParameters(A<IEnumerable<IParameter>>._,true)).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_the_advanced_parmaeters_from_the_population_and_display_them_in_the_advanced_parameters_section()
      {
         A.CallTo(() => _advancedParameterGroupsPresenter.AddParameters(A<IEnumerable<IParameter>>._, true)).MustHaveHappened();
      }

      [Observation]
      public void should_also_clear_any_previously_selected_advanced_parameters()
      {
         A.CallTo(() => _advancedParameterPresenter.RemoveSelection()).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_notified_that_an_advanced_parameter_group_was_selected : concern_for_PopulationAdvancedParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         _advancedParameterGroupsPresenter.GroupNodeSelected += Raise.With(new NodeSelectedEventArgs(A.Fake<ITreeNode>())); 
      }

      [Observation]
      public void should_disable_the_remove_button()
      {
         _view.RemoveEnabled.ShouldBeFalse();
      }
   }

   public class When_the_presenter_is_notified_that_an_advanced_parameter_was_selected : concern_for_PopulationAdvancedParametersPresenter
   {
      private ITreeNode<IParameter> _treeNode;
      private IParameter _parameter;
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         sut.EditPopulation(_population);
         _advancedParameter =  new  AdvancedParameter();
         _parameter = new PKSimParameter().WithName("tralala");
         _treeNode = new ObjectWithIdAndNameNode<IParameter>(_parameter);
         A.CallTo(() => _population.AdvancedParameterFor(_entityPathResolver, _parameter)).Returns(_advancedParameter);
      }

      protected override void Because()
      {
         _advancedParameterGroupsPresenter.ParameterNodeSelected += Raise.With(new ParameterNodeSelectedEventArgs(_treeNode));
      }

      [Observation]
      public void should_enable_the_remove_button()
      {
         _view.RemoveEnabled.ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_display_name_of_the_advanced_parameter_to_the_full_path_of_the_node()
      {
         _advancedParameter.FullDisplayName.ShouldBeEqualTo(_treeNode.FullPath());
      }
   }

   public class When_the_presenter_is_notified_that_an_available_parameter_group_was_selected : concern_for_PopulationAdvancedParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         _advancedParameterGroupsPresenter.GroupNodeSelected += Raise.With(new NodeSelectedEventArgs(A.Fake<ITreeNode>()));
      }

      [Observation]
      public void should_disable_the_add_button()
      {
         _view.AddEnabled.ShouldBeFalse();
      }
   }

   public class When_the_presenter_is_notified_that_an_available_parameter_was_selected : concern_for_PopulationAdvancedParametersPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         _allConstantParameterGroupsPresenter.ParameterNodeSelected += Raise.With(new ParameterNodeSelectedEventArgs(A.Fake<ITreeNode<IParameter>>()));
      }

      [Observation]
      public void should_enable_the_add_button()
      {
         _view.AddEnabled.ShouldBeTrue();
      }
   }

   public class When_the_advanced_parameter_presenter_is_told_to_add_the_selected_parameter_as_a_distributed_parameter : concern_for_PopulationAdvancedParametersPresenter
   {
      private ITreeNode<IParameter> _parameterNode;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter =  A.Fake<IParameter>();
         _parameterNode =  A.Fake<ITreeNode<IParameter>>();
        A.CallTo(() =>  _parameterNode.Tag).Returns(_parameter);
        A.CallTo(() => _allConstantParameterGroupsPresenter.SelectedNode).Returns(_parameterNode);
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.AddAdvancedParameter();
      }

      [Observation]
      public void should_add_the_parameter_as_advanced_parameter_in_the_population()
      {
         A.CallTo(() => _advancedParametersTask.AddAdvancedParameter(_parameter, _population)).MustHaveHappened();
      }

      [Observation]
      public void should_select_the_advanced_parameters_and_display_its_distribution()
      {
         A.CallTo(() => _advancedParameterGroupsPresenter.SelectParameter(_parameter)).MustHaveHappened();
      }
   }

   public class When_the_advanced_parameter_presenter_is_told_to_remove_the_selected_advanced_parameter : concern_for_PopulationAdvancedParametersPresenter
   {
      private ITreeNode<IParameter> _parameterNode;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter =  A.Fake<IParameter>();
         _parameterNode =  A.Fake<ITreeNode<IParameter>>();
         A.CallTo(() => _parameterNode.Tag).Returns(_parameter);
         A.CallTo(() => _advancedParameterGroupsPresenter.SelectedNode).Returns(_parameterNode);
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.RemoveAdvancedParameter();
      }

      [Observation]
      public void should_remove_the_parameter_from_the_advanced_parameters_defined_in_the_population()
      {
         A.CallTo(() => _advancedParametersTask.RemoveAdvancedParameter(_parameter, _population)).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_being_notified_that_an_advanced_parameter_was_added_to_a_population_that_is_not_the_one_being_edited : concern_for_PopulationAdvancedParametersPresenter
   {
      private RandomPopulation _anotherPopulation;
      private AdvancedParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _anotherPopulation =  A.Fake<RandomPopulation>();
         _parameter =  A.Fake<AdvancedParameter>();
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.Handle(new AddAdvancedParameterToContainerEvent {Container = _anotherPopulation, Entity = _parameter});
      }

      [Observation]
      public void should_not_add_any_parameter_to_the_advanced_parameters_presenter()
      {
         A.CallTo(() => _advancedParameterGroupsPresenter.AddParameter(A<IParameter>._)).MustNotHaveHappened(); 
      }

      [Observation]
      public void should_not_remove_any_parameter_from_the_constant_parameters_presenter()
      {
         A.CallTo(() => _allConstantParameterGroupsPresenter.RemoveParameter(A<IParameter>._)).MustNotHaveHappened();
      }
   }

   public class When_the_presenter_is_being_notified_that_an_advanced_parameter_was_added_to_the_population_being_edited : concern_for_PopulationAdvancedParametersPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter =  A.Fake<AdvancedParameter>();
         //in this scenario, para1 was selected to become and advanced parameter
         _advancedParameter.ParameterPath = _pathPara1;
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.Handle(new AddAdvancedParameterToContainerEvent {Container = _population, Entity = _advancedParameter});
      }

      [Observation]
      public void should_add_the_parameter_to_the_advanced_parameters_presenter()
      {
         A.CallTo(() => _advancedParameterGroupsPresenter.AddParameter(_para1)).MustHaveHappened();
      }

      [Observation]
      public void should_remove_the_parameter_from_the_constant_parameters_presenter()
      {
         A.CallTo(() => _allConstantParameterGroupsPresenter.RemoveParameter(_para1)).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_being_notified_that_an_advanced_parameter_was_removed_from_a_population_that_is_not_the_one_being_edited : concern_for_PopulationAdvancedParametersPresenter
   {
      private RandomPopulation _anotherPopulation;
      private AdvancedParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _anotherPopulation =  A.Fake<RandomPopulation>();
         _parameter = new AdvancedParameter();
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveAdvancedParameterFromContainerEvent {Container = _anotherPopulation, Entity = _parameter});
      }

      [Observation]
      public void should_not_remove_any_parameter_from_the_advanced_parameters_presenter()
      {
         A.CallTo(() => _advancedParameterGroupsPresenter.RemoveParameter(A<IParameter>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_add_any_parameter_to_the_constant_parameters_presenter()
      {
         A.CallTo(() => _allConstantParameterGroupsPresenter.AddParameter(A<IParameter>._)).MustNotHaveHappened();
      }
   }

   public class When_the_presenter_is_being_notified_that_an_advanced_parameter_was_removed_from_the_population_being_edited : concern_for_PopulationAdvancedParametersPresenter
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = new AdvancedParameter();
         //in this scenario, _advancedPara1 is being deleted from the advanced parameters
         _advancedParameter.ParameterPath = _pathAdvancedPara1;
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveAdvancedParameterFromContainerEvent {Container = _population, Entity = _advancedParameter});
      }

      [Observation]
      public void should_remove_the_parameter_from_the_advanced_parameters_presenter()
      {
         A.CallTo(() => _advancedParameterGroupsPresenter.RemoveParameter(_advancedPara1)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_parameter_to_the_constant_parameters_presenter()
      {
         A.CallTo(() => _allConstantParameterGroupsPresenter.AddParameter(_advancedPara1)).MustHaveHappened();
      }

      [Observation]
      public void should_remove_the_advanced_parameter_selection_if_the_parmaeter_was_selected()
      {
         A.CallTo(() => _advancedParameterPresenter.RemoveSelectionFor(_advancedParameter)).MustHaveHappened();
      }
   }

   public class When_an_advanced_parameter_is_being_selected : concern_for_PopulationAdvancedParametersPresenter
   {
      private AdvancedParameter _advancedParameter;
      private ITreeNode<IParameter> _parameterNode;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter =  A.Fake<IParameter>();
         _parameterNode =  A.Fake<ITreeNode<IParameter>>();
         A.CallTo(()=>_parameterNode.Tag).Returns(_parameter);
         _advancedParameter = new AdvancedParameter();
         sut.EditPopulation(_population);
         A.CallTo(() => _population.AdvancedParameterFor(_entityPathResolver, _parameter)).Returns(_advancedParameter);
      }

      protected override void Because()
      {
         _advancedParameterGroupsPresenter.ParameterNodeSelected += Raise.With(new ParameterNodeSelectedEventArgs(_parameterNode));
      }

      [Observation]
      public void should_update_the_advanced_parameter_presenter_with_the_selected_advanced_parameter()
      {
         A.CallTo(() => _advancedParameterPresenter.Edit(_advancedParameter)).MustHaveHappened();
      }
   }

   public class When_an_advanced_parameterg_group_is_being_selected : concern_for_PopulationAdvancedParametersPresenter
   {
      private AdvancedParameter _advancedParameter;
      private IParameter _parameter;
      private ITreeNode _node;

      protected override void Context()
      {
         base.Context();
         _parameter =  A.Fake<IParameter>();
         _node =  A.Fake<ITreeNode>();
         _advancedParameter =  new AdvancedParameter();
         sut.EditPopulation(_population);
         A.CallTo(() => _population.AdvancedParameterFor(_entityPathResolver, _parameter)).Returns(_advancedParameter);
      }

      
      protected override void Because()
      {
         _advancedParameterGroupsPresenter.GroupNodeSelected += Raise.With(new NodeSelectedEventArgs(null));
      }

      [Observation]
      public void should_remove_any_information_displayed_previously()
      {
         A.CallTo(() => _advancedParameterPresenter.RemoveSelection()).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_being_notified_that_the_distribution_type_of_an_advanced_presenter_needs_to_be_changed : concern_for_PopulationAdvancedParametersPresenter
   {
      private AdvancedParameter _advancedParameter;
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _command =  A.Fake<IPKSimCommand>();
         _advancedParameter =  new AdvancedParameter();
         _advancedParameter.ParameterPath = _pathAdvancedPara1;
         A.CallTo(() => _advancedParametersTask.SwitchDistributionTypeFor(_advancedPara1, _population, DistributionTypes.Normal)).Returns(_command);
         sut.EditPopulation(_population);
      }

      protected override void Because()
      {
        _advancedParameterPresenter.OnDistributionTypeChanged += Raise.With(new DistributionTypeChangedEventArgs(_advancedParameter, DistributionTypes.Normal));
      }

      [Observation]
      public void should_change_the_type_of_the_advanced_presenter_and_register_the_command()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_being_initialized_with_a_command_register : concern_for_PopulationAdvancedParametersPresenter
   {
      protected override void Because()
      {
         sut.InitializeWith(_commandRegister);
      }

      [Observation]
      public void should_also_set_the_command_register_in_the_advanced_parameter_presenter()
      {
         A.CallTo(() => _advancedParameterPresenter.InitializeWith(_commandRegister)).MustHaveHappened();
      }
   }
}