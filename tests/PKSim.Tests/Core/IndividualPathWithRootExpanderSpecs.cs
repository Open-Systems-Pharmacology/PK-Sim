using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualPathWithRootExpander : ContextSpecification<IIndividualPathWithRootExpander>
   {
      protected override void Context()
      {
         sut = new IndividualPathWithRootExpander();
      }
   }

   
   public class When_expanding_the_path_defined_in_the_formula_used_in_an_individual_ : concern_for_IndividualPathWithRootExpander
   {
      private PKSim.Core.Model.Individual _individual;
      private IContainer _container;
      private IParameter _parameter;
      private IFormulaUsablePath _oneRelativeObjectPath;
      private IFormulaUsablePath _oneOrganismAbsoluteObjectPath;
      private string _oneRelativeObjectPathAsString;
      private string _oneOrganismAbsoluteObjectPathAsString;

      protected override void Context()
      {
         base.Context();
         _individual = new Individual().WithName("Individual");
         _container = new Container().WithName("TOTO");
         _parameter = new Parameter().WithName("Pa1");
         _parameter.Formula = new ExplicitFormula();
         _oneRelativeObjectPath = new FormulaUsablePath(new[] {"A", "B", "C"});
         _oneRelativeObjectPathAsString = _oneRelativeObjectPath.ToString();
         _parameter.Formula.AddObjectPath(_oneRelativeObjectPath);
         _oneOrganismAbsoluteObjectPath = new FormulaUsablePath(new[] { Constants.ORGANISM, "B", "C" });
         _oneOrganismAbsoluteObjectPathAsString = _oneOrganismAbsoluteObjectPath.ToString();
         _parameter.Formula.AddObjectPath(_oneOrganismAbsoluteObjectPath);

         _container.Add(_parameter);
         _individual.Add(_container);
      }

      protected override void Because()
      {
         sut.AddRootToPathIn(_individual);
      }

      [Observation]
      public void should_add_the_root_keyword_for_all_paths_that_are_starting_with_the_organism()
      {
         _oneOrganismAbsoluteObjectPath.PathAsString.ShouldNotBeEqualTo(_oneOrganismAbsoluteObjectPathAsString);
         _oneOrganismAbsoluteObjectPath.PathAsString.StartsWith(Constants.ROOT).ShouldBeTrue();
      }

      [Observation]
      public void should_not_change_the_paths_that_do_not_start_with_organism()
      {
         _oneRelativeObjectPath.PathAsString.ShouldBeEqualTo(_oneRelativeObjectPathAsString);
      }
   }
}