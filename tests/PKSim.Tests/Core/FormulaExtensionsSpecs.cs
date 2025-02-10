using System;
using System.Linq;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_FormulaExtensions : ContextSpecification<Formula>
   {
      protected override void Context()
      {
         var sumFormula = new SumFormula();
         sut = sumFormula;
         
         sut.AddObjectPath(new FormulaUsablePath(new string[]{"X1","Y1","Z1"}));
         sut.AddObjectPath(new FormulaUsablePath(new string[] { "X2", "Y2", "Z2" }));

         sumFormula.Criteria.Add(new MatchTagCondition("X1"));
         sumFormula.Criteria.Add(new MatchTagCondition("XYZ"));
      }
   }

   public class When_replacing_formula_keywords : concern_for_FormulaExtensions
   {
      protected override void Because()
      {
         sut.ReplaceKeywordsInObjectPaths(new string[] {"X1", "X2","Y1","Y2"}, new string[] {"a", "b","c","Y2"});
      }

      [Observation]
      public void should_replace_keywords_in_object_paths_and_criteria_conditions()
      {
         var paths = sut.ObjectPaths.ToList();

         paths.Count.ShouldBeEqualTo(2);

         paths[0].ToString().ShouldBeEqualTo("a|c|Z1");
         paths[1].ToString().ShouldBeEqualTo("b|Y2|Z2");

         var sumFormula = sut as SumFormula;
         sumFormula.Criteria.Count.ShouldBeEqualTo(2);
         sumFormula.Criteria[0].Condition.ShouldBeEqualTo("a");
         sumFormula.Criteria[1].Condition.ShouldBeEqualTo("XYZ");
      }
   }
}