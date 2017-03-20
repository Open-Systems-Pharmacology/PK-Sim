using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using FakeItEasy;
using IDistributionFormulaFactory = OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_LogNormalDistributionSpecificationFactory : ContextSpecification<LogNormalDistributionFormulaSpecificationFactory>
   {
      protected IInterpolation _interpolation;
      protected IDistributionFormulaFactory _distrFormulaFactory;

      protected override void Context()
      {
         _interpolation = A.Fake<IInterpolation>();
         _distrFormulaFactory = A.Fake<IDistributionFormulaFactory>();
         sut = new LogNormalDistributionFormulaSpecificationFactory(_interpolation, _distrFormulaFactory);
      }
   }

   
   public class When_asked_if_a_set_of_distributions_has_the_type_log_normal_distribution : concern_for_LogNormalDistributionSpecificationFactory
   {
      private IEnumerable<ParameterDistributionMetaData> _onlyLogNormalDistributions;
      private IEnumerable<ParameterDistributionMetaData> _mixedDistributions;
      private ParameterDistributionMetaData _logNormalDistribution1;
      private ParameterDistributionMetaData _logNormalDistribution2;
      private ParameterDistributionMetaData _normalDistribution;

      protected override void Context()
      {
         base.Context();
         _logNormalDistribution1 = new ParameterDistributionMetaData();
         _logNormalDistribution1.DistributionType = CoreConstants.Distribution.LogNormal; 
         _logNormalDistribution2 = new ParameterDistributionMetaData();
         _logNormalDistribution2.DistributionType = CoreConstants.Distribution.LogNormal;
         _normalDistribution = new ParameterDistributionMetaData();
         _normalDistribution.DistributionType = CoreConstants.Distribution.Normal;
         _onlyLogNormalDistributions = new List<ParameterDistributionMetaData> {_logNormalDistribution1, _logNormalDistribution2};
         _mixedDistributions = new List<ParameterDistributionMetaData> {_logNormalDistribution1, _normalDistribution};
      }

      [Observation]
      public void should_return_true_for_a_set_containing_only_log_normal_distributions()
      {
         sut.IsSatisfiedBy(_onlyLogNormalDistributions).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_set_containing_at_least_one_distribution_which_is_not_log_normal()
      {
         sut.IsSatisfiedBy(_mixedDistributions).ShouldBeFalse();
      }
   }
}