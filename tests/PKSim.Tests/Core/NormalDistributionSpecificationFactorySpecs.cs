using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using FakeItEasy;
using DistributionType = OSPSuite.Core.Domain.Formulas.DistributionType;
using IDistributionFormulaFactory = OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_NormalDistributionSpecificationFactory : ContextSpecification<NormalDistributionFormulaSpecificationFactory>
   {
      protected IInterpolation _interpolation;
      protected IDistributionFormulaFactory _distrFormulaFactory;

      protected override void Context()
      {
         _interpolation = A.Fake<IInterpolation>();

         _distrFormulaFactory = A.Fake<IDistributionFormulaFactory>();
         sut = new NormalDistributionFormulaSpecificationFactory(_interpolation, _distrFormulaFactory);
      }
   }

   
   public class When_asked_if_a_set_of_distributions_has_the_type_normal_distribution : concern_for_NormalDistributionSpecificationFactory
   {
      private IEnumerable<ParameterDistributionMetaData> _onlyNormalDistributions;
      private IEnumerable<ParameterDistributionMetaData> _mixedDistributions;
      private ParameterDistributionMetaData _normalDistribution1;
      private ParameterDistributionMetaData _normalDistribution2;
      private ParameterDistributionMetaData _logNormalDistribution;

      protected override void Context()
      {
         base.Context();
         _normalDistribution1 = new ParameterDistributionMetaData
         {
            DistributionType = DistributionType.Normal
         };
         _normalDistribution2 = new ParameterDistributionMetaData
         {
            DistributionType = DistributionType.Normal
         };
         _logNormalDistribution = new ParameterDistributionMetaData
         {
            DistributionType = DistributionType.LogNormal
         };
         _onlyNormalDistributions = new List<ParameterDistributionMetaData> {_normalDistribution1, _normalDistribution2};
         _mixedDistributions = new List<ParameterDistributionMetaData> {_normalDistribution1, _logNormalDistribution};
      }

      [Observation]
      public void should_return_true_for_a_set_containing_only_normal_distributions()
      {
         sut.IsSatisfiedBy(_onlyNormalDistributions).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_set_containing_at_least_one_distribution_which_is_not_normal()
      {
         sut.IsSatisfiedBy(_mixedDistributions).ShouldBeFalse();
      }
   }

   
   public class When_creating_a_normal_distribution : concern_for_NormalDistributionSpecificationFactory
   {
      protected IEnumerable<ParameterDistributionMetaData> _normalDistributions;
      protected ParameterDistributionMetaData _normalDistribution1;
      protected ParameterDistributionMetaData _normalDistribution2;
      protected DistributionFormula _resultingDistribution;
      protected IDistributedParameter _parameter;
      protected OriginData _originData;

      protected override void Context()
      {
         base.Context();
         _normalDistribution1 = new ParameterDistributionMetaData {Age = 20, Mean = 5, Deviation = 10};
         _normalDistribution2 = new ParameterDistributionMetaData {Age = 10, Mean = 15, Deviation = 30};
         _normalDistributions = new List<ParameterDistributionMetaData> {_normalDistribution1, _normalDistribution2};
         _parameter = A.Fake<IDistributedParameter>();

         IParameter meanParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("Mean");
         IParameter deviationParameter = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("Deviation");
         A.CallTo(() => _parameter.MeanParameter).Returns(meanParameter);
         A.CallTo(() => _parameter.DeviationParameter).Returns(deviationParameter);
         A.CallTo(() => _distrFormulaFactory.CreateNormalDistributionFormulaFor(_parameter,meanParameter, deviationParameter)).Returns(new NormalDistributionFormula());

         _originData = new OriginData {Age = new OriginDataParameter(12)};
      }

      protected override void Because()
      {
         _resultingDistribution = sut.CreateFor(_normalDistributions, _parameter, _originData);
      }

      [Observation]
      public void should_return_a_distributon_formula_of_type_normal_distribution()
      {
         _resultingDistribution.IsAnImplementationOf<NormalDistributionFormula>().ShouldBeTrue();
      }
   }
}