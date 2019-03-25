using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ObserverLoader : ContextForIntegration<IObserverLoader>
   {
   }

   public class When_loading_an_observer_from_a_valid_observer_file : concern_for_ObserverLoader
   {
      private IObserverBuilder _observer;
      private readonly string _observerFile = DomainHelperForSpecs.DataFilePathFor("Observer.pkml");

      protected override void Context()
      {
         base.Context();
         _observer = sut.Load(_observerFile);
      }

      [Observation]
      public void should_return_an_observer_with_the_expected_properties()
      {
         _observer.Name.ShouldBeEqualTo("Amount Lumen Colon");
         _observer.Formula.ShouldBeAnInstanceOf<ExplicitFormula>();
         _observer.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("col_asc + col_desc + col_sig + col_trans");
      }
   }

   public class When_loading_an_observer_from_an_invalid_observer_file : concern_for_ObserverLoader
   {
      private readonly string _observerFile = DomainHelperForSpecs.DataFilePathFor("Application.pkml");

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.Load(_observerFile)).ShouldThrowAn<OSPSuiteException>();
      }
   }
}