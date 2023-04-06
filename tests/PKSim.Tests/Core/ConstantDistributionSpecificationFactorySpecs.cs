using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Maths.Interpolations;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using DistributionType = OSPSuite.Core.Domain.Formulas.DistributionType;
using IDistributionFormulaFactory = OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_DiscreteDistributionSpecificationFactory : ContextSpecification<DiscreteDistributionFormulaSpecificationFactory>
   {
      protected IInterpolation _interpolation;
      protected IDistributionFormulaFactory _distrFormulaFactory;

      protected override void Context()
      {
         _interpolation = A.Fake<IInterpolation>();
         _distrFormulaFactory = A.Fake<IDistributionFormulaFactory>();

         sut = new DiscreteDistributionFormulaSpecificationFactory(_interpolation, _distrFormulaFactory);
      }
   }

   public class When_asked_if_a_set_of_distributions_has_the_type_discrete_distribution : concern_for_DiscreteDistributionSpecificationFactory
   {
      private IEnumerable<ParameterDistributionMetaData> _onlyDiscreteDistributions;
      private IEnumerable<ParameterDistributionMetaData> _mixedDistributions;
      private ParameterDistributionMetaData _discreteDistribution1;
      private ParameterDistributionMetaData _discreteDistribution2;
      private ParameterDistributionMetaData _logNormalDistribution;

      protected override void Context()
      {
         base.Context();
         _discreteDistribution1 = new ParameterDistributionMetaData
         {
            DistributionType = DistributionType.Discrete
         };
         _discreteDistribution2 = new ParameterDistributionMetaData
         {
            DistributionType = DistributionType.Discrete
         };
         _logNormalDistribution = new ParameterDistributionMetaData
         {
            DistributionType = DistributionType.LogNormal
         };
         _onlyDiscreteDistributions = new List<ParameterDistributionMetaData> {_discreteDistribution1, _discreteDistribution2};
         _mixedDistributions = new List<ParameterDistributionMetaData> {_discreteDistribution1, _logNormalDistribution};
      }

      [Observation]
      public void should_return_true_for_a_set_containing_only_normal_distributions()
      {
         sut.IsSatisfiedBy(_onlyDiscreteDistributions).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_set_containing_at_least_one_distribution_which_is_not_normal()
      {
         sut.IsSatisfiedBy(_mixedDistributions).ShouldBeFalse();
      }
   }

   public class When_creating_a_discrete_distribution : concern_for_DiscreteDistributionSpecificationFactory
   {
      protected IEnumerable<ParameterDistributionMetaData> _discreteDistributions;
      protected ParameterDistributionMetaData _discreteDistribution1;
      protected ParameterDistributionMetaData _discreteDistribution2;
      protected DistributionFormula _resultingDistribution;
      protected IDistributedParameter _parameter;
      protected OriginData _originData;

      protected override void Context()
      {
         base.Context();
         _discreteDistribution1 = new ParameterDistributionMetaData {Age = 20, Mean = 5};
         _discreteDistribution2 = new ParameterDistributionMetaData {Age = 10, Mean = 15};
         _discreteDistributions = new List<ParameterDistributionMetaData> {_discreteDistribution1, _discreteDistribution2};
         _parameter = A.Fake<IDistributedParameter>();
         var meanParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("Mean");
         A.CallTo(() => _parameter.MeanParameter).Returns(meanParameter);

         _originData = new OriginData { Age = new OriginDataParameter(12)};
         A.CallTo(() => _distrFormulaFactory.CreateDiscreteDistributionFormulaFor(_parameter, meanParameter)).Returns(new DiscreteDistributionFormula());
      }

      protected override void Because()
      {
         _resultingDistribution = sut.CreateFor(_discreteDistributions, _parameter, _originData);
      }

      [Observation]
      public void should_return_a_distribution_formula_of_type_discrete_distribution()
      {
         _resultingDistribution.IsAnImplementationOf<DiscreteDistributionFormula>().ShouldBeTrue();
      }

      [Observation]
      public void should_used_the_provided_interpolation_methods_to_interpolate_the_mean()
      {
         A.CallTo(() => _interpolation.Interpolate(A<IEnumerable<Sample>>._, _originData.Age.Value)).MustHaveHappened();
      }
   }
}