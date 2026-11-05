using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class RemoteTemplateRepository : StartableRepository<RemoteTemplate>, IRemoteTemplateRepository
   {
      private readonly IPKSimConfiguration _configuration;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly ISnapshotTask _snapshotTask;
      private readonly IOSPSuiteLogger _logger;
      private readonly ICoreUserSettings _userSettings;
      private readonly ITemplateDownloader _templateDownloader;
      private readonly List<RemoteTemplate> _allTemplates = new List<RemoteTemplate>();
      private readonly object _locker = new object();
      public string Version { get; private set; }

      public RemoteTemplateRepository(
         IPKSimConfiguration configuration,
         IJsonSerializer jsonSerializer,
         ISnapshotTask snapshotTask,
         IOSPSuiteLogger logger,
         ICoreUserSettings userSettings,
         ITemplateDownloader templateDownloader)
      {
         _configuration = configuration;
         _jsonSerializer = jsonSerializer;
         _snapshotTask = snapshotTask;
         _logger = logger;
         _userSettings = userSettings;
         _templateDownloader = templateDownloader;
      }

      public IReadOnlyList<RemoteTemplate> AllTemplatesFor(TemplateType templateType) => All().Where(x => x.Type.Is(templateType)).ToList();

      public RemoteTemplate TemplateBy(TemplateType templateType, string name) => AllTemplatesFor(templateType).FindByName(name);

      public async Task<T> LoadTemplateAsync<T>(RemoteTemplate remoteTemplate)
      {
         var localFile = localFileFor(remoteTemplate);
         if (shouldDownload(remoteTemplate, localFile))
            await _templateDownloader.DownloadAsync(new Uri(remoteTemplate.Url), localFile, allowInteractive: true);

         var buildingBlockType = EnumHelper.ParseValue<PKSimBuildingBlockType>(remoteTemplate.Type.ToString());
         return await _snapshotTask.LoadModelFromProjectFileAsync<T>(localFile, buildingBlockType, remoteTemplate.Name);
      }

      /// <summary>
      ///    A template whose url carries a version is downloaded once. A template of an additional source without version
      ///    information cannot be cached safely and is downloaded every time
      /// </summary>
      private static bool shouldDownload(RemoteTemplate remoteTemplate, string localFile)
      {
         if (!FileHelper.FileExists(localFile))
            return true;

         return !remoteTemplate.IsFromOSPLibrary && string.Equals(remoteTemplate.Version, CoreConstants.DEFAULT_TEMPLATE_VERSION);
      }

      public IReadOnlyList<RemoteTemplate> AllReferenceTemplatesFor<T>(RemoteTemplate remoteTemplate, T loadedTemplate)
      {
         //We only have reference templates for very specific building block types. So we filter for those
         switch (loadedTemplate)
         {
            case Compound compound:
               return metabolitesFor(remoteTemplate, compound);
            case ISimulationSubject simulationSubject:
               return expressionProfileFor(remoteTemplate, simulationSubject);
            default:
               return Array.Empty<RemoteTemplate>();
         }
      }

      public async Task UpdateLocalTemplateSummaryFile()
      {
         await downloadSummaryAsync(new Uri(CoreConstants.REMOTE_TEMPLATE_FILE_URL), _configuration.RemoteTemplateSummaryPath, allowInteractive: false);
         reload();
      }

      public async Task UpdateTemplateSourcesAsync()
      {
         foreach (var source in validTemplateSources())
         {
            await downloadSummaryAsync(source.Uri, summaryPathFor(source), allowInteractive: false);
         }

         reload();
      }

      public async Task LoadUnavailableTemplateSourcesAsync()
      {
         var unavailableSources = validTemplateSources().Where(x => !FileHelper.FileExists(summaryPathFor(x))).ToList();
         if (!unavailableSources.Any())
            return;

         foreach (var source in unavailableSources)
         {
            await downloadSummaryAsync(source.Uri, summaryPathFor(source), allowInteractive: true);
         }

         reload();
      }

      /// <summary>
      ///    Downloads the summary file to a temporary location first so that a failed download never destroys the summary
      ///    downloaded earlier. Returns true if the summary was updated.
      /// </summary>
      private async Task<bool> downloadSummaryAsync(Uri uri, string destination, bool allowInteractive)
      {
         var tempFile = FileHelper.GenerateTemporaryFileName();
         try
         {
            await _templateDownloader.DownloadAsync(uri, tempFile, allowInteractive);
            DirectoryHelper.CreateDirectory(FileHelper.FolderFromFileFullPath(destination));
            FileHelper.Copy(tempFile, destination);
            return true;
         }
         catch (Exception e)
         {
            _logger.AddWarning(PKSimConstants.Warning.CannotUpdateTemplateSummaryFrom(uri.ToString(), e.Message));
            return false;
         }
         finally
         {
            FileHelper.DeleteFile(tempFile);
         }
      }

      private IReadOnlyList<TemplateSource> validTemplateSources()
      {
         var templateSources = _userSettings.TemplateSources ?? Array.Empty<TemplateSource>();
         return templateSources.Where(x => x.HasValidUrl).ToList();
      }

      private string sourceFolderFor(TemplateSource source) => Path.Combine(_configuration.RemoteTemplateFolderPath, source.Key);

      private string summaryPathFor(TemplateSource source) => Path.Combine(sourceFolderFor(source), CoreConstants.REMOTE_TEMPLATE_SUMMARY);

      private string localFileFor(RemoteTemplate template)
      {
         var folder = template.IsFromOSPLibrary ? _configuration.RemoteTemplateFolderPath : sourceFolderFor(template.Source);
         DirectoryHelper.CreateDirectory(folder);
         return Path.Combine(folder, fileNameWithVersionFor(template));
      }

      private IReadOnlyList<RemoteTemplate> expressionProfileFor(RemoteTemplate remoteTemplate, ISimulationSubject simulationSubject)
      {
         //This will for now assume that the expression profile templates are in the same file
         return simulationSubject.AllExpressionProfiles()
            .Select(x => x.Name)
            .Select(name => referenceTemplateFor(remoteTemplate, TemplateType.ExpressionProfile, name))
            .ToList();
      }

      private IReadOnlyList<RemoteTemplate> metabolitesFor(RemoteTemplate remoteTemplate, Compound compound)
      {
         //This will for now assume that the metabolite template are in the same file
         return compound.AllProcesses<EnzymaticProcess>()
            .Select(x => x.MetaboliteName)
            .Where(x => x.StringIsNotEmpty())
            .Distinct()
            .Select(meta => referenceTemplateFor(remoteTemplate, TemplateType.Compound, meta))
            .ToList();
      }

      private static RemoteTemplate referenceTemplateFor(RemoteTemplate remoteTemplate, TemplateType templateType, string name) => new RemoteTemplate
      {
         Url = remoteTemplate.Url,
         Version = remoteTemplate.Version,
         RepositoryUrl = remoteTemplate.RepositoryUrl,
         Source = remoteTemplate.Source,
         Type = templateType,
         Name = name
      };

      protected override void DoStart() => reload();

      private void reload()
      {
         lock (_locker)
         {
            //a failed previous start may have partially filled the repository (lazy Start retries); begin clean
            _allTemplates.Clear();
            Version = null;

            loadOSPLibrary();
            validTemplateSources().Each(loadTemplateSource);
         }
      }

      private void loadOSPLibrary()
      {
         //the summary file is provisioned best-effort (the copy from the all-users folder swallows IOException),
         //so a missing file means no remote templates are available, not a broken installation
         var summaryPath = _configuration.RemoteTemplateSummaryPath;
         if (!FileHelper.FileExists(summaryPath))
         {
            _logger.AddDebug(PKSimConstants.UI.RemoteTemplateSummaryNotFoundMessage(summaryPath));
            return;
         }

         var summary = deserializeSummary(summaryPath);
         addTemplatesFrom(summary, source: null, baseUri: new Uri(CoreConstants.REMOTE_TEMPLATE_FILE_URL));
         Version = summary.Version;
      }

      private void loadTemplateSource(TemplateSource source)
      {
         var summaryPath = summaryPathFor(source);
         if (!FileHelper.FileExists(summaryPath))
            return;

         try
         {
            addTemplatesFrom(deserializeSummary(summaryPath), source, source.Uri);
         }
         catch (Exception e)
         {
            //an invalid summary should not prevent the other sources from being loaded
            _logger.AddWarning(PKSimConstants.Warning.CannotLoadTemplateSource(source.Name, source.Url, e.Message));
         }
      }

      private RemoteTemplates deserializeSummary(string summaryPath) => Task.Run(() => _jsonSerializer.Deserialize<RemoteTemplates>(summaryPath)).Result;

      private void addTemplatesFrom(RemoteTemplates summary, TemplateSource source, Uri baseUri)
      {
         var templates = summary?.Templates ?? Array.Empty<RemoteTemplate>();
         templates.Each(x =>
         {
            x.Source = source;
            x.Url = absoluteUrlFor(baseUri, x.Url);
            var (version, repositoryUrl) = extractDataFromUrl(x.Url);
            x.Version = version;
            x.RepositoryUrl = repositoryUrl;
         });
         _allTemplates.AddRange(templates);
      }

      /// <summary>
      ///    A template url may be relative to the summary file. This allows a library to be hosted anywhere (web server,
      ///    network share, local clone) without rewriting the summary
      /// </summary>
      private static string absoluteUrlFor(Uri baseUri, string url)
      {
         if (string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            return url;

         return Uri.TryCreate(baseUri, url, out var absoluteUri) ? absoluteUri.AbsoluteUri : url;
      }

      private (string version, string repositoryUrl) extractDataFromUrl(string url)
      {
         var invalidUrl = (CoreConstants.DEFAULT_TEMPLATE_VERSION, url);
         if (string.IsNullOrEmpty(url))
            return invalidUrl;

         //a Url looks like so (5 segments, 1, 2 and 3 of interest for our use case)
         //https://raw.githubusercontent.com/Open-Systems-Pharmacology/Rifampicin-Model/v1.1/Rifampicin-Model.json
         //and we want to create something like this
         //https://github.com/Open-Systems-Pharmacology/Rifampicin-Model/tree/v1.1

         var segments = new Uri(url).Segments;
         //The url does not respect the expected format. Returned the default raw url
         if (segments.Length != 5)
            return invalidUrl;

         var versionSegment = segments[3];
         if (!versionSegment.StartsWith("v") || !versionSegment.EndsWith("/"))
            return invalidUrl;

         //Removes the v at the beginning end the "/" at the ned
         var version = versionSegment.Substring(1, versionSegment.Length - 2);
         return (version, $"https://github.com/{segments[1]}{segments[2]}tree/{versionSegment}");
      }

      public override IEnumerable<RemoteTemplate> All()
      {
         Start();
         //the list may be reloaded in the background when a summary was updated
         lock (_locker)
         {
            return _allTemplates.ToList();
         }
      }

      private string fileNameWithVersionFor(RemoteTemplate template)
      {
         var url = template.Url;
         var fileName = new Uri(url).Segments.Last();
         return $"{template.Version}_{fileName}";
      }
   }
}
