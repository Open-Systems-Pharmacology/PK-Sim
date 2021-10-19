using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using FormulaFactory = PKSim.Core.Model.FormulaFactory;
using IDistributionFormulaFactory = PKSim.Core.Model.IDistributionFormulaFactory;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_FormulaFactory : ContextSpecification<IFormulaFactory>
   {
      protected IRateObjectPathsRepository _rateObjectPathsRepository;
      protected IRateFormulaRepository _rateFormulaRepository;
      protected IDistributionFormulaFactory _distributionFactory;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IObjectPathFactory _objectPathFactory;
      private IDimensionRepository _dimensionRepository;
      protected IDimension _timeDimension;
      protected IIdGenerator _idGenerator;
      protected IDynamicFormulaCriteriaRepository _dynamicFormulaCriteriaRepo;
      protected RateKey _dynamicSumFormulaRateKey;
      protected DescriptorCriteria _sumFormulaCriteria;

      protected override void Context()
      {
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _rateObjectPathsRepository = A.Fake<IRateObjectPathsRepository>();
         _rateFormulaRepository = A.Fake<IRateFormulaRepository>();
         _distributionFactory = A.Fake<IDistributionFormulaFactory>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _timeDimension = A.Fake<IDimension>();
         _idGenerator = A.Fake<IIdGenerator>();
         A.CallTo(() => _objectBaseFactory.Create<ExplicitFormula>()).Returns(new ExplicitFormula());
         A.CallTo(() => _objectBaseFactory.Create<ConstantFormula>()).Returns(new ConstantFormula());
         A.CallTo(() => _objectBaseFactory.Create<SumFormula>()).Returns(new SumFormula());
         A.CallTo(() => _dimensionRepository.Time).Returns(_timeDimension);
         _objectPathFactory = new ObjectPathFactoryForSpecs();
         _dynamicFormulaCriteriaRepo = A.Fake<IDynamicFormulaCriteriaRepository>();

         _dynamicSumFormulaRateKey = new RateKey(CoreConstants.CalculationMethod.DYNAMIC_SUM_FORMULAS, "SomeFormula");

         _sumFormulaCriteria = new DescriptorCriteria();
         _sumFormulaCriteria.Add(new MatchTagCondition("xxx"));
         _sumFormulaCriteria.Add(new NotMatchTagCondition("yyy"));

         A.CallTo(() => _dynamicFormulaCriteriaRepo.CriteriaFor(_dynamicSumFormulaRateKey)).Returns(_sumFormulaCriteria);
         A.CallTo(() => _rateFormulaRepository.FormulaFor(_dynamicSumFormulaRateKey)).Returns("P_#i");
         sut = new FormulaFactory(_objectBaseFactory, _rateObjectPathsRepository, _rateFormulaRepository, _distributionFactory,
            _objectPathFactory, _dimensionRepository, _idGenerator, _dynamicFormulaCriteriaRepo);
      }
   }

   public class When_creating_dynamic_sum_formula : concern_for_FormulaFactory
   {
      private IFormula _formula;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(_dynamicSumFormulaRateKey)).Returns(new List<IFormulaUsablePath>());
      }

      protected override void Because()
      {
         _formula = sut.RateFor(_dynamicSumFormulaRateKey, null);
      }

      [Observation]
      public void should_create_dynamic_sum_formula()
      {
         _formula.IsAnImplementationOf<SumFormula>().ShouldBeTrue();
      }

      [Observation]
      public void should_copy_criteria()
      {
         _formula.DowncastTo<DynamicFormula>().Criteria.ShouldOnlyContain(_sumFormulaCriteria);
      }
   }

   public class When_createing_a_formula_from_a_predefined_rate_definition_corresponding_to_a_formula_that_already_exists : concern_for_FormulaFactory
   {
      private ParameterRateMetaData _rateDefinition1;
      private ParameterRateMetaData _rateDefinition2;
      private IFormula _result1;
      private IFormula _result2;
      private IFormulaCache _formulaCache;

      protected override void Context()
      {
         base.Context();
         _rateDefinition1 = new ParameterRateMetaData {Rate = "toto", CalculationMethod = "tutu"};
         _rateDefinition2 = new ParameterRateMetaData {Rate = "toto", CalculationMethod = "tutu"};

         _formulaCache = new FormulaCache();
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(new RateKey(_rateDefinition1.CalculationMethod, _rateDefinition1.Rate))).Returns(new List<IFormulaUsablePath>());

         A.CallTo(() => _rateFormulaRepository.FormulaFor(new RateKey(_rateDefinition1.CalculationMethod, _rateDefinition1.Rate))).Returns("0");
      }

      protected override void Because()
      {
         _result1 = sut.RateFor(_rateDefinition1, _formulaCache);
         _result2 = sut.RateFor(_rateDefinition2, _formulaCache);
      }

      [Observation]
      public void should_return_an_existing_formula_if_the_formula_already_exists()
      {
         _result1.ShouldBeEqualTo(_result2);
      }
   }

   public class When_creating_a_formula_for_a_predefined_rate_definition_for_a_building_block_in_pksim : concern_for_FormulaFactory
   {
      private ParameterRateMetaData _rateDefinition;
      private IFormula _result;

      protected override void Context()
      {
         base.Context();
         _rateDefinition = new ParameterRateMetaData {Rate = "toto", CalculationMethod = "tutu"};
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(new RateKey(_rateDefinition.CalculationMethod, _rateDefinition.Rate))).Returns(new List<IFormulaUsablePath>());
         A.CallTo(() => _rateFormulaRepository.FormulaFor(new RateKey(_rateDefinition.CalculationMethod, _rateDefinition.Rate))).Returns("0");
         A.CallTo(() => _idGenerator.NewId()).Returns("newId");
      }

      protected override void Because()
      {
         _result = sut.RateFor(_rateDefinition, null);
      }

      [Observation]
      public void should_always_return_a_new_formula_with_a_new_id()
      {
         _result.Id.ShouldBeEqualTo("newId");
      }
   }

   public class When_generating_a_formula_for_a_parameter_value_definition : concern_for_FormulaFactory
   {
      private ConstantFormula _result;
      private ParameterValueMetaData _parameterValueDefinition;
      private double _value;

      protected override void Context()
      {
         base.Context();
         _value = 5;
         _parameterValueDefinition = new ParameterValueMetaData {DefaultValue = _value};
      }

      protected override void Because()
      {
         _result = sut.ValueFor(_parameterValueDefinition) as ConstantFormula;
      }

      [Observation]
      public void should_return_a_formula_whose_value_is_the_parameter_value()
      {
         _result.ShouldNotBeNull();
         _result.Value.ShouldBeEqualTo(_value);
      }
   }

   public class When_creating_a_formula_for_a_rate_definition_that_was_never_created_yet : concern_for_FormulaFactory
   {
      private ParameterRateMetaData _rateDefinition;
      private IFormula _formula;
      private IFormulaUsablePath _objectPath1;
      private IFormulaUsablePath _objectPath2;
      private string _formulaString;

      protected override void Context()
      {
         base.Context();
         _objectPath1 = A.Fake<IFormulaUsablePath>();
         _objectPath2 = A.Fake<IFormulaUsablePath>();
         _rateDefinition = new ParameterRateMetaData {Rate = "toto", CalculationMethod = "tutu"};
         var rateKey = new RateKey(_rateDefinition.CalculationMethod, _rateDefinition.Rate);
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(rateKey)).Returns(new[] {_objectPath1, _objectPath2});
         _formulaString = "3*4";
         A.CallTo(() => _rateFormulaRepository.FormulaFor(rateKey)).Returns(_formulaString);
      }

      protected override void Because()
      {
         _formula = sut.RateFor(_rateDefinition, new FormulaCache());
      }

      [Observation]
      public void should_set_the_list_of_all_object_paths_used_to_describe_the_formula()
      {
         _formula.ObjectPaths.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_retrieve_the_formula_for_the_rate_and_calculation_method()
      {
         _formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo(_formulaString);
      }
   }

   public class When_creating_a_formula_for_a_rate_definition_representing_a_constant_parameter : concern_for_FormulaFactory
   {
      private ParameterRateMetaData _rateDefinition;
      private IFormula _formula;
      private string _formulaString;

      protected override void Context()
      {
         base.Context();
         _rateDefinition = new ParameterRateMetaData {Rate = "toto", CalculationMethod = "tutu"};
         var rateKey = new RateKey(_rateDefinition.CalculationMethod, _rateDefinition.Rate);
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(rateKey)).Returns(Enumerable.Empty<IFormulaUsablePath>());
         _formulaString = "10";
         A.CallTo(() => _rateFormulaRepository.FormulaFor(rateKey)).Returns(_formulaString);
      }

      protected override void Because()
      {
         _formula = sut.RateFor(_rateDefinition, new FormulaCache());
      }

      [Observation]
      public void should_have_retrieved_the_formula_as_a_consant_formula()
      {
         _formula.IsConstant().ShouldBeTrue();
      }

      [Observation]
      public void the_constant_formula_value_should_be_equal_to_the_value_of_the_formula()
      {
         _formula.DowncastTo<ConstantFormula>().Value.ShouldBeEqualTo(10);
      }
   }

   public class When_creating_a_concentration_formula_to_be_added_in_a_formula_cache_containing_already_the_formula_for_concentration : concern_for_FormulaFactory
   {
      private IFormula _exisitingFormula;
      private IFormulaCache _formulaCache;

      protected override void Context()
      {
         base.Context();
         _exisitingFormula = A.Fake<IFormula>().WithId(CoreConstants.Formula.Concentration);
         _formulaCache = new FormulaCache {_exisitingFormula};
      }

      [Observation]
      public void should_return_the_predefined_formula()
      {
         sut.ConcentrationFormulaFor(_formulaCache).ShouldBeEqualTo(_exisitingFormula);
      }
   }

   public class When_creating_a_concentration_formula_to_be_added_in_a_formula_cache_which_does_not_contain_the_formula_for_concentration : concern_for_FormulaFactory
   {
      private IFormulaCache _formulaCache;
      private IFormula _concentrationFormula;

      protected override void Context()
      {
         base.Context();
         _formulaCache = new FormulaCache();
      }

      protected override void Because()
      {
         _concentrationFormula = sut.ConcentrationFormulaFor(_formulaCache);
      }

      [Observation]
      public void should_create_a_new_concentration_formula_and_add_it_to_the_formula_cache()
      {
         _formulaCache.Contains(_concentrationFormula).ShouldBeTrue();
      }
   }

   public class When_creating_time_independent_formula : concern_for_FormulaFactory
   {
      private RateKey _rateKey;
      private IFormulaCache _formulaCache;
      private IFormula _formula;

      protected override void Context()
      {
         base.Context();

         _rateKey = new RateKey("CM1", "Rate1");

         _formulaCache = new FormulaCache();
         A.CallTo(() => _rateFormulaRepository.FormulaFor(_rateKey)).Returns("x+y");
         A.CallTo(() => _objectBaseFactory.Create<ExplicitFormula>()).Returns(new ExplicitFormula());
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(_rateKey)).Returns(
            new List<IFormulaUsablePath> {new FormulaUsablePath(new[] {"x"})});
      }

      protected override void Because()
      {
         _formula = sut.RateFor(_rateKey, _formulaCache);
      }

      [Observation]
      public void time_path_should_not_be_added_by_formula_factory()
      {
         _formula.ContainsTimePath().ShouldBeFalse();
      }
   }

   public abstract class concern_for_time_dependent_formulas : concern_for_FormulaFactory
   {
      protected RateKey _rateKey;
      protected IFormulaCache _formulaCache;
      protected IFormula _formula;

      protected override void Context()
      {
         base.Context();

         _rateKey = new RateKey("CM1", "Rate1");

         _formulaCache = new FormulaCache();
         A.CallTo(() => _rateFormulaRepository.FormulaFor(_rateKey)).Returns("Time * 2");
         A.CallTo(() => _objectBaseFactory.Create<ExplicitFormula>()).Returns(new ExplicitFormula());
         A.CallTo(() => _rateObjectPathsRepository.ObjectPathsFor(_rateKey)).Returns(
            new List<IFormulaUsablePath>());
      }
   }

   public class When_creating_time_dependent_formula_without_time_path : concern_for_time_dependent_formulas
   {
      protected override void Because()
      {
         _formula = sut.RateFor(_rateKey, _formulaCache);
      }

      [Observation]
      public void time_path_should_be_added_by_formula_factory()
      {
         _formula.ContainsTimePath().ShouldBeTrue();
      }
   }

   public class When_creating_time_dependent_formula_with_time_path : concern_for_time_dependent_formulas
   {
      protected override void Because()
      {
         _formula = sut.RateFor(_rateKey, _formulaCache);
         _formula.AddObjectPath(_objectPathFactory.CreateTimePath(_timeDimension));
      }

      [Observation]
      public void time_path_should_not_be_added_by_formula_factory()
      {
         _formula.ObjectPaths.Count().ShouldNotBeEqualTo(1);
         _formula.ContainsTimePath().ShouldBeTrue(); //predefined
      }
   }
}