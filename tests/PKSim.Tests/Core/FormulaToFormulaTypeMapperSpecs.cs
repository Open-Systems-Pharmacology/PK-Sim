using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_formula_to_formula_type_mapper : ContextSpecification<IFormulaToFormulaTypeMapper>
   {
      protected override void Context()
      {
         sut = new FormulaToFormulaTypeMapper();
      }
   }

   
   public class When_mapping_a_distribution_formula : concern_for_formula_to_formula_type_mapper
   {
      private FormulaType _result;

      protected override void Because()
      {
         _result = sut.MapFrom(A.Fake<IDistributionFormula>());
      }

      [Observation]
      public void the_returned_formula_type_shouyld_be_distribution()
      {
         _result.ShouldBeEqualTo(FormulaType.Distribution);
      }
   }

   
   public class When_mapping_a_constant_formula : concern_for_formula_to_formula_type_mapper
   {
      private FormulaType _result;

      protected override void Because()
      {
         _result = sut.MapFrom(new ConstantFormula());
      }

      [Observation]
      public void the_returned_formula_type_shouyld_be_constant()
      {
         _result.ShouldBeEqualTo(FormulaType.Constant);
      }
   }

   
   public class When_mapping_a_rate_formula_without_dependent_parameters : concern_for_formula_to_formula_type_mapper
   {
      private FormulaType _result;
      private IFormula _formula;

      protected override void Context()
      {
         base.Context();
         _formula = A.Fake<IFormula>();
         A.CallTo(() => _formula.ObjectPaths).Returns(new List<FormulaUsablePath> {A.Fake<FormulaUsablePath>()});
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_formula);
      }

      [Observation]
      public void the_returned_formula_type_shouyld_be_rate()
      {
         _result.ShouldBeEqualTo(FormulaType.Rate);
      }
   }

   
   public class When_mapping_a_null_formula : concern_for_formula_to_formula_type_mapper
   {
      private FormulaType _result;

      protected override void Because()
      {
         _result = sut.MapFrom(null);
      }

      [Observation]
      public void the_returned_formula_type_shouyld_be_constant()
      {
         _result.ShouldBeEqualTo(FormulaType.Constant);
      }
   }
}