using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using DistributionFormulaFactory = PKSim.Core.Model.DistributionFormulaFactory;
using IDistributionFormulaFactory = PKSim.Core.Model.IDistributionFormulaFactory;

namespace PKSim.Core
{
   public abstract class concern_for_DistributionFactory : ContextSpecification<IDistributionFormulaFactory>
   {
      protected IRepository<IDistributionFormulaSpecificationFactory> _repository;
      protected IDistributionFormulaSpecificationFactory _distributionSpecFactory;
      protected IEnumerable<ParameterDistributionMetaData> _distributionDefList;
      protected IDistributedParameter _parameter;
      protected OriginData _originData;
      private IReportGenerator _reportGenerator;
      protected IDistributedParameter _baseParameter;

      protected override void Context()
      {
         _repository = A.Fake<IRepository<IDistributionFormulaSpecificationFactory>>();
         _distributionSpecFactory = A.Fake<IDistributionFormulaSpecificationFactory>();
         _distributionDefList = new List<ParameterDistributionMetaData>();
         _parameter = A.Fake<IDistributedParameter>();
         _baseParameter = A.Fake<IDistributedParameter>();

         _reportGenerator = A.Fake<IReportGenerator>();
         _originData = A.Fake<OriginData>();
         A.CallTo(() => _reportGenerator.StringReportFor(_originData)).Returns("Origin Data");
         A.CallTo(() => _repository.All()).Returns(new[] {_distributionSpecFactory});
         sut = new DistributionFormulaFactory(_repository, _reportGenerator);
      }
   }

   public class When_creating_a_distribution_formula_for_a_distribution_that_does_not_exit : concern_for_DistributionFactory
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _distributionSpecFactory.IsSatisfiedBy(_distributionDefList)).Returns(false);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.CreateFor(_distributionDefList, _parameter, _originData)).ShouldThrowAn<DistributionNotFoundException>();
      }
   }

   public class When_creating_a_distribution_formula_for_a_distribution_that_is_recognized : concern_for_DistributionFactory
   {
      private DistributionFormula _formula;
      private DistributionFormula _result;

      protected override void Context()
      {
         base.Context();
         _formula = A.Fake<DistributionFormula>();
         A.CallTo(() => _distributionSpecFactory.IsSatisfiedBy(A<IEnumerable<ParameterDistributionMetaData>>._)).Returns(true);
         A.CallTo(() => _distributionSpecFactory.CreateFor(A<IEnumerable<ParameterDistributionMetaData>>._, _parameter, _originData)).Returns(_formula);
      }

      protected override void Because()
      {
         _result = sut.CreateFor(_distributionDefList, _parameter, _originData);
      }

      [Observation]
      public void should_return_a_well_defined_distribution_formula()
      {
         _result.ShouldBeEqualTo(_formula);
      }
   }

   public class When_updating_a_distribution_formula_for_a_distribution_ : concern_for_DistributionFactory
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _distributionSpecFactory.IsSatisfiedBy(A<IEnumerable<ParameterDistributionMetaData>>._)).Returns(true);
      }

      protected override void Because()
      {
         sut.UpdateDistributionBasedOn(_distributionDefList, _parameter, _baseParameter, _originData);
      }

      [Observation]
      public void should_leverage_the_accurate_distribution_formula_factory_to_update_the_distribution_parameters()
      {
         A.CallTo(() => _distributionSpecFactory.UpdateDistributionBasedOn(A<IEnumerable<ParameterDistributionMetaData>>._, _parameter, _baseParameter, _originData)).MustHaveHappened();
      }
   }
}