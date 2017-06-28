using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   [NightlyOnly]
   public abstract class concern_for_TemplateTaskQueryConversion : ContextForIntegration<ITemplateTaskQuery>
   {
      
   }

   public class When_loading_some_templates_from_a_template_database_created_with_a_version_5_2_of_the_software_or_older : concern_for_TemplateTaskQueryConversion
   {
      private IEnumerable<Template> _allTemplates;
      private string _tmpFile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var userSettings = IoC.Resolve<ICoreUserSettings>();
         var testFile = DomainHelperForSpecs.DataFilePathFor("TemplateDatabaseV5.2.mdb");
         _tmpFile = FileHelper.GenerateTemporaryFileName();
         FileHelper.Copy(testFile, _tmpFile);
         userSettings.TemplateDatabasePath = _tmpFile;
      }

      protected override void Because()
      {
         _allTemplates = sut.AllTemplatesFor(TemplateDatabaseType.User, TemplateType.Individual|TemplateType.Event|TemplateType.Formulation|TemplateType.Population|TemplateType.Protocol|TemplateType.Compound);
      }

      [Observation]
      public void should_have_converted_the_database_and_return_the_expected_templates()
      {
         //we exactly have 7 templates in our test db
         _allTemplates.Count().ShouldBeEqualTo(7);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         try
         {
            //for some reason this fails permanently in TeamCity
            FileHelper.DeleteFile(_tmpFile);
         }
         catch {}
      }
   }

   public class When_loading_some_templates_from_a_template_database_created_with_a_version_5_5_of_the_software_or_older : concern_for_TemplateTaskQueryConversion
   {
      private IEnumerable<Template> _allTemplates;
      private string _tmpFile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var userSettings = IoC.Resolve<ICoreUserSettings>();
         var testFile = DomainHelperForSpecs.DataFilePathFor("TemplateDatabaseV5.5.mdb");
         _tmpFile = FileHelper.GenerateTemporaryFileName();
         FileHelper.Copy(testFile, _tmpFile);
         userSettings.TemplateDatabasePath = _tmpFile;
      }

      protected override void Because()
      {
         _allTemplates = sut.AllTemplatesFor(TemplateDatabaseType.User, TemplateType.PopulationAnalysisField);
      }

      [Observation]
      public void should_have_converted_the_database_and_return_the_expected_templates()
      {
         //we exactly have 2 templates in our test db
         _allTemplates.Count().ShouldBeEqualTo(2);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_tmpFile);
      }
   }
}