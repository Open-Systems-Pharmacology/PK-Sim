﻿using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualProteinExpressionsPresenter : ContextSpecification<IIndividualProteinExpressionsPresenter>
   {
      protected IIndividualProteinExpressionsView _view;
      protected IEditParameterPresenterTask _parameterTask;
      protected IIndividualProteinToIndividualProteinDTOMapper _individualProteinMapper;
      protected IIndividualMoleculePropertiesPresenter<Individual> _moleculesPropertiesPresenter;
      protected IExpressionLocalizationPresenter<Individual> _expressionLocalizationPresenter;
      protected Individual _individual;
      protected IndividualEnzyme _enzyme;
      protected IndividualProteinDTO _enzymeDTO;
      protected ExpressionParameterDTO _initialConcentration;
      protected ExpressionParameterDTO _relativeExpression;
      protected ExpressionParameterDTO _relativeExpression2;
      protected ExpressionParameterDTO _fraction_exp_bc;
      protected List<ExpressionParameterDTO> _allParameters;

      private ExpressionParameterDTO createParameter(string parameterName)
      {
         return new ExpressionParameterDTO
            {Parameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(parameterName))};
      }

      protected override void Context()
      {
         _view = A.Fake<IIndividualProteinExpressionsView>();
         _parameterTask = A.Fake<IEditParameterPresenterTask>();
         _individualProteinMapper = A.Fake<IIndividualProteinToIndividualProteinDTOMapper>();
         _moleculesPropertiesPresenter = A.Fake<IIndividualMoleculePropertiesPresenter<Individual>>();
         _expressionLocalizationPresenter = A.Fake<IExpressionLocalizationPresenter<Individual>>();

         sut = new IndividualEnzymeExpressionsPresenter<Individual>(_view, _parameterTask, _individualProteinMapper, _moleculesPropertiesPresenter,
            _expressionLocalizationPresenter);


         _individual = new Individual();
         sut.SimulationSubject = _individual;


         _enzyme = new IndividualEnzyme().WithName("CYP3A4");

         _enzymeDTO = new IndividualProteinDTO(_enzyme);
         _initialConcentration = createParameter(CoreConstants.Parameters.INITIAL_CONCENTRATION);
         _relativeExpression = createParameter(CoreConstants.Parameters.REL_EXP);
         _relativeExpression2 = createParameter(CoreConstants.Parameters.REL_EXP);
         _fraction_exp_bc = createParameter(CoreConstants.Parameters.FRACTION_EXPRESSED_BLOOD_CELLS);
         _enzymeDTO.AddExpressionParameter(_initialConcentration);
         _enzymeDTO.AddExpressionParameter(_relativeExpression);
         _enzymeDTO.AddExpressionParameter(_fraction_exp_bc);
         A.CallTo(() => _individualProteinMapper.MapFrom(_individual, _enzyme)).Returns(_enzymeDTO);

         A.CallTo(() => _view.BindTo(A<IEnumerable<ExpressionParameterDTO>>._))
            .Invokes(x => _allParameters = x.GetArgument<IEnumerable<ExpressionParameterDTO>>(0).ToList());
      }
   }

   public class When_editing_an_enzyme_defined_in_a_given_individual : concern_for_IndividualProteinExpressionsPresenter
   {
      protected override void Because()
      {
         sut.ActivateMolecule(_enzyme);
      }

      [Observation]
      public void should_retrieves_all_expression_parameters_from_the_individual_defined_for_the_enzyme_and_display_them_in_the_view()
      {
         _allParameters.ShouldOnlyContain(_initialConcentration, _relativeExpression, _fraction_exp_bc);
      }

      [Observation]
      public void should_edit_the_molecule_localization()
      {
         A.CallTo(() => _expressionLocalizationPresenter.Edit(_enzyme, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_molecule_ontogeny()
      {
         A.CallTo(() => _moleculesPropertiesPresenter.Edit(_enzyme, _individual)).MustHaveHappened();

      }
   }

   public class When_switching_the_visibility_of_initial_concentration_parameters_off : concern_for_IndividualProteinExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.ActivateMolecule(_enzyme);
      }

      protected override void Because()
      {
         sut.ShowInitialConcentration = false;
      }

      [Observation]
      public void should_hide_concentration_parameters()
      {
         _allParameters.ShouldOnlyContain(_relativeExpression, _fraction_exp_bc);
      }
   }

   public class When_setting_the_value_of_an_expression_parameter : concern_for_IndividualProteinExpressionsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _enzymeDTO.AddExpressionParameter(_relativeExpression2);

         _relativeExpression.Parameter.Parameter.Value = 5;
         _relativeExpression2.Parameter.Parameter.Value = 10;

         sut.ActivateMolecule(_enzyme);
         //Make sure that we in fact update the value of the parameter as the command will not be executed
         A.CallTo(() => _parameterTask.SetParameterValue(sut, _relativeExpression.Parameter, 20))
            .Invokes(x => _relativeExpression.Parameter.Parameter.Value = 20);
      }

      protected override void Because()
      {
         sut.SetExpressionParameterValue(_relativeExpression.Parameter, 20);
      }

      [Observation]
      public void should_also_calculate_the_normalized_value()
      {
         _relativeExpression.NormalizedExpressionPercent.ShouldBeEqualTo(100);
         _relativeExpression2.NormalizedExpressionPercent.ShouldBeEqualTo(50);
      }
   }
}