using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using IDistributionFormulaFactory = PKSim.Core.Model.IDistributionFormulaFactory;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using ParameterFactory = PKSim.Core.Model.ParameterFactory;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterFactory : ContextSpecification<IParameterFactory>
   {
      protected IPKSimObjectBaseFactory _objectBaseFactory;
      protected IDistributionFormulaFactory _distributionFactory;
      protected IFormulaFactory _formulaFactory;
      protected IDimensionRepository _dimensionRepository;
      protected IDimension _dimension;
      protected IDisplayUnitRetriever _displayUnitRetriever;

      protected override void Context()
      {
         _dimension = A.Fake<IDimension>();
         _dimension.DefaultUnit = A.Fake<Unit>();
         _objectBaseFactory = A.Fake<IPKSimObjectBaseFactory>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _displayUnitRetriever = A.Fake<IDisplayUnitRetriever>();
         sut = new ParameterFactory(_objectBaseFactory, _formulaFactory, _dimensionRepository, _displayUnitRetriever);
      }
   }

   public class When_creating_a_parameter_from_a_parameter_value_definition : concern_for_ParameterFactory
   {
      private ParameterValueMetaData _parameterValueDefinition;
      private IParameter _result;
      private IFormula _valueFormula;
      private IParameter _parameter;
      private Unit _displayUnit;

      protected override void Context()
      {
         base.Context();
         _parameterValueDefinition = new ParameterValueMetaData();
         _parameterValueDefinition.ParameterName = "tralal";
         _valueFormula = A.Fake<IFormula>();
         _parameter = A.Fake<IParameter>();
         _displayUnit= A.Fake<Unit>();
         A.CallTo(() => _formulaFactory.ValueFor(_parameterValueDefinition)).Returns(_valueFormula);
         A.CallTo(() => _objectBaseFactory.CreateParameter()).Returns(_parameter);
         A.CallTo(() => _dimensionRepository.DimensionByName(_parameterValueDefinition.Dimension)).Returns(_dimension);
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(_parameter)).Returns(_displayUnit);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_parameterValueDefinition);
      }

      [Observation]
      public void should_leverage_the_entity_factory_to_create_a_new_parameter()
      {
         _result.ShouldBeEqualTo(_parameter);
      }

      [Observation]
      public void should_leverage_the_dimension_factory_to_set_the_accurate_dimension_for_the_parameter()
      {
         _result.Dimension.ShouldBeEqualTo(_dimension);
      }
      
      [Observation]
      public void should_use_the_display_unit_using_the_default_display_unit_defined_for_the_parameter()
      {
         _result.DisplayUnit.ShouldBeEqualTo(_displayUnit);
      }
   }

   public class When_creating_a_parameter_from_a_parameter_rate_definition : concern_for_ParameterFactory
   {
      private ParameterRateMetaData _parameterRateDefinition;
      private IParameter _result;
      private IFormula _rateFormula;
      private IParameter _parameter;
      private IFormulaCache _formulaCache;

      protected override void Context()
      {
         base.Context();
         _formulaCache = A.Fake<IFormulaCache>();
         _parameterRateDefinition = new ParameterRateMetaData();
         _parameterRateDefinition.ParameterName = "Tralal";
         _rateFormula = A.Fake<IFormula>();
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _formulaFactory.RateFor(_parameterRateDefinition, _formulaCache)).Returns(_rateFormula);
         A.CallTo(() => _objectBaseFactory.CreateParameter()).Returns(_parameter);
         A.CallTo(() => _dimensionRepository.DimensionByName(_parameterRateDefinition.Dimension)).Returns(_dimension);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_parameterRateDefinition, _formulaCache);
      }

      [Observation]
      public void should_leverage_the_entity_factory_to_create_a_new_parameter()
      {
         _result.ShouldBeEqualTo(_parameter);
      }

      [Observation]
      public void should_leverage_the_formula_factory_to_set_the_accurate_rate_to_the_parameter()
      {
         _result.Formula.ShouldBeEqualTo(_rateFormula);
      }
   }

   public class When_creating_a_parameter_from_a_set_of_parameter_distribution_definition : concern_for_ParameterFactory
   {
      private IDistributedParameter _result;
      private IDistributionFormula _distributionFormula;
      private IDistributedParameter _parameter;
      private OriginData _originData;
      private readonly IList<ParameterDistributionMetaData> _distributions = new List<ParameterDistributionMetaData>();
      private IParameter _subParameter;

      protected override void Context()
      {
         base.Context();
         _distributionFormula = A.Fake<IDistributionFormula>();
         _parameter = A.Fake<IDistributedParameter>();
         _subParameter = A.Fake<IParameter>();
         _originData = new OriginData();
         _distributions.Add(new ParameterDistributionMetaData {DistributionType = CoreConstants.Distribution.Normal});
         A.CallTo(() => _formulaFactory.DistributionFor(A<IEnumerable<ParameterDistributionMetaData>>._, _parameter, _originData)).Returns(_distributionFormula);
         A.CallTo(() => _objectBaseFactory.CreateDistributedParameter()).Returns(_parameter);
         A.CallTo(() => _objectBaseFactory.CreateParameter()).Returns(_subParameter);
         A.CallTo(() => _objectBaseFactory.Create<ExplicitFormula>()).Returns(new ExplicitFormula());
         A.CallTo(() => _dimensionRepository.DimensionByName(A<string>.Ignored)).Returns(_dimension);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_distributions, _originData);
      }

      [Observation]
      public void should_leverage_the_entity_factory_to_create_a_new_parameter()
      {
         _result.ShouldBeEqualTo(_parameter);
      }

      [Observation]
      public void should_leverage_the_formula_factory_to_set_the_accurate_rate_to_the_parameter()
      {
         _result.Formula.ShouldBeEqualTo(_distributionFormula);
      }

      [Observation]
      public void should_add_a_mean_parameter_a_std_parameter_and_a_percentile_parameter_to_the_distributed_parameter()
      {
         A.CallTo(() => _result.Add(A<IEntity>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(3));
      }
   }

   public class When_creating_a_parameter_from_a_parameter_name : concern_for_ParameterFactory
   {
      private string _parameterName;
      private IParameter _result;
      private IFormula _formula;

      protected override void Context()
      {
         base.Context();
         _parameterName = "tralala";
         _formula = A.Fake<IFormula>();
         A.CallTo(() => _objectBaseFactory.CreateParameter()).Returns(new PKSimParameter());
         A.CallTo(() => _dimensionRepository.DimensionByName(A<string>.Ignored)).Returns(_dimension);
         A.CallTo(() => _formulaFactory.ValueFor(A<ParameterValueMetaData>.Ignored)).Returns(_formula);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_parameterName, PKSimBuildingBlockType.Individual);
      }

      [Observation]
      public void the_returned_parameter_shoudld_have_a_min_of_zero_and_an_undefined_max()
      {
         _result.MinValue.ShouldBeEqualTo(0);
         _result.MaxValue.ShouldBeNull();
      }

      [Observation]
      public void the_returned_parameter_shoudld_have_a_value_set_to_the_default_value()
      {
         _result.Formula.ShouldBeEqualTo(_formula);
      }
   }
}