using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Summary;
using PKSim.Infrastructure.Reporting.Summary.Items;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationCompoundConfigurationReportBuilder : ContextSpecification<SimulationCompoundConfigurationReportBuilder>
   {
      protected IReportGenerator _reportGenerator;
      protected CompoundProperties _compoundProperties;
      protected TablePart _calculationMethodsPart;
      protected TablePart _result;

      protected override void Context()
      {
         _reportGenerator = A.Fake<IReportGenerator>();
         _compoundProperties = new CompoundProperties();
         _calculationMethodsPart = new TablePart(PKSimConstants.UI.Category, PKSimConstants.UI.CalculationMethods);
         _calculationMethodsPart.AddIs("Partition coefficients", "Rodgers and Rowland");
         A.CallTo(() => _reportGenerator.ReportFor(A<IEnumerable<CalculationMethod>>._)).Returns(_calculationMethodsPart);
         sut = new SimulationCompoundConfigurationReportBuilder(_reportGenerator);
      }

      protected IReadOnlyList<string> valuesFor(string key) => _result.Rows.First(x => x.Key == key).Value;
   }

   public class When_reporting_the_calculation_methods_of_a_compound_with_a_selected_overwrite_parameter_set : concern_for_SimulationCompoundConfigurationReportBuilder
   {
      protected override void Because()
      {
         var overwriteParameterSet = new OverwriteParameterSet {Name = "Renal impairment"};
         _result = sut.Report(new SimulationCompoundConfiguration("Midazolam", _compoundProperties, overwriteParameterSet)).DowncastTo<TablePart>();
      }

      [Observation]
      public void should_use_the_compound_name_as_title()
      {
         _result.Title.ShouldBeEqualTo("Midazolam");
      }

      [Observation]
      public void should_list_the_selected_overwrite_parameter_set_after_the_calculation_methods()
      {
         _result.Rows.Select(x => x.Key).ShouldOnlyContainInOrder("Partition coefficients", PKSimConstants.ObjectTypes.OverwriteParameterSet);
         valuesFor(PKSimConstants.ObjectTypes.OverwriteParameterSet).ShouldOnlyContain("Renal impairment");
      }
   }

   public class When_reporting_the_calculation_methods_of_a_compound_without_a_selected_overwrite_parameter_set : concern_for_SimulationCompoundConfigurationReportBuilder
   {
      protected override void Because()
      {
         _result = sut.Report(new SimulationCompoundConfiguration("Midazolam", _compoundProperties, null)).DowncastTo<TablePart>();
      }

      [Observation]
      public void should_only_list_the_calculation_methods()
      {
         _result.Rows.Select(x => x.Key).ShouldOnlyContain("Partition coefficients");
      }
   }
}
