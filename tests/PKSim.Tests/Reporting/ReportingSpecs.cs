using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Reporting;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.IntegrationTests;

namespace PKSim.Reporting
{
   [Category("Reporting")]
   public abstract class concern_for_Reporting : ContextWithLoadedProject<IOSPSuiteTeXReporter>
   {
      protected IReportingTask _reportingTask;
      protected ReportConfiguration _reportConfiguration;
      private DirectoryInfo _reportsDir;
      protected string _projectName;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject(_projectName);
         _reportsDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", _projectName));
         if (!_reportsDir.Exists)
            _reportsDir.Create();

         _reportingTask = IoC.Resolve<IReportingTask>();
         _reportConfiguration = new ReportConfiguration
         {
            Title = "Testing Reports",
            Author = "Unit Tests Engine",
            Keywords = new[] { "Tests", "PKReporting", "SBSuite" },
            Software = "SBSuite",
            SubTitle = "SubTitle",
            SoftwareVersion = "5.2",
            ContentFileName = "Content",
            DeleteWorkingDir = true,
            ColorStyle = ReportSettings.ReportColorStyles.Color
         };

         _reportConfiguration.Template = new ReportTemplate { Path = DomainHelperForSpecs.TEXTemplateFolder() };

         //Add all simulation to repository as they will be required when deserialzing the summary charts
         var repo = IoC.Resolve<IWithIdRepository>();
         All<Simulation>().Each(repo.Register);
      }

      public void CreateReportAndValidate(IObjectBase objectBase)
      {
         if (objectBase == null) return;
         var lazyLoadable = objectBase as ILazyLoadable;
         if (lazyLoadable != null)
            _lazyLoadTask.Load(lazyLoadable);

         CreateReportAndValidate(objectBase, objectBase.Name);
      }

      public void CreateReportAndValidate(object objectToReport, string reportName)
      {
         if (objectToReport == null) return;
         _reportConfiguration.ReportFile = Path.Combine(_reportsDir.FullName, string.Format("{0}.pdf", reportName));
         _reportConfiguration.SubTitle = reportName;
         _reportingTask.CreateReportAsync(objectToReport, _reportConfiguration).Wait();
         FileHelper.FileExists(_reportConfiguration.ReportFile).ShouldBeTrue();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         var repo = IoC.Resolve<IWithIdRepository>();
         All<Simulation>().Each(s => repo.Unregister(s.Id));
      }
   }

   public abstract class When_creating_a_report : concern_for_Reporting
   {
      protected When_creating_a_report(string projectName)
      {
         _projectName = projectName;
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_individual()
      {
         CreateReportAndValidate(First<Individual>());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_individuals()
      {
         CreateReportAndValidate(All<Individual>(), "Individuals");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_compound()
      {
         CreateReportAndValidate(First<Compound>());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_compounds()
      {
         CreateReportAndValidate(All<Compound>(), "Compounds");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_formulation()
      {
         CreateReportAndValidate(First<Formulation>());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_formulations()
      {
         CreateReportAndValidate(All<Formulation>(), "Formulations");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_population()
      {
         CreateReportAndValidate(First<Population>());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_populations()
      {
         CreateReportAndValidate(All<Population>(), "Populations");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_protocol()
      {
         CreateReportAndValidate(First<AdvancedProtocol>());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_protocols()
      {
         CreateReportAndValidate(All<AdvancedProtocol>(), "Advanced Protocols");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_simulation()
      {
         CreateReportAndValidate(First<Simulation>());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_simulations()
      {
         CreateReportAndValidate(All<Simulation>(), "Simulations");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_observed_data()
      {
         CreateReportAndValidate(_project.AllObservedData.FirstOrDefault(), "Observed Data");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_all_observed_data()
      {
         CreateReportAndValidate(_project.AllObservedData.ToList(), "Observed Data List");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_project()
      {
         CreateReportAndValidate(_project, "Project");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_all_comparison_simulations()
      {
         CreateReportAndValidate(_project.AllSimulationComparisons, "SimulationComparisons");
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_individual_simulation_comparison()
      {
         CreateReportAndValidate(_project.AllSimulationComparisons.OfType<IndividualSimulationComparison>().FirstOrDefault());
      }

      [Observation]
      public void should_have_created_a_valid_pdf_report_for_population_simulation_comparison()
      {
         CreateReportAndValidate(_project.AllSimulationComparisons.OfType<PopulationSimulationComparison>().FirstOrDefault());
      }
   }

   public class Testing_project_ReportingProject : When_creating_a_report
   {
      public Testing_project_ReportingProject()
         : base("ReportingProject")
      {
      }
   }

   public class Testing_project_ObservedData_515 : When_creating_a_report
   {
      public Testing_project_ObservedData_515()
         : base("ObservedData_515")
      {
      }
   }

   public class Testing_project_PopSimTest : When_creating_a_report
   {
      public Testing_project_PopSimTest()
         : base("PopSimTest")
      {
      }
   }
}