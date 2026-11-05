using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_RemoteTemplateRepository_with_template_sources : ContextSpecificationAsync<RemoteTemplateRepository>
   {
      protected IPKSimConfiguration _configuration;
      protected IJsonSerializer _jsonSerializer;
      protected ISnapshotTask _snapshotTask;
      protected IOSPSuiteLogger _logger;
      protected ICoreUserSettings _userSettings;
      protected ITemplateDownloader _templateDownloader;
      protected string _rootFolder;
      protected string _templateFolder;
      protected string _ospSummaryPath;
      protected TemplateSource _source;
      protected string _sourceSummaryPath;
      protected RemoteTemplates _ospSummary;
      protected RemoteTemplates _sourceSummary;
      protected List<TemplateSource> _templateSources;

      protected override Task Context()
      {
         _rootFolder = Path.Combine(Path.GetTempPath(), "PKSimSpecs", Guid.NewGuid().ToString("N"));
         _templateFolder = Path.Combine(_rootFolder, "Templates");
         _ospSummaryPath = Path.Combine(_rootFolder, CoreConstants.REMOTE_TEMPLATE_SUMMARY);
         DirectoryHelper.CreateDirectory(_templateFolder);

         _configuration = A.Fake<IPKSimConfiguration>();
         A.CallTo(() => _configuration.RemoteTemplateFolderPath).Returns(_templateFolder);
         A.CallTo(() => _configuration.RemoteTemplateSummaryPath).Returns(_ospSummaryPath);

         _jsonSerializer = A.Fake<IJsonSerializer>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _templateDownloader = A.Fake<ITemplateDownloader>();

         _source = new TemplateSource("Company Library", "https://templates.company.com/lib/templates.json");
         _templateSources = new List<TemplateSource> {_source};
         A.CallTo(() => _userSettings.TemplateSources).Returns(_templateSources);
         _sourceSummaryPath = Path.Combine(_templateFolder, _source.Key, CoreConstants.REMOTE_TEMPLATE_SUMMARY);

         _ospSummary = new RemoteTemplates
         {
            Version = "1.5",
            Templates = new[]
            {
               new RemoteTemplate {Name = "Alprazolam", Type = TemplateType.Compound, Url = "https://raw.githubusercontent.com/Open-Systems-Pharmacology/Alprazolam-Model/v2.0/Alprazolam-Model.json"}
            }
         };

         _sourceSummary = new RemoteTemplates
         {
            Version = "3.0",
            Templates = new[]
            {
               new RemoteTemplate {Name = "InHouse", Type = TemplateType.Compound, Url = "compounds/InHouse.json"},
               new RemoteTemplate {Name = "Versioned", Type = TemplateType.Compound, Url = "https://raw.githubusercontent.com/Company/Versioned-Model/v1.2/Versioned-Model.json"}
            }
         };

         writeSummary(_ospSummaryPath, _ospSummary);
         A.CallTo(() => _jsonSerializer.Deserialize<RemoteTemplates>(_sourceSummaryPath)).Returns(_sourceSummary);

         //the fake downloader creates the destination file to simulate a successful download
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._))
            .Invokes(x => File.WriteAllText(x.GetArgument<string>(1), "{}"));

         sut = new RemoteTemplateRepository(_configuration, _jsonSerializer, _snapshotTask, _logger, _userSettings, _templateDownloader);
         return _completed;
      }

      protected void writeSummary(string summaryPath, RemoteTemplates summary)
      {
         DirectoryHelper.CreateDirectory(Path.GetDirectoryName(summaryPath));
         File.WriteAllText(summaryPath, "{}");
         A.CallTo(() => _jsonSerializer.Deserialize<RemoteTemplates>(summaryPath)).Returns(summary);
      }

      protected string sourceFileFor(string fileName) => Path.Combine(_templateFolder, _source.Key, fileName);

      public override Task Cleanup()
      {
         try
         {
            Directory.Delete(_rootFolder, recursive: true);
         }
         catch (Exception)
         {
            //best effort
         }

         return base.Cleanup();
      }
   }

   public class When_loading_the_templates_and_the_summary_of_an_additional_source_was_downloaded_already : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private List<RemoteTemplate> _allTemplates;

      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
      }

      protected override Task Because()
      {
         _allTemplates = sut.All().ToList();
         return _completed;
      }

      [Observation]
      public void should_return_the_templates_of_the_osp_library_and_of_the_additional_source()
      {
         _allTemplates.Select(x => x.Name).ShouldOnlyContain("Alprazolam", "InHouse", "Versioned");
      }

      [Observation]
      public void should_use_the_version_of_the_osp_library()
      {
         sut.Version.ShouldBeEqualTo("1.5");
      }

      [Observation]
      public void should_mark_the_templates_of_the_osp_library_as_such()
      {
         var template = _allTemplates.FindByName("Alprazolam");
         template.IsFromOSPLibrary.ShouldBeTrue();
         template.Version.ShouldBeEqualTo("2.0");
      }

      [Observation]
      public void should_attach_the_source_to_its_templates()
      {
         _allTemplates.FindByName("InHouse").Source.ShouldBeEqualTo(_source);
         _allTemplates.FindByName("Versioned").Source.ShouldBeEqualTo(_source);
      }

      [Observation]
      public void should_resolve_relative_urls_against_the_location_of_the_summary()
      {
         var template = _allTemplates.FindByName("InHouse");
         template.Url.ShouldBeEqualTo("https://templates.company.com/lib/compounds/InHouse.json");
         template.Version.ShouldBeEqualTo(CoreConstants.DEFAULT_TEMPLATE_VERSION);
      }

      [Observation]
      public void should_extract_version_and_repository_from_github_urls_of_additional_sources()
      {
         var template = _allTemplates.FindByName("Versioned");
         template.Version.ShouldBeEqualTo("1.2");
         template.RepositoryUrl.ShouldBeEqualTo("https://github.com/Company/Versioned-Model/tree/v1.2/");
      }

      [Observation]
      public void should_not_download_anything()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).MustNotHaveHappened();
      }
   }

   public class When_loading_the_templates_and_the_summary_of_an_additional_source_is_not_available : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private List<RemoteTemplate> _allTemplates;

      protected override Task Because()
      {
         _allTemplates = sut.All().ToList();
         return _completed;
      }

      [Observation]
      public void should_only_return_the_templates_of_the_osp_library()
      {
         _allTemplates.Select(x => x.Name).ShouldOnlyContain("Alprazolam");
      }
   }

   public class When_loading_the_unavailable_template_sources : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private List<RemoteTemplate> _allTemplates;

      protected override async Task Context()
      {
         await base.Context();
         //ensure the repository was loaded before the summary becomes available
         sut.All().Count().ShouldBeEqualTo(1);
      }

      protected override async Task Because()
      {
         await sut.LoadUnavailableTemplateSourcesAsync();
         _allTemplates = sut.All().ToList();
      }

      [Observation]
      public void should_download_the_summary_of_the_source_allowing_interaction_with_the_user()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(_source.Uri, A<string>._, true)).MustHaveHappened();
         File.Exists(_sourceSummaryPath).ShouldBeTrue();
      }

      [Observation]
      public void should_reload_the_repository_with_the_templates_of_the_source()
      {
         _allTemplates.Select(x => x.Name).ShouldOnlyContain("Alprazolam", "InHouse", "Versioned");
      }
   }

   public class When_loading_the_unavailable_template_sources_and_all_sources_are_available : concern_for_RemoteTemplateRepository_with_template_sources
   {
      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
      }

      protected override Task Because() => sut.LoadUnavailableTemplateSourcesAsync();

      [Observation]
      public void should_not_download_anything()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).MustNotHaveHappened();
      }
   }

   public class When_updating_the_template_sources : concern_for_RemoteTemplateRepository_with_template_sources
   {
      protected override async Task Context()
      {
         await base.Context();
         _templateSources.Add(new TemplateSource("Invalid", "not a url"));
      }

      protected override Task Because() => sut.UpdateTemplateSourcesAsync();

      [Observation]
      public void should_download_the_summary_of_each_valid_source_without_interacting_with_the_user()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(_source.Uri, A<string>._, false)).MustHaveHappenedOnceExactly();
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_store_the_summary_in_a_folder_dedicated_to_the_source()
      {
         File.Exists(_sourceSummaryPath).ShouldBeTrue();
      }

      [Observation]
      public void should_make_the_templates_of_the_source_available()
      {
         sut.All().Select(x => x.Name).ShouldOnlyContain("Alprazolam", "InHouse", "Versioned");
      }
   }

   public class When_updating_the_template_sources_and_a_source_cannot_be_reached : concern_for_RemoteTemplateRepository_with_template_sources
   {
      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).Throws(new OSPSuiteException("offline"));
      }

      protected override Task Because() => sut.UpdateTemplateSourcesAsync();

      [Observation]
      public void should_log_a_warning()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Warning, A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_keep_the_previously_downloaded_summary()
      {
         File.Exists(_sourceSummaryPath).ShouldBeTrue();
         sut.All().Select(x => x.Name).ShouldOnlyContain("Alprazolam", "InHouse", "Versioned");
      }
   }

   public class When_loading_a_template_of_an_additional_source : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private RemoteTemplate _template;
      private string _expectedFile;

      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
         _template = sut.TemplateBy(TemplateType.Compound, "InHouse");
         _expectedFile = sourceFileFor($"{CoreConstants.DEFAULT_TEMPLATE_VERSION}_InHouse.json");
      }

      protected override Task Because() => sut.LoadTemplateAsync<Compound>(_template);

      [Observation]
      public void should_download_the_template_into_the_folder_of_the_source_allowing_interaction_with_the_user()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(new Uri("https://templates.company.com/lib/compounds/InHouse.json"), _expectedFile, true)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_building_block_from_the_downloaded_file()
      {
         A.CallTo(() => _snapshotTask.LoadModelFromProjectFileAsync<Compound>(_expectedFile, PKSimBuildingBlockType.Compound, "InHouse")).MustHaveHappened();
      }
   }

   public class When_loading_an_unversioned_template_of_an_additional_source_that_was_downloaded_before : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private RemoteTemplate _template;

      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
         _template = sut.TemplateBy(TemplateType.Compound, "InHouse");
         DirectoryHelper.CreateDirectory(Path.GetDirectoryName(sourceFileFor("x")));
         File.WriteAllText(sourceFileFor($"{CoreConstants.DEFAULT_TEMPLATE_VERSION}_InHouse.json"), "{}");
      }

      protected override Task Because() => sut.LoadTemplateAsync<Compound>(_template);

      [Observation]
      public void should_download_the_template_again_as_it_cannot_be_cached_safely()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).MustHaveHappened();
      }
   }

   public class When_loading_a_versioned_template_of_an_additional_source_that_was_downloaded_before : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private RemoteTemplate _template;

      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
         _template = sut.TemplateBy(TemplateType.Compound, "Versioned");
         File.WriteAllText(sourceFileFor("1.2_Versioned-Model.json"), "{}");
      }

      protected override Task Because() => sut.LoadTemplateAsync<Compound>(_template);

      [Observation]
      public void should_use_the_downloaded_file()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).MustNotHaveHappened();
      }
   }

   public class When_loading_a_template_of_the_osp_library_that_was_downloaded_before : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private RemoteTemplate _template;
      private string _localFile;

      protected override async Task Context()
      {
         await base.Context();
         _template = sut.TemplateBy(TemplateType.Compound, "Alprazolam");
         _localFile = Path.Combine(_templateFolder, "2.0_Alprazolam-Model.json");
         File.WriteAllText(_localFile, "{}");
      }

      protected override Task Because() => sut.LoadTemplateAsync<Compound>(_template);

      [Observation]
      public void should_not_download_the_template_again()
      {
         A.CallTo(() => _templateDownloader.DownloadAsync(A<Uri>._, A<string>._, A<bool>._)).MustNotHaveHappened();
         A.CallTo(() => _snapshotTask.LoadModelFromProjectFileAsync<Compound>(_localFile, PKSimBuildingBlockType.Compound, "Alprazolam")).MustHaveHappened();
      }
   }

   public class When_retrieving_the_reference_templates_of_a_template_from_an_additional_source : concern_for_RemoteTemplateRepository_with_template_sources
   {
      private RemoteTemplate _template;
      private IReadOnlyList<RemoteTemplate> _references;

      protected override async Task Context()
      {
         await base.Context();
         writeSummary(_sourceSummaryPath, _sourceSummary);
         _template = sut.TemplateBy(TemplateType.Compound, "InHouse");
      }

      protected override Task Because()
      {
         var compound = new Compound();
         compound.AddProcess(new EnzymaticProcess {MoleculeName = "CYP3A4", MetaboliteName = "Meta"});
         _references = sut.AllReferenceTemplatesFor(_template, compound);
         return _completed;
      }

      [Observation]
      public void should_return_reference_templates_pointing_to_the_same_source()
      {
         _references.Count.ShouldBeEqualTo(1);
         _references[0].Name.ShouldBeEqualTo("Meta");
         _references[0].Url.ShouldBeEqualTo(_template.Url);
         _references[0].Source.ShouldBeEqualTo(_source);
      }
   }
}
