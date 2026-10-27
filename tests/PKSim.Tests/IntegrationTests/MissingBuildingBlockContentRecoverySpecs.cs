using System;
using System.IO;
using Microsoft.Data.Sqlite;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   /// <summary>
   ///    Reproduces the project files written by PK-Sim 12.1 to 12.3 for projects created before version 11 (issue 3861):
   ///    the building block row of an expression profile exists but its content row holds no data
   /// </summary>
   public abstract class concern_for_project_with_missing_building_block_content : ContextWithLoadedProject<IMissingBuildingBlockContentRecoveryTask>
   {
      //expression profile defined in v11_expression_profile.pksim5 and referenced by the individual 'Ind'
      protected const string _expressionProfileId = "AddYgnx15EmknpjNPN8ncg";
      protected const string _expressionProfileName = "CYP3A|Human|Healthy";
      protected const string _individualName = "Ind";
      protected string _projectFile;
      protected bool _expressionProfileWasSkippedDuringLoad;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _projectFile = temporaryProjectFile();
         File.Copy(DomainHelperForSpecs.DataFilePathFor("v11_expression_profile.pksim5"), _projectFile, overwrite: true);
         removeContentOf(_expressionProfileId);

         LoadProject(_projectFile, isFullPath: true);
         _expressionProfileWasSkippedDuringLoad = _project.BuildingBlockById<ExpressionProfile>(_expressionProfileId) == null;
         sut = IoC.Resolve<IMissingBuildingBlockContentRecoveryTask>();
      }

      protected static string temporaryProjectFile() => Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{CoreConstants.Filter.PROJECT_EXTENSION}");

      private void removeContentOf(string buildingBlockId)
      {
         using (var connection = new SqliteConnection($"Data Source={_projectFile}"))
         {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
               command.CommandText = "UPDATE CONTENTS SET Data = NULL WHERE Id = (SELECT ContentId FROM BUILDING_BLOCKS WHERE Id = $id)";
               command.Parameters.AddWithValue("$id", buildingBlockId);
               command.ExecuteNonQuery();
            }
         }
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         //the session factory keeps the project file open
         IoC.Resolve<IWorkspacePersistor>().CloseSession();
         FileHelper.DeleteFile(_projectFile);
      }
   }

   public class When_loading_a_project_in_which_the_content_of_an_expression_profile_used_by_an_individual_is_missing : concern_for_project_with_missing_building_block_content
   {
      private MissingBuildingBlockContentRecoveryResult _result;
      private ExpressionProfile _rebuiltExpressionProfile;
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         //the recovery happens once per load: the registry is cleared afterwards
         _result = sut.Recover(_project);
      }

      protected override void Because()
      {
         _rebuiltExpressionProfile = _project.BuildingBlockById<ExpressionProfile>(_expressionProfileId);
         _individual = FindByName<Individual>(_individualName);
      }

      [Observation]
      public void should_have_loaded_the_project_without_the_expression_profile_instead_of_crashing()
      {
         _expressionProfileWasSkippedDuringLoad.ShouldBeTrue();
      }

      [Observation]
      public void should_have_rebuilt_the_expression_profile_from_the_individual_with_its_original_id_and_name()
      {
         _rebuiltExpressionProfile.ShouldNotBeNull();
         _rebuiltExpressionProfile.Name.ShouldBeEqualTo(_expressionProfileName);
         _rebuiltExpressionProfile.Molecule.ShouldBeAnInstanceOf<IndividualEnzyme>();
         _rebuiltExpressionProfile.MoleculeName.ShouldBeEqualTo("CYP3A");
         _rebuiltExpressionProfile.IsLoaded.ShouldBeTrue();
         _rebuiltExpressionProfile.HasChanged.ShouldBeTrue();
      }

      [Observation]
      public void should_have_added_the_rebuilt_expression_profile_to_the_individual()
      {
         _individual.Uses(_rebuiltExpressionProfile).ShouldBeTrue();
      }

      [Observation]
      public void should_report_the_rebuilt_expression_profile()
      {
         _result.RebuiltBuildingBlocks.ShouldOnlyContain(_rebuiltExpressionProfile);
         _result.RemovedBuildingBlocks.ShouldBeEmpty();
         _result.Message.Contains(_expressionProfileName).ShouldBeTrue();
      }
   }

   public class When_saving_a_project_in_which_an_expression_profile_was_rebuilt_and_loading_it_again : concern_for_project_with_missing_building_block_content
   {
      private string _savedProjectFile;
      private ExpressionProfile _expressionProfileAfterReload;
      private MissingBuildingBlockContentRecoveryResult _resultAfterReload;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut.Recover(_project);
      }

      protected override void Because()
      {
         var workspacePersistor = IoC.Resolve<IWorkspacePersistor>();
         _savedProjectFile = temporaryProjectFile();
         workspacePersistor.SaveSession(_workspace, _savedProjectFile);
         workspacePersistor.CloseSession();
         UnloadProject();

         LoadProject(_savedProjectFile, isFullPath: true);
         _expressionProfileAfterReload = _project.BuildingBlockById<ExpressionProfile>(_expressionProfileId);
         _resultAfterReload = sut.Recover(_project);
      }

      [Observation]
      public void should_have_persisted_the_rebuilt_expression_profile_with_its_content()
      {
         _expressionProfileAfterReload.ShouldNotBeNull();
         _expressionProfileAfterReload.IsLoaded.ShouldBeTrue();
         _expressionProfileAfterReload.Name.ShouldBeEqualTo(_expressionProfileName);
         _expressionProfileAfterReload.MoleculeName.ShouldBeEqualTo("CYP3A");
      }

      [Observation]
      public void should_not_have_anything_left_to_recover_in_the_saved_project()
      {
         _resultAfterReload.Any.ShouldBeFalse();
      }

      [Observation]
      public void should_still_reference_the_expression_profile_from_the_individual()
      {
         FindByName<Individual>(_individualName).Uses(_expressionProfileAfterReload).ShouldBeTrue();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_savedProjectFile);
      }
   }
}
