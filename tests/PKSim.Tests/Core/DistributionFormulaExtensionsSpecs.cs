using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core
{
   public class When_retrieving_the_distribution_type_for_a_formula : StaticContextSpecification
   {
      private IDistributionFormula _normalDistributionFormula;
      private IDistributionFormula _logNormalDistributionFormula;
      private UniformDistributionFormula _uniformDistributionFormula;

      protected override void Context()
      {
         _normalDistributionFormula = A.Fake<NormalDistributionFormula>();
         _logNormalDistributionFormula = A.Fake<LogNormalDistributionFormula>();
         _uniformDistributionFormula = A.Fake<UniformDistributionFormula>();
      }

      [Observation]
      public void should_return_the_accurate_distribution_type()
      {
         _normalDistributionFormula.DistributionType().ShouldBeEqualTo(DistributionTypes.Normal);
         _logNormalDistributionFormula.DistributionType().ShouldBeEqualTo(DistributionTypes.LogNormal);
         _uniformDistributionFormula.DistributionType().ShouldBeEqualTo(DistributionTypes.Uniform);
      }
   }
}