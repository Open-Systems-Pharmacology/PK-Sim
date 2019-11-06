using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_IndividualResultsImporter : ContextSpecification<IIndividualResultsImporter>
   {
      protected string _fileName;
      protected List<IndividualResults> _results;
      protected ImportLogger _importLogger = new ImportLogger();
      private Simulation _simulation;

      protected override void Context()
      {
         sut = new IndividualResultsImporter();
         _simulation = A.Fake<Simulation>().WithName("Sim");
      }

      protected override void Because()
      {
         _results = sut.ImportFrom(_fileName, _simulation, _importLogger).ToList();
      }
   }

   public class When_importing_a_file_containing_individual_results : concern_for_IndividualResultsImporter
   {
      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.SimulationResultsFilePathFor("res_10");
      }

      [Observation]
      public void should_return_an_individual_results_containing_the_expected_number_of_individuals()
      {
         _results.Count.ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_have_created_one_quantity_values_for_each_individual_results()
      {
         _results.Each(x => x.Count().ShouldBeEqualTo(1));
      }

      [Observation]
      public void should_have_imported_the_expected_number_of_values()
      {
         _results.Each(x =>
         {
            x.Time.Length.ShouldBeEqualTo(150);
            x.AllValues.First().Length.ShouldBeEqualTo(150);
         });
      }
   }

   public class When_importing_a_file_that_does_not_have_headers : concern_for_IndividualResultsImporter
   {
      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.SimulationResultsFilePathFor("no_header");
      }

      [Observation]
      public void should_return_an_empty_individual_results_set()
      {
         _results.ShouldBeEmpty();
      }

      [Observation]
      public void should_have_the_error_status()
      {
         _importLogger.Status.ShouldBeEqualTo(NotificationType.Error);
      }
   }

   public class When_importing_a_file_that_does_not_have_the_expected_header_values : concern_for_IndividualResultsImporter
   {
      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.SimulationResultsFilePathFor("header_renamed");
      }

      [Observation]
      public void should_return_a_valid_results_set()
      {
         _results.Count.ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_have_a_valid_status()
      {
         _importLogger.Status.ShouldBeEqualTo(NotificationType.Info);
      }
   }

   public class When_importing_a_file_containign_quantity_path_with_simulation_name : concern_for_IndividualResultsImporter
   {
      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.SimulationResultsFilePathFor("res_10_sim_name_set");
      }

      [Observation]
      public void should_have_removed_the_name_of_the_simulation_at_the_beginning()
      {
         _results.Count.ShouldBeGreaterThan(0);
         var quantityPath = _results[0].AllValues.Select(x => x.QuantityPath).First();
         quantityPath.ShouldBeEqualTo("Organism|Comp");
      }
   }

   public class When_importing_a_file_containing_empty_cells : concern_for_IndividualResultsImporter
   {
      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.SimulationResultsFilePathFor("res_10_nan");
      }

      [Observation]
      public void should_interpret_them_as_NaN()
      {
         _results.Count.ShouldBeGreaterThan(0);
      }
   }

   public class When_importing_a_valid_file_containing_some_units_in_header : concern_for_IndividualResultsImporter
   {
      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.SimulationResultsFilePathFor("header_unit_10");
      }

      [Observation]
      public void should_return_an_individual_results_containing_the_expected_number_of_individuals()
      {
         _results.Count.ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_have_created_one_quantity_values_for_each_individual_results()
      {
         _results.Each(x => x.Count().ShouldBeEqualTo(1));
      }

      [Observation]
      public void should_have_removed_the_unit_name_of_the_quantity_header()
      {
         _results.First().ValuesFor("Organism|plasma").Length.ShouldBeEqualTo(150);
      }

      [Observation]
      public void should_have_imported_the_expected_number_of_values()
      {
         _results.Each(x =>
         {
            x.Time.Length.ShouldBeEqualTo(150);
            x.AllValues.First().Length.ShouldBeEqualTo(150);
         });
      }
   }
}