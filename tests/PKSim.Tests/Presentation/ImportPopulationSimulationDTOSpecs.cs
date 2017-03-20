using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Core.Extensions;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportPopulationSimulationDTO : ContextSpecification<ImportPopulationSimulationDTO>
   {
      private const string _simulationFile = "SimulationFile";
      protected const string _populationFile = "PopulationFile";
      private Func<string, bool> _oldFileExists;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileExists = FileHelper.FileExists;
         FileHelper.FileExists = s => s.IsOneOf(_simulationFile, _populationFile);
      }

      protected override void Context()
      {
         sut = new ImportPopulationSimulationDTO {FilePath = _simulationFile};
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExists;
      }
   }

   public class When_the_import_mode_is_building_block : concern_for_ImportPopulationSimulationDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.PopulationImportMode = PopulationImportMode.BuildingBlock;
      }

      [Observation]
      public void should_have_an_invalid_status_if_the_building_block_is_not_set()
      {
         sut.Population = null;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_have_anvalid_status_if_the_building_block_is_set()
      {
         sut.Population = A.Fake<Population>();
         ;
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_the_import_mode_is_population_file : concern_for_ImportPopulationSimulationDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.PopulationImportMode = PopulationImportMode.File;
      }

      [Observation]
      public void should_have_an_invalid_status_if_the_population_file_is_not_set()
      {
         sut.PopulationFile = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_have_an_invalid_status_if_the_population_file_does_not_exist()
      {
         sut.PopulationFile = "File that does not exist";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_have_anvalid_status_if_the_building_block_is_set()
      {
         sut.PopulationFile = _populationFile;
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_the_import_mode_is_population_size : concern_for_ImportPopulationSimulationDTO
   {
      protected override void Context()
      {
         base.Context();
         sut.PopulationImportMode = PopulationImportMode.Size;
      }

      [Observation]
      public void should_have_an_invalid_status_if_the_size_is_smaller_than_2()
      {
         sut.NumberOfIndividuals = 1;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_have_a_valid_status_if_the_size_was_set_to_a_number_bigger_than_1()
      {
         sut.NumberOfIndividuals = 2;
         sut.IsValid().ShouldBeTrue();
      }
   }
}