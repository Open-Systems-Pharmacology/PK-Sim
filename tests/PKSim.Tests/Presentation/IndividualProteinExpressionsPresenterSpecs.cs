using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualProteinExpressionsPresenter : ContextSpecification<IndividualEnzymeExpressionsPresenter<Individual>>
   {
      protected IIndividualProteinExpressionsView _view;
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
      protected IExpressionParametersPresenter _expressionParametersPresenter;

      private ExpressionParameterDTO createParameter(string parameterName)
      {
         return new ExpressionParameterDTO
            {Parameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(parameterName))};
      }

      protected override void Context()
      {
         _view = A.Fake<IIndividualProteinExpressionsView>();
         _individualProteinMapper = A.Fake<IIndividualProteinToIndividualProteinDTOMapper>();
         _moleculesPropertiesPresenter = A.Fake<IIndividualMoleculePropertiesPresenter<Individual>>();
         _expressionLocalizationPresenter = A.Fake<IExpressionLocalizationPresenter<Individual>>();
         _expressionParametersPresenter = A.Fake<IExpressionParametersPresenter>();

         sut = new IndividualEnzymeExpressionsPresenter<Individual>(
            _view, _individualProteinMapper,
            _moleculesPropertiesPresenter,
            _expressionLocalizationPresenter,
            _expressionParametersPresenter);


         _individual = new Individual();
         sut.SimulationSubject = _individual;


         _enzyme = new IndividualEnzyme().WithName("CYP3A4");

         _enzymeDTO = new IndividualProteinDTO(_enzyme);
         _initialConcentration = createParameter(INITIAL_CONCENTRATION);
         _relativeExpression = createParameter(REL_EXP);
         _relativeExpression2 = createParameter(REL_EXP);
         _fraction_exp_bc = createParameter(FRACTION_EXPRESSED_BLOOD_CELLS);
         _enzymeDTO.AddExpressionParameter(_initialConcentration);
         _enzymeDTO.AddExpressionParameter(_relativeExpression);
         _enzymeDTO.AddExpressionParameter(_fraction_exp_bc);
         A.CallTo(() => _individualProteinMapper.MapFrom(_enzyme, _individual)).Returns(_enzymeDTO);
         A.CallTo(() => _expressionParametersPresenter.Edit(A<IReadOnlyList<ExpressionParameterDTO>>._))
            .Invokes(x => _allParameters = x.GetArgument<IReadOnlyList<ExpressionParameterDTO>>(0).ToList());


         sut.ActivateMolecule(_enzyme);
      }
   }

   public class When_editing_an_enzyme_defined_in_a_given_individual : concern_for_IndividualProteinExpressionsPresenter
   {
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

      [Observation]
      public void should_update_the_expression_parameters()
      {
         A.CallTo(() => _expressionParametersPresenter.Edit(_enzymeDTO.AllExpressionParameters)).MustHaveHappened();
      }
   }

   public class when_checking_the_visibility_of_an_expression_parameter_in_vascular_system : concern_for_IndividualProteinExpressionsPresenter
   {
      protected ExpressionParameterDTO _expressionParameterDTO;
      protected IParameterDTO _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameterDTO>();
         _expressionParameterDTO = new ExpressionParameterDTO {Parameter = _parameter, GroupName = CoreConstants.Groups.VASCULAR_SYSTEM};
      }

      [Observation]
      public void should_return_visible_if_the_parameter_is_rel_exp_blood_cells_and_the_enzyme_is_expressed_in_blood_cells()
      {
         _enzyme.IsBloodCellsMembrane = true;
         _parameter.Name = REL_EXP_BLOOD_CELLS;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_hidden_if_the_parameter_is_rel_exp_blood_cells_and_the_enzyme_is_not_expressed_in_blood_cells()
      {
         _enzyme.IsBloodCellsMembrane = false;
         _parameter.Name = REL_EXP_BLOOD_CELLS;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();
      }

      [Observation]
      public void should_return_visible_if_the_parameter_is_fraction_exp_blood_cells_and_the_enzyme_is_expressed_in_all_blood_cells_locations()
      {
         _enzyme.IsBloodCellsMembrane = true;
         _enzyme.IsBloodCellsIntracellular = true;
         _parameter.Name = FRACTION_EXPRESSED_BLOOD_CELLS;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeTrue();

         _parameter.Name = FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_hidden_if_the_parameter_is_fraction_exp_blood_cells_and_the_enzyme_is_not_expressed_in_all_blood_cells_locations()
      {
         _enzyme.IsBloodCellsMembrane = true;
         _enzyme.IsBloodCellsIntracellular = false;
         _parameter.Name = FRACTION_EXPRESSED_BLOOD_CELLS;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();

         _parameter.Name = FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();
      }


      [Observation]
      public void should_return_visible_if_the_parameter_is_rel_exp_vasc_endo_and_the_enzyme_is_expressed_in_vasc_endothelium()
      {
         _enzyme.IsVascEndosome = true;
         _parameter.Name = REL_EXP_VASCULAR_ENDOTHELIUM;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_hidden_if_the_parameter_is_rel_exp_vasc_endo_and_the_enzyme_is_not_expressed_in_vasc_endothelium()
      {
         _enzyme.IsVascEndosome = false;
         _parameter.Name = REL_EXP_VASCULAR_ENDOTHELIUM;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();
      }


      [Observation]
      public void should_return_visible_if_the_parameter_is_fraction_exp_vasc_end_and_the_enzyme_is_expressed_in_all_vasc_end_locations()
      {
         _enzyme.IsVascMembraneTissueSide = true;
         _enzyme.IsVascEndosome = true;
         _parameter.Name = FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeTrue();

         _parameter.Name = FRACTION_EXPRESSED_VASC_ENDO_TISSUE_SIDE;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeTrue();


      }

      [Observation]
      public void should_return_hidden_if_the_parameter_is_fraction_exp_vasc_end_and_the_enzyme_is_not_expressed_in_all_vasc_end_locations()
      {
         _enzyme.IsVascEndosome = true;
         _enzyme.IsVascMembraneTissueSide = false;
         _enzyme.IsVascMembranePlasmaSide = false;
         _parameter.Name = FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();

         _enzyme.IsVascEndosome = false;
         _enzyme.IsVascMembraneTissueSide = true;
         _enzyme.IsVascMembranePlasmaSide = false;
         _parameter.Name = FRACTION_EXPRESSED_VASC_ENDO_TISSUE_SIDE;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();


         _enzyme.IsVascEndosome = false;
         _enzyme.IsVascMembraneTissueSide = false ;
         _enzyme.IsVascMembranePlasmaSide = true;
         _parameter.Name = FRACTION_EXPRESSED_VASC_ENDO_PLASMA_SIDE;
         sut.IsParameterVisible(_expressionParameterDTO).ShouldBeFalse();
      }
   }
}