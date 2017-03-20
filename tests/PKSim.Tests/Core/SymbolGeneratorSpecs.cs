using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SymbolGenerator : ContextSpecification<ISymbolGenerator>
   {
      protected override void Context()
      {
         sut = new SymbolGenerator();
      }
   }

   public class When_generating_symbols : concern_for_SymbolGenerator
   {
      [Observation]
      public void should_always_genereate_different_symbols()
      {
         var s1 = sut.NextSymbol();
         var s2 = sut.NextSymbol();
         var s3 = sut.NextSymbol();
         var s4 = sut.NextSymbol();
         
         s1.ShouldNotBeEqualTo(s2);
         s1.ShouldNotBeEqualTo(s3);
         s1.ShouldNotBeEqualTo(s4);
         s2.ShouldNotBeEqualTo(s3);
         s2.ShouldNotBeEqualTo(s4);
         s3.ShouldNotBeEqualTo(s4);
      }
   }
}	