using System;
using System.Linq;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_FormulaExtensions : ContextSpecification<Formula>
   {
      protected override void Context()
      {
         sut=new ExplicitFormula();
         
         sut.AddObjectPath(new FormulaUsablePath(new string[]{"X1","Y1","Z1"}));
         sut.AddObjectPath(new FormulaUsablePath(new string[] { "X2", "Y2", "Z2" }));
      }
   }

   public class When_replacing_formula_keywords : concern_for_FormulaExtensions
   {
      protected override void Because()
      {
         sut.ReplaceKeywordsInObjectPaths(new string[] {"X1", "X2","Y1","Y2"}, new string[] {"a", "b","c","Y2"});
      }

      [Observation]
      public void should_replace_keywords()
      {
         var paths = sut.ObjectPaths.ToList();

         paths.Count.ShouldBeEqualTo(2);

         paths[0].ToString().ShouldBeEqualTo("a|c|Z1");
         paths[1].ToString().ShouldBeEqualTo("b|Y2|Z2");
      }
   }
}