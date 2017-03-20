using OSPSuite.Core.Domain.Formulas;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_parameter_to_formula_mapper : ContextSpecification<IParameterToFomulaTypeMapper>
   {
      protected IFormulaToFormulaTypeMapper _formulaTypeMapper;

      protected override void Context()
      {
         _formulaTypeMapper = A.Fake<IFormulaToFormulaTypeMapper>();
         sut = new ParameterToFomulaTypeMapper(_formulaTypeMapper);
      }
   }

   
   public class When_retrieving_the_formula_type_from_a_parameter : concern_for_parameter_to_formula_mapper
   {
      private FormulaType _formulaType;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _formulaType = FormulaType.Distribution;
         _parameter = A.Fake<IParameter>();
         _parameter.Formula = A.Fake<IFormula>();
         A.CallTo(() => _formulaTypeMapper.MapFrom(_parameter.Formula)).Returns(_formulaType);
      }
      [Observation]
      public void should_return_constant_if_the_parameter_is_not_defined()
      {
         sut.MapFrom(null).ShouldBeEqualTo(FormulaType.Constant);
      }

      [Observation]
      public void should_return_the_type_of_the_formula_is_defined()
      {
         sut.MapFrom(_parameter).ShouldBeEqualTo(_formulaType);

      }

   }
}	