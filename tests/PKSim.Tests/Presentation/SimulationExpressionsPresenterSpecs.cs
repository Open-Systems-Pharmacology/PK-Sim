using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using IParameter = OSPSuite.Core.Domain.IParameter;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationExpressionsPresenter : ContextSpecification<ISimulationExpressionsPresenter>
   {
      protected ISimulationExpressionsView _view;
      protected IExpressionParametersToSimulationExpressionsDTOMapper _simulationExpressionsDTOMapper;
      protected IEditParameterPresenterTask _editParameterPresenterTask;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IEntityPathResolver _entityPathResolver;
      protected IParameterTask _parameterTask;
      protected IMultiParameterEditPresenter _moleculeParametersPresenter;
      protected List<IParameter> _allParameters;
      private PathCache<IParameter> _pathCache;
      protected SimulationExpressionsDTO _simulationExpressionDTO;
      protected IParameter _propertyParameter;
      protected IParameter _relativeExpressionParameter;
      protected ICommandCollector _commandCollector;

      protected override void Context()
      {
         _view = A.Fake<ISimulationExpressionsView>();
         _simulationExpressionsDTOMapper = A.Fake<IExpressionParametersToSimulationExpressionsDTOMapper>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterTask = A.Fake<IParameterTask>();
         _moleculeParametersPresenter = A.Fake<IMultiParameterEditPresenter>();
         _commandCollector= A.Fake<ICommandCollector>();
         sut = new SimulationExpressionsPresenter(_view, _simulationExpressionsDTOMapper, _editParameterPresenterTask, _moleculeExpressionTask, _entityPathResolver, _parameterTask, _moleculeParametersPresenter);

         sut.InitializeWith(_commandCollector);

         _simulationExpressionDTO = new SimulationExpressionsDTO(new ParameterDTO(_propertyParameter), new ParameterDTO(_propertyParameter), new ParameterDTO(_propertyParameter), 
            new List<ExpressionContainerDTO>());

         _propertyParameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName("PROP");
         _relativeExpressionParameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName("REL_EXP");
         _allParameters = new List<IParameter> {_propertyParameter, _relativeExpressionParameter};
         _pathCache = new PathCacheForSpecs<IParameter>();  

         A.CallTo(() => _parameterTask.PathCacheFor(A<IEnumerable<IParameter>>.That.Matches(x=>x.ContainsAll(_allParameters)))).Returns(_pathCache);
         A.CallTo(() => _simulationExpressionsDTOMapper.MapFrom(A<IEnumerable<IParameter>>.That.Matches(x => x.ContainsAll(_allParameters)))).Returns(_simulationExpressionDTO);
      }
   }

   public class When_creting_the_simulation_expression_presenter : concern_for_SimulationExpressionsPresenter
   {
      [Observation]
      public void it_should_not_always_refresh()
      {
         sut.AlwaysRefresh.ShouldBeFalse();
      }

      [Observation]
      public void it_should_not_force_display()
      {
         sut.ForcesDisplay.ShouldBeFalse();         
      }
   }

   public class When_the_simulation_expression_presenter_is_editing_a_set_of_parameters : concern_for_SimulationExpressionsPresenter
   {
      protected override void Because()
      {
         sut.Edit(_allParameters);
      }

      [Observation]
      public void should_retrieve_a_simulation_expression_dto_and_binding_it_to_the_view()
      {
         A.CallTo(() => _view.BindTo(_simulationExpressionDTO)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_all_global_molecule_parameters()
      {
         A.CallTo(() => _moleculeParametersPresenter.Edit(_simulationExpressionDTO.MoleculeParameters)).MustHaveHappened();
      }

   }

   public class When_the_value_of_a_relative_expression_parameter_is_set_in_the_simulation_expression_presenter : concern_for_SimulationExpressionsPresenter
   {
      private ExpressionContainerDTO _expressionContainerDTO;
      private double _value=5;
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<ICommand>();
         _expressionContainerDTO = new ExpressionContainerDTO {RelativeExpressionParameter = new ParameterDTO(_relativeExpressionParameter)};
         A.CallTo(() => _moleculeExpressionTask.SetRelativeExpressionFor(_relativeExpressionParameter, _value)).Returns(_command);
      }

      protected override void Because()
      {
         sut.SetRelativeExpression(_expressionContainerDTO, _value);
      }

      [Observation]
      public void should_leverage_the_relative_expression_command_to_update_the_value()
      {
         A.CallTo(() => _moleculeExpressionTask.SetRelativeExpressionFor(_relativeExpressionParameter,_value)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_command_to_the_history()
      {
         A.CallTo(() => _commandCollector.AddCommand(_command)).MustHaveHappened();
      }
   }
}