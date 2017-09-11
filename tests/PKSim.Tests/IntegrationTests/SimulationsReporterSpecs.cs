using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting;
using PKSim.Infrastructure.Reporting.TeX.Reporters;

namespace PKSim.IntegrationTests
{
   [Category("Reporting")]
   public abstract class concern_for_SimulationsReporter : ContextForSimulationIntegration<SimulationsReporter>
   {
      protected PKSimProject _project;
      private List<IPKSimBuildingBlock> _allBuildingBlocks;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _allBuildingBlocks = new List<IPKSimBuildingBlock>();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulation();
         _allBuildingBlocks.Add(_simulation.BuildingBlock<Individual>());
         _allBuildingBlocks.Add(_simulation.BuildingBlock<Compound>());
         _allBuildingBlocks.Add(_simulation.BuildingBlock<Protocol>());
      }

      protected override void Context()
      {
         var workspace = A.Fake<IWorkspace>();
         _project = A.Fake<PKSimProject>();
         workspace.Project = _project;
         A.CallTo(() => _project.All(PKSimBuildingBlockType.Template)).Returns(_allBuildingBlocks);
         A.CallTo(() => _project.All<Simulation>()).Returns(new[] {_simulation});
         sut = IoC.Resolve<SimulationsReporter>();
      }
   }

   public class When_creating_a_report_using_the_simulation_reporter : concern_for_SimulationsReporter
   {
      private string _fileName;
      private ReportSettings _reportSettings;
      private OSPSuiteBuildSettings _buildSettings;
      private OSPSuiteTracker _buildTracker;

      protected override void Context()
      {
         base.Context();
         _buildSettings = new OSPSuiteBuildSettings();
         _fileName = FileHelper.GenerateTemporaryFileName() + ".pdf";
         var buildTrackerFactory = IoC.Resolve<IBuildTrackerFactory>();
         _buildTracker = buildTrackerFactory.CreateFor<OSPSuiteTracker>(_fileName);
         _buildTracker.Settings = _buildSettings;
         _reportSettings = new ReportSettings
            {
               Title = "Test",
               Author = "Unit Tests Engine",
               Keywords = new[] {"Tests", "PKReporting", "SBSuite"},
               Software = "SBSuite",
               SubTitle = "SubTitle",
               SoftwareVersion = "5.2",
               TemplateFolder = DomainHelperForSpecs.TEXTemplateFolder(),
               ContentFileName = "Content",
               DeleteWorkingDir = true
            };
      }

      protected override void Because()
      {
         var reportCreator = IoC.Resolve<IReportCreator>();
         reportCreator.ReportToPDF(_buildTracker, _reportSettings, sut.Report(_project.All<Simulation>().ToList(), _buildTracker)).Wait();
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report()
      {
         FileHelper.FileExists(_fileName).ShouldBeTrue();
      }

      public override void Cleanup()
      {
         FileHelper.DeleteFile(_fileName);
         base.Cleanup();
      }
   }
}