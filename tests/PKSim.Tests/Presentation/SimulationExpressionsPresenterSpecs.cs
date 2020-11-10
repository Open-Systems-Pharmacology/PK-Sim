using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using PKSim.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationExpressionsPresenter : ContextSpecification<ISimulationExpressionsPresenter>
   {
      protected ISimulationExpressionsView _view;
      protected IExpressionParametersToSimulationExpressionsDTOMapper _simulationExpressionsDTOMapper;
      protected IMultiParameterEditPresenter _moleculeParametersPresenter;
      protected List<IParameter> _allParameters;
      protected SimulationExpressionsDTO _simulationExpressionDTO;
      protected IParameter _propertyParameter;
      protected IParameter _relativeExpressionParameter;
      protected ICommandCollector _commandCollector;
      protected IExpressionParametersPresenter _expressionParametersPresenter;

      protected override void Context()
      {
         _view = A.Fake<ISimulationExpressionsView>();
         _simulationExpressionsDTOMapper = A.Fake<IExpressionParametersToSimulationExpressionsDTOMapper>();
         _moleculeParametersPresenter = A.Fake<IMultiParameterEditPresenter>();
         _commandCollector = A.Fake<ICommandCollector>();
         _expressionParametersPresenter = A.Fake<IExpressionParametersPresenter>();
         sut = new SimulationExpressionsPresenter(_view, _simulationExpressionsDTOMapper, _moleculeParametersPresenter,
            _expressionParametersPresenter);

         sut.InitializeWith(_commandCollector);

         _simulationExpressionDTO = new SimulationExpressionsDTO(new ParameterDTO(_propertyParameter), new ParameterDTO(_propertyParameter),
            new ParameterDTO(_propertyParameter),
            new List<ExpressionParameterDTO>());

         _propertyParameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName("PROP");
         _relativeExpressionParameter = DomainHelperForSpecs.ConstantParameterWithValue().WithName("REL_EXP");
         _allParameters = new List<IParameter> {_propertyParameter, _relativeExpressionParameter};
         A.CallTo(() => _simulationExpressionsDTOMapper.MapFrom(A<IEnumerable<IParameter>>.That.Matches(x => x.ContainsAll(_allParameters))))
            .Returns(_simulationExpressionDTO);
      }
   }

   public class When_creating_the_simulation_expression_presenter : concern_for_SimulationExpressionsPresenter
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
         A.CallTo(() => _expressionParametersPresenter.Edit(_simulationExpressionDTO.ExpressionParameters)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_all_global_molecule_parameters()
      {
         A.CallTo(() => _moleculeParametersPresenter.Edit(_simulationExpressionDTO.MoleculeParameters)).MustHaveHappened();
      }
   }
}