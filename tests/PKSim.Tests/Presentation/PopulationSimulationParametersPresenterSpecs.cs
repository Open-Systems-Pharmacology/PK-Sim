using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;


namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationSimulationParametersPresenter : ContextSpecification<IPopulationSimulationParametersPresenter>
   {
      private ISimulationParametersView _view;
      protected IParameterGroupsPresenter _parameterGroupPresenter;

      protected override void Context()
      {
         _view = A.Fake<ISimulationParametersView>();
         _parameterGroupPresenter = A.Fake<IParameterGroupsPresenter>();
         sut = new PopulationSimulationParametersPresenter(_view, _parameterGroupPresenter);
      }
   }

   public class When_editing_a_population_simulation : concern_for_PopulationSimulationParametersPresenter
   {
      private PopulationSimulation _populationSimulation;
      private List<IParameter> _allParameters;
      private IParameter _compoundParameter;
      private IParameter _simulationVariableParameter;
      private IParameter _simulationNotVariableParameter;
      private IParameter _eventParameter;
      private IParameter _protocolParameter;
      private IParameter _formulationParameter;
      private IEnumerable<IParameter> _displayedParameters;
      private IParameter _individualParameter;
      private IParameter _volumeParameter;
      private IParameter _expressionParameter;

      protected override void Context()
      {
         base.Context();
         _allParameters = new List<IParameter>();

         _compoundParameter = create(PKSimBuildingBlockType.Compound);
         _individualParameter = create(PKSimBuildingBlockType.Individual);
         _simulationVariableParameter = create(PKSimBuildingBlockType.Simulation);
         _simulationVariableParameter.CanBeVaried = true;
         _simulationVariableParameter.CanBeVariedInPopulation = true;
         _simulationNotVariableParameter = create(PKSimBuildingBlockType.Simulation);
         _eventParameter = create(PKSimBuildingBlockType.Event);
         _protocolParameter = create(PKSimBuildingBlockType.Protocol);
         _formulationParameter = create(PKSimBuildingBlockType.Formulation);
         _volumeParameter = create(PKSimBuildingBlockType.Simulation).WithName(Constants.Parameters.VOLUME);
         _expressionParameter = create(PKSimBuildingBlockType.Simulation).WithName(CoreConstants.Parameter.REL_EXP);

         _allParameters.Add(_compoundParameter);
         _allParameters.Add(_simulationVariableParameter);
         _allParameters.Add(_simulationNotVariableParameter);
         _allParameters.Add(_eventParameter);
         _allParameters.Add(_protocolParameter);
         _allParameters.Add(_formulationParameter);
         _allParameters.Add(_volumeParameter);
         _allParameters.Add(_expressionParameter);
         _populationSimulation = A.Fake<PopulationSimulation>();
         _populationSimulation.Model = A.Fake<IModel>();
         A.CallTo(() => _parameterGroupPresenter.InitializeWith(A<IContainer>._, A<IEnumerable<IParameter>>._))
            .Invokes(x => _displayedParameters = x.GetArgument<IEnumerable<IParameter>>(1));

         A.CallTo(() => _populationSimulation.All<IParameter>()).Returns(_allParameters);
      }

      private IParameter create(PKSimBuildingBlockType buildingBlockType)
      {
         var parameter = A.Fake<IParameter>();
         parameter.CanBeVaried = false;
         parameter.CanBeVariedInPopulation = false;
         A.CallTo(() => parameter.BuildingBlockType).Returns(buildingBlockType);
         return parameter;
      }

      protected override void Because()
      {
         sut.EditSimulation(_populationSimulation);
      }

      [Observation]
      public void should_display_the_compound_parameters()
      {
         _displayedParameters.ShouldContain(_compoundParameter);
      }

      [Observation]
      public void should_display_the_formulation_parameters()
      {
         _displayedParameters.ShouldContain(_formulationParameter);
      }

      [Observation]
      public void should_display_the_event_parameters()
      {
         _displayedParameters.ShouldContain(_eventParameter);
      }

      [Observation]
      public void should_display_the_protocol_parameters()
      {
         _displayedParameters.ShouldContain(_protocolParameter);
      }

      [Observation]
      public void should_display_the_simulation_parameters_that_are_not_variable()
      {
         _displayedParameters.ShouldContain(_simulationNotVariableParameter);
      }

      [Observation]
      public void should_not_display_the_simulation_parameters_that_are_variable()
      {
         _displayedParameters.Contains(_simulationVariableParameter).ShouldBeFalse();
      }

      [Observation]
      public void should_not_display_the_individual_parameters()
      {
         _displayedParameters.Contains(_individualParameter).ShouldBeFalse();
      }

      [Observation]
      public void should_not_display_simulation_parameters_named_volume()
      {
         _displayedParameters.Contains(_volumeParameter).ShouldBeFalse();
      }

      [Observation]
      public void should_not_display_simulation_expression_parameters()
      {
         _displayedParameters.Contains(_expressionParameter).ShouldBeFalse();
      }
   }
}