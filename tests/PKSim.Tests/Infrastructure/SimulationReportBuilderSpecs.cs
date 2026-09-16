using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Summary;
using PKSim.Infrastructure.Reporting.Summary.Items;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_SimulationReportBuilder : ContextSpecification<SimulationReportBuilder>
   {
      protected IReportGenerator _reportGenerator;
      protected IndividualSimulation _simulation;
      protected Compound _midazolam;
      protected Compound _aspirin;
      protected OverwriteParameterSet _overwriteParameterSet;

      protected override void Context()
      {
         _reportGenerator = A.Fake<IReportGenerator>();
         _midazolam = new Compound().WithName("Midazolam");
         _aspirin = new Compound().WithName("Aspirin");
         _overwriteParameterSet = new OverwriteParameterSet {Name = "Renal impairment"};
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("midazolamTemplate", PKSimBuildingBlockType.Compound) {BuildingBlock = _midazolam, Name = _midazolam.Name});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("aspirinTemplate", PKSimBuildingBlockType.Compound) {BuildingBlock = _aspirin, Name = _aspirin.Name});
         _simulation.Properties.AddCompoundProperties(new CompoundProperties {Compound = _midazolam});
         _simulation.Properties.AddCompoundProperties(new CompoundProperties {Compound = _aspirin});
         _simulation.AddOverwriteParameterSetSelection(_midazolam.Name, _overwriteParameterSet);
         sut = new SimulationReportBuilder(_reportGenerator);
      }
   }

   public class When_reporting_a_simulation_with_an_overwrite_parameter_set_selected_for_one_compound : concern_for_SimulationReportBuilder
   {
      protected override void Because()
      {
         sut.Report(_simulation);
      }

      [Observation]
      public void should_report_the_selected_overwrite_parameter_set_for_that_compound()
      {
         A.CallTo(() => _reportGenerator.ReportFor(A<CompoundPropertiesCalculationMethods>.That.Matches(x =>
            x.CompoundName == _midazolam.Name && x.OverwriteParameterSet == _overwriteParameterSet))).MustHaveHappened();
      }

      [Observation]
      public void should_not_report_an_overwrite_parameter_set_for_the_other_compound()
      {
         A.CallTo(() => _reportGenerator.ReportFor(A<CompoundPropertiesCalculationMethods>.That.Matches(x =>
            x.CompoundName == _aspirin.Name && x.OverwriteParameterSet == null))).MustHaveHappened();
      }
   }
}
