using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand : ContextSpecification<EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand>
   {
      protected IndividualMolecule _molecule;
      private QueryExpressionResults _result;
      private ISimulationSubject _simulationSubject;
      protected IExecutionContext _context;
      protected readonly List<ExpressionResult> _expressionResults = new List<ExpressionResult>();
      protected readonly Cache<string, IParameter> _allExpressionParameters = new Cache<string, IParameter>();
      protected IndividualProtein _protein;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         var expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         (_molecule, _) = expressionProfile;
         _protein = _molecule.DowncastTo<IndividualProtein>();
         _result = new QueryExpressionResults(_expressionResults);
         _simulationSubject = A.Fake<ISimulationSubject>();
         sut = new EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand(_molecule, _result, _simulationSubject);

         A.CallTo(() => _simulationSubject.AllExpressionParametersFor(_molecule)).Returns(_allExpressionParameters);
      }
   }

   public class When_setting_the_relative_expression_for_a_molecule_that_is_not_localized_in_blood_cells_and_the_blood_cells_expression_is_greater_than_zero : concern_for_EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand
   {
      protected override void Context()
      {
         base.Context();
         _allExpressionParameters.Add(CoreConstants.Compartment.BLOOD_CELLS, DomainHelperForSpecs.ConstantParameterWithValue(1));
         _expressionResults.Add(new ExpressionResult {ContainerName = CoreConstants.Compartment.BLOOD_CELLS, RelativeExpression = 2});
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_BLOOD_CELLS));
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_turn_on_blood_cell_intracellular_localization()
      {
         _protein.InBloodCells.ShouldBeTrue();
         _protein.IsBloodCellsIntracellular.ShouldBeTrue();
      }
   }

   public class When_setting_the_relative_expression_for_a_molecule_that_is_localized_in_blood_cells_and_the_blood_cells_expression_is_greater_than_zero : concern_for_EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand
   {
      protected override void Context()
      {
         base.Context();
         _protein.IsBloodCellsMembrane = true;
         _allExpressionParameters.Add(CoreConstants.Compartment.BLOOD_CELLS, DomainHelperForSpecs.ConstantParameterWithValue(1));
         _expressionResults.Add(new ExpressionResult {ContainerName = CoreConstants.Compartment.BLOOD_CELLS, RelativeExpression = 2});
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_BLOOD_CELLS));
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_not_turn_on_blood_cell_intracellular_localization()
      {
         _protein.InBloodCells.ShouldBeTrue();
         _protein.IsBloodCellsIntracellular.ShouldBeFalse();
      }
   }

   public class When_setting_the_relative_expression_for_a_molecule_that_is_not_localized_in_vascular_endothelium_and_the_vasc_endothelium_expression_is_greater_than_zero : concern_for_EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand
   {
      protected override void Context()
      {
         base.Context();
         _allExpressionParameters.Add(CoreConstants.Compartment.VASCULAR_ENDOTHELIUM, DomainHelperForSpecs.ConstantParameterWithValue(1));
         _expressionResults.Add(new ExpressionResult {ContainerName = CoreConstants.Compartment.VASCULAR_ENDOTHELIUM, RelativeExpression = 2});
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_VASC_ENDO_PLASMA_SIDE));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME));
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_turn_on_vascular_endothelium_localization()
      {
         _protein.InVascularEndothelium.ShouldBeTrue();
         _protein.IsVascEndosome.ShouldBeTrue();
      }
   }

   public class When_setting_the_relative_expression_for_a_molecule_that_is_localized_in_vascular_endothelium_and_the_vasc_endothelium_expression_is_greater_than_zero : concern_for_EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand
   {
      protected override void Context()
      {
         base.Context();
         _protein.IsVascMembranePlasmaSide = true;
         _allExpressionParameters.Add(CoreConstants.Compartment.VASCULAR_ENDOTHELIUM, DomainHelperForSpecs.ConstantParameterWithValue(1));
         _expressionResults.Add(new ExpressionResult {ContainerName = CoreConstants.Compartment.VASCULAR_ENDOTHELIUM, RelativeExpression = 2});
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_not_turn_on_vascular_endosome()
      {
         _protein.InVascularEndothelium.ShouldBeTrue();
         _protein.IsVascEndosome.ShouldBeFalse();
      }
   }
}