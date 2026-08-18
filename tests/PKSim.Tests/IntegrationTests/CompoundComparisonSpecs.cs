using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_CompoundComparison : ContextForIntegration<IObjectComparer>
   {
      protected Compound _templateCompound;
      protected Compound _simulationCompound;
      protected DiffReport _report;

      protected override void Context()
      {
         base.Context();
         var compoundFactory = IoC.Resolve<ICompoundFactory>();
         _templateCompound = compoundFactory.Create().WithName("C1");
         _simulationCompound = compoundFactory.Create().WithName("C1");
      }

      protected override void Because()
      {
         _report = sut.Compare(_templateCompound, _simulationCompound);
      }

      protected OverwriteParameterSet OverwriteParameterSetFor(string name, double value)
      {
         var overwriteParameterSet = new OverwriteParameterSet { Name = name };
         overwriteParameterSet.Add(new ParameterValue { Path = "C1|Lipophilicity".ToObjectPath(), Value = value });
         return overwriteParameterSet;
      }
   }

   public class When_comparing_a_compound_defining_an_overwrite_parameter_set_with_a_compound_that_does_not_define_it : concern_for_CompoundComparison
   {
      protected override void Context()
      {
         base.Context();
         _templateCompound.AddOverwriteParameterSet(OverwriteParameterSetFor("OPS xyz", 2));
      }

      [Observation]
      public void should_report_the_overwrite_parameter_set_as_missing()
      {
         var missingItem = _report.OfType<MissingDiffItem>().SingleOrDefault(x => x.MissingObjectType == PKSimConstants.ObjectTypes.OverwriteParameterSet);
         missingItem.ShouldNotBeNull();
         missingItem.MissingObjectName.ShouldBeEqualTo("OPS xyz");
      }
   }

   public class When_comparing_two_compounds_defining_the_same_overwrite_parameter_set_with_different_values : concern_for_CompoundComparison
   {
      protected override void Context()
      {
         base.Context();
         _templateCompound.AddOverwriteParameterSet(OverwriteParameterSetFor("OPS xyz", 2));
         _simulationCompound.AddOverwriteParameterSet(OverwriteParameterSetFor("OPS xyz", 5));
      }

      [Observation]
      public void should_report_the_value_defined_in_the_set_as_different()
      {
         _report.OfType<PropertyValueDiffItem>().Any(x => x.Object1.IsAnImplementationOf<ParameterValue>()).ShouldBeTrue();
      }
   }

   public class When_comparing_two_compounds_defining_the_same_overwrite_parameter_set : concern_for_CompoundComparison
   {
      protected override void Context()
      {
         base.Context();
         _templateCompound.AddOverwriteParameterSet(OverwriteParameterSetFor("OPS xyz", 2));
         _simulationCompound.AddOverwriteParameterSet(OverwriteParameterSetFor("OPS xyz", 2));
      }

      [Observation]
      public void should_not_report_any_difference()
      {
         _report.IsEmpty.ShouldBeTrue();
      }
   }
}
